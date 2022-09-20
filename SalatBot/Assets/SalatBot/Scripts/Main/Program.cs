using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using SalatBot.Scripts.Config;
using SalatBot.Scripts.Main.Interfaces;
using UnityEngine;

namespace SalatBot.Scripts.Main
{
    public static class Program
    {
        private static TelegramBotClient Bot;
        private static CancellationTokenSource cts;
        private static IMessageProcessor m_messageProcessor;
        public static void StopBot()
        {
            // Send cancellation request to stop bot
            cts.Cancel();
        }

        public static async Task StartBot(IMessageProcessor messageProcessor)
        {
            m_messageProcessor = messageProcessor;
            Bot = new TelegramBotClient(Configuration.BotToken);

            var me = await Bot.GetMeAsync();
            Console.Title = me.Username;

            cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            Bot.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);

            Debug.Log($"Start listening for @{me.Username}");
            Console.ReadLine();
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                // UpdateType.Unknown:
                // UpdateType.ChannelPost:
                // UpdateType.EditedChannelPost:
                // UpdateType.ShippingQuery:
                // UpdateType.PreCheckoutQuery:
                // UpdateType.Poll:
                UpdateType.Message            => BotOnMessageReceived(update.Message),
                UpdateType.EditedMessage      => BotOnMessageReceived(update.Message),
                UpdateType.CallbackQuery      => BotOnCallbackQueryReceived(update.CallbackQuery),
                UpdateType.InlineQuery        => BotOnInlineQueryReceived(update.InlineQuery),
                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(update.ChosenInlineResult),
                _                             => UnknownUpdateHandlerAsync(update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        public static async Task<Message> SendMessage(Message message)
        {
            return await Bot.SendTextMessageAsync(chatId: message.Chat.Id,
                text: message.Text,
                replyMarkup: new ReplyKeyboardRemove());
        }

        private static async Task BotOnMessageReceived(Message message)
        {
            Debug.Log($"Receive message type: {message.Type}");
            Debug.Log($"Receive message text: {message.Text}");
            
            if (message.Type != MessageType.Text)
                return;
            
            var from = message.From;
            // if (message.ReplyToMessage != null ) { }

            var action = (message.Text.Split(' ').First()) switch
            {
                _              => Usage(message, from)
            };
            var sentMessage = await action;
            Debug.Log($"The message was sent with id: {sentMessage.MessageId}");

            static async Task<Message> Usage(Message message, User from)
            {
                var usage = $"пока нихуя";
                
                return await Bot.SendTextMessageAsync(chatId: message.Chat.Id, text: usage, 
                    replyMarkup: new ReplyKeyboardRemove());
            }
        }

        // Process Inline Keyboard callback data
        private static async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        {
            var from = callbackQuery.From;
            var message = callbackQuery.Message;
            var data = callbackQuery.Data;
            
            var action = (callbackQuery.Data.Split(' ').First()) switch
            { 
                "set_english"      => RequestData(message, from,"set_english"),
                "set_russian"      => RequestData(message, from, "set_russian"),
               // "prediction_month" => RequestData(message, from, data),
                _ => null
            };

            await Bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"Received {callbackQuery.Data}");
            
            if (action != null) 
                await action;
        }
        
        private static async Task<Message> RequestData(Message message, User from, string text)
        {
            message.Text = text;
            message.From = from;
            m_messageProcessor.Process(message);
                
            return message;
        }

        // private static async Task<Message> SendMenuInlineKeyboard(Message message, User from, bool editMessage = false)
        // {
        //     var keyboardData = new []
        //     {
        //         new[] {Translate("language", from), Translate("predictions", from), Translate("predictions_percent_desc", from)},  //Text
        //         new[] {"language",                      "predictions",                      "predictions_percent"                    }   //Callback
        //     };
        //
        //     return await SendInlineKeyboard(message, keyboardData, Translate("menu_desc", from), editMessage);
        // }

        // private static async Task<Message> SendLanguageInlineKeyboard(Message message, User from, bool editMessage = false)
        // {
        //     var keyboardData = new []
        //     {
        //         new[] {Translate("lang_english", from), Translate("lang_russian", from), Translate("back", from)},  //Text
        //         new[] {"set_english",                       "set_russian",                      "menu"}                        //Callback
        //     };
        //
        //     return await SendInlineKeyboard(message, keyboardData, Translate("lang_select", from), editMessage);
        // }
        
        public static async Task<Message> SendInlineKeyboard(Message message, string[][] keyboardData, string messageText, bool editMessage = false)
        {
            await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            
            var inlineKeyboard = new InlineKeyboardMarkup(GetInlineKeyboard(keyboardData));
                
            if (!editMessage)
            {
                return await Bot.SendTextMessageAsync(chatId: message.Chat.Id, text: messageText,
                    replyMarkup: inlineKeyboard);
            }

            return await Bot.EditMessageTextAsync(chatId: message.Chat.Id, messageId: message.MessageId, messageText,
                replyMarkup: inlineKeyboard);
        }

        private static InlineKeyboardButton[][] GetInlineKeyboard(string [][] stringArray)
        {
            var keyboardInline = new InlineKeyboardButton[stringArray[0].Length][];
            for (var i = 0; i < stringArray[0].Length; i++)
            {
                var keyboardButton = new InlineKeyboardButton
                {
                    Text = stringArray[0][i],
                    CallbackData = stringArray[1][i],
                };

                keyboardInline[i] = new InlineKeyboardButton[1];
                keyboardInline[i][0] = keyboardButton;
            }
            
            return keyboardInline;
        }

        #region Inline Mode

        private static async Task BotOnInlineQueryReceived(InlineQuery inlineQuery)
        {
            Debug.Log($"Received inline query from: {inlineQuery.From.Id}");

            InlineQueryResultBase[] results = {
                // displayed result
                new InlineQueryResultArticle(
                    id: "3",
                    title: "TgBots",
                    inputMessageContent: new InputTextMessageContent(
                        "hello"
                    )
                )
            };

            await Bot.AnswerInlineQueryAsync(inlineQuery.Id,
                                             results,
                                             isPersonal: true,
                                             cacheTime: 0);
        }

        private static Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult)
        {
            Debug.Log($"Received inline result: {chosenInlineResult.ResultId}");
            return Task.CompletedTask;
        }

        #endregion

        private static Task UnknownUpdateHandlerAsync(Update update)
        {
            Debug.Log($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Debug.Log(exception);
            Debug.Log(ErrorMessage);
            return Task.CompletedTask;
        }

        // private static string Translate(string id, User from)
        // {
        //     var lang = GlobalData.Instance.UsersData.GetUserData(from).UserLanguage;
        //     return TranslationManager.Instance.Get(id, lang);
        // }
    }
}
