using sy.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace sy.Profile
{
    public class PlayerProfile : MonoBehaviour
    {
        #region Fields

        public TMPro.TMP_InputField nicknameInput;
        public GameObject ProfilePanel;
        public GameObject userNamePanel;
        public GameObject RulesPanel;

        #endregion

        #region Methods

        public void onProfileMenu()
        {
            ProfilePanel.SetActive(true);
        }
        public void ExitProfile()
        {
            ProfilePanel.SetActive(false);
        }
        public void onRules()
        {
            RulesPanel.SetActive(true);
        }
        public void ExitRules()
        {
            RulesPanel.SetActive(false);
        }
        public void onUserDetailsChanged()
        {
            ClientInfo.Username = nicknameInput.text;
            nicknameInput.text = ClientInfo.Username;
            userNamePanel.SetActive(false);
        }

        public void onSetUserName()
        {
            userNamePanel.SetActive(true);
            nicknameInput.text = ClientInfo.Username;
            string playerName = ClientInfo.Username;

            if (playerName.Equals(""))
            {
                Debug.LogError("Player Name is invalid.");
            }
        }

        public void closeUserName()
        {
            userNamePanel.SetActive(false);
        }

        #endregion
    }
}
