using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine.SceneManagement;

namespace sy.Networking
{
    public class TestRelay : MonoBehaviour
    {
        public async Task<string> CreateRelay()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log("Join code: " + joinCode);
                RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                return joinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return null;
            }

        }

        public async void JoinRelay(string joinCode)
        {
            try
            {
                Debug.Log("Joining relay with code: " + joinCode);
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartClient();
                await WaitForClientConnection();
                SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
            }

            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
        }

        public void Disconnect()
        {
            NetworkManager.Singleton.Shutdown();
        }

        private async Task WaitForClientConnection()
        {
            while (!NetworkManager.Singleton.IsClient)
            {
                await Task.Delay(100);
            }
        }
    }
}