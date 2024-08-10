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

        public static List<int> PlayerCharacters;

        public static void SetCharacters(int ID)
        {
            if (PlayerCharacters == null)
                PlayerCharacters = new List<int>();

            PlayerCharacters.Add(ID);
        }

        public static void DeleteCharacters(int ID)
        {
            if (PlayerCharacters == null)
                return;

            PlayerCharacters.Remove(ID);
        }
    }
}