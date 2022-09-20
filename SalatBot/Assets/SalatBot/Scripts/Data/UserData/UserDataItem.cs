using Telegram.Bot.Types;

namespace SalatBot.Scripts.Data.UserData
{
  public class UserDataItem
  {
    public User TgUser { get; set; }
    public string UserLanguage { get; set; }
  }
}