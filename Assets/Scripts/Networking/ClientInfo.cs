using System.Collections.Generic;
using UnityEngine;

namespace sy.Data
{
    public static class ClientInfo
    {
        public static string Username
        {
            get => PlayerPrefs.GetString("username", "BOT");
            set => PlayerPrefs.SetString("username", value);
        }

        public static string LobbyName
        {
            get => PlayerPrefs.GetString("lobby", "");
            set => PlayerPrefs.SetString("lobby", value);
        }
    }
}