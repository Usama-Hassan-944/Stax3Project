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
        public GameObject profilePanel;
        public GameObject userNamePanel;

        #endregion

        #region Methods

        public void onProfileMenu()
        {
            profilePanel.SetActive(true);
        }

        public void ExitProfile()
        {
            profilePanel.SetActive(false);
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
