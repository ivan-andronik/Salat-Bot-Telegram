using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using UnityEngine;

namespace SalatBot.Scripts.Data.UserData
{
    public class UsersData : ICollection<UserDataItem>
    {
        private readonly List<UserDataItem> m_items;

        public delegate void UsersDataChangedHandler(UserDataItem item);

        public event UsersDataChangedHandler OnUsersDataChanged = null;

        public UsersData(List<UserDataItem> collection)
        {
            if (collection == null || !collection.Any())
            {
                m_items = new List<UserDataItem>();
                return;
            }
            
            Debug.Log("add collection " + collection.Count);
            m_items = collection;
        }

        public UserDataItem GetUserData(User tgUser)
        {
            var item = m_items.FirstOrDefault(x => x.TgUser.Id == tgUser.Id);
            if (item != null) return item;

            Debug.Log($"Added new user to database - {tgUser.Username}");
            
            item = new UserDataItem{TgUser = tgUser, UserLanguage = "English"};
            m_items.Add(item);
            
            OnUsersDataChanged?.Invoke(item);
            return item;
        }

        public void EditUserData(UserDataItem item)
        {
            var idx = m_items.FindIndex(x => x.TgUser.Id == item.TgUser.Id);
            m_items[idx] = item;
            OnUsersDataChanged?.Invoke(item);
        }

        #region Implementation
        public IEnumerator<UserDataItem> GetEnumerator() =>
            m_items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        
        public void Add(UserDataItem item) =>
            m_items.Add(item);

        public void Clear() => m_items.Clear();

        public bool Contains(UserDataItem item) =>
            m_items.Contains(item);

        public void CopyTo(UserDataItem[] array, int arrayIndex) =>
            m_items.CopyTo(array, arrayIndex);

        public bool Remove(UserDataItem item) =>
            m_items.Remove(item);

        public int Count => m_items.Count;
        public bool IsReadOnly => true;
        #endregion
    }
}