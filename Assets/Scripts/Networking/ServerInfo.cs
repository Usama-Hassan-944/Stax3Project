using UnityEngine;
using UnityEngine.SceneManagement;

namespace sy.Data
{
    public static class ServerInfo
    {
        public static string LobbyName;

        public static int MaxUsers
        {
            get => 2;
        }
    }
}