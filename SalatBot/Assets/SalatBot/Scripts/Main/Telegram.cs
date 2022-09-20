using System.Collections;
using SalatBot.Scripts.Config;
using SalatBot.Scripts.Main.Interfaces;
using UnityEngine;
using UnityEngine.Networking;

namespace SalatBot.Scripts.Main
{
    public class Telegram : MonoBehaviour
    {
        #region Serializefields
        [SerializeField] private string m_chatId = "0000000"; // ID (you can know your id via @userinfobot)
        #endregion

        #region Private
        private IMessageProcessor m_messageProcessor;
        #endregion

        #region Public
        public string API_URL
        {
            get
            {
                return string.Format("https://api.telegram.org/bot{0}/", Configuration.BotToken);
            }
        }
        #endregion

        #region Public Methods
        public void GetMe()
        {
            WWWForm form = new WWWForm();
            UnityWebRequest www = UnityWebRequest.Post(API_URL + "getMe", form);
            StartCoroutine(SendRequest(www));
        }

        public void SendFile(byte[] bytes, string filename, string caption = "")
        {
            WWWForm form = new WWWForm();
            form.AddField("chat_id", m_chatId);
            form.AddField("caption", caption);
            form.AddBinaryData("document", bytes, filename, "filename");
            UnityWebRequest www = UnityWebRequest.Post(API_URL + "sendDocument?", form);
            StartCoroutine(SendRequest(www));
        }

        public void SendPhoto(byte[] bytes, string filename, string caption = "")
        {
            WWWForm form = new WWWForm();
            form.AddField("chat_id", m_chatId);
            form.AddField("caption", caption);
            form.AddBinaryData("photo", bytes, filename, "filename");
            UnityWebRequest www = UnityWebRequest.Post(API_URL + "sendPhoto?", form);
            StartCoroutine(SendRequest(www));
        }

        public new void SendMessage(string text)
        {
            WWWForm form = new WWWForm();
            form.AddField("chat_id", m_chatId);
            form.AddField("text", text);
            form.AddField("reply_markup", "{\"text\":\"test\", \"callback_data\":\"test_callback_data\"}");
            UnityWebRequest www = UnityWebRequest.Post(API_URL + "sendMessage?", form);
            StartCoroutine(SendRequest(www));
        }

        public void GetUpdates(IMessageProcessor messageProcessor)
        {
            m_messageProcessor = messageProcessor;
            
            WWWForm form = new WWWForm();
            UnityWebRequest www = UnityWebRequest.Post(API_URL + "getUpdates?", form);
            StartCoroutine(SendRequest(www));
        }
        #endregion

        #region Coroutines
        private IEnumerator SendRequest(UnityWebRequest www)
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var w = www;
                m_messageProcessor.SendMessage(w.downloadHandler.text);
                Debug.Log("Success!\n" + w.downloadHandler.text);
            }
        }
        #endregion
        
    }
}