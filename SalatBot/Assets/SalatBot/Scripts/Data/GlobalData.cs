using System;
using System.Collections.Generic;
using SalatBot.Scripts.Config;
using SalatBot.Scripts.Data.UserData;
using SalatBot.Scripts.Data.Utils;
using UnityEngine;

namespace SalatBot.Scripts.Data
{
    public class GlobalData : MonoBehaviour
    {
        #region Singleton
        public static GlobalData Instance { get; private set; } = null;

        private void InitInstance()
        {
            if (Instance == null) Instance = this;
            else if (Instance == this) Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }
        #endregion

        #region Variables
        public UsersData UsersData { get; private set; }
        public List<DateTime> PredictionsDates { get; private set; }
        public int RightPredictionsPercent { get; set; } = 0;

        private string m_dataPath = "";
        #endregion
        
        #region Unity Methods
        private void Awake()
        {
            InitInstance();
            Initialization();
        }

        private void OnDestroy()
        {
            UsersData.OnUsersDataChanged -= UsersDataChangedHandler;
        }
        #endregion

        #region Private Methods
        private void Initialization()
        {
            m_dataPath = Application.persistentDataPath;
            var usersDataList = DataSerializer.GetSerializedData<List<UserDataItem>>(m_dataPath + Configuration.UsersDataPath);
            
            UsersData = new UsersData(usersDataList);
            UsersData.OnUsersDataChanged += UsersDataChangedHandler;
        }
        
        private void UsersDataChangedHandler(UserDataItem item) =>
            DataSerializer.SerializeData(UsersData, m_dataPath + Configuration.UsersDataPath);
        #endregion
    }
}