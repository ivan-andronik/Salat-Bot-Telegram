using Telegram.Bot.Types;
using SalatBot.Scripts.Main.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using File = System.IO.File;

namespace SalatBot.Scripts.Main
{
    public class Main : MonoBehaviour, IMessageProcessor
    {
        #region Internal
        [System.Serializable]
        public class View
        {
            [Header("Buttons")] public Button getMe;
            public Button sendDocument;
            public Button sendPhoto;
            public Button sendMessage;
            public Button startStopBot;

            [Space(10)] [Header("Text")] public Text inputText;
        }
        #endregion

        #region Serialize Fields
        [SerializeField] private Telegram telegram;
        [SerializeField] private View view;
        #endregion

        #region Private
        private bool m_botIsRunning;
        private Message m_processMsg;
        private Message m_lastMessage;
        #endregion

        #region Unity Methods
        private void Start()
        {
            m_processMsg = null;
            m_lastMessage = null;

            // Bot info
            view.getMe.onClick.AddListener(() => telegram.GetMe());

            // Send any document
            view.sendDocument.onClick.AddListener(() =>
            {
                // send txt file
                var bytes = System.Text.Encoding.UTF8.GetBytes(GetUserInfo());
                telegram.SendFile(bytes, "file.txt", GetUserInfo());
            });

            // send file as photo
            view.sendPhoto.onClick.AddListener(() =>
            {
                var screenshotName = "Screenshot.png";
                ScreenCapture.CaptureScreenshot("Assets/" + screenshotName);
                try
                {
                    var bytes = File.ReadAllBytes(Application.dataPath + "/" + screenshotName);
                    telegram.SendPhoto(bytes, screenshotName);
                }
                catch
                {
                    // ignored
                }
            });

            // send message
            view.sendMessage.onClick.AddListener(() => { telegram.SendMessage(view.inputText.text); });

            view.startStopBot.onClick.AddListener(() =>
            {
                telegram.GetUpdates(this);
                if (m_botIsRunning)
                    Program.StopBot();
                else
                    Program.StartBot(this);
                m_botIsRunning = !m_botIsRunning;
            });
        }

        private void Update()
        {
            view.startStopBot.GetComponentInChildren<Text>().text = (m_botIsRunning ? "STOP" : "START") + " BOT";

            if (null != m_processMsg)
            {
                m_lastMessage = m_processMsg;
                m_processMsg = null;
            }
        }
        #endregion

        #region Public Methods
        public void Process(Message msg)
        {
            m_processMsg = msg;
        }

        public void SendMessage(string text)
        {
            if (null == m_lastMessage)
                return;
            m_lastMessage.Text = text;
            Program.SendMessage(m_lastMessage);
        }
        
        public void SendInlineMessage(Message message, string[][] keyboardData)
        {
            if (null == message)
                return;
            
            Program.SendInlineKeyboard(message, keyboardData, message.Text, true);
        }
        #endregion

        #region Private Methods
        private string GetUserInfo()
        {
            return string.Format("From :\n{0}\n{1}", SystemInfo.deviceName, SystemInfo.deviceModel);
        }
        #endregion
    }
}