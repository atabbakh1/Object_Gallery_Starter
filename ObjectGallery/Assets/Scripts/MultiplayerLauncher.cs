using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace Photon.Pun.Demo.PunBasics
{
    public class MultiplayerLauncher : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameObject UISetup;

        [SerializeField]
        private Text statusLog;

        [SerializeField]
        private byte maxPlayerCount = 20;

        [SerializeField]
        private string sceneToLoad;

        [SerializeField]
        private string roomName = "";

        string gameVersion = "1.0";


        [Space(5)]
        public GameObject joinRoomButton;

        string playerName = "";

        // Start Method
        void Start()
        {
            joinRoomButton.SetActive(false);
            playerName = Random.Range(0, 1000).ToString();

            statusLog.text = "Connecting to network...";
            ConnectToPhoton();
        }

        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        #region MULTIPLAYER METHODS
        void ConnectToPhoton()
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings(); 
        }

        public void JoinRoom()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName; 
                Debug.Log("CONNECTED! | Joining room: " + roomName);

                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = maxPlayerCount;
                TypedLobby typedLobby = new TypedLobby(roomName, LobbyType.Default); 
                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby); 
            }
        }

        public void LoadArena()
        {
            PhotonNetwork.LoadLevel(sceneToLoad);
        }
        #endregion



        #region PHOTON CALLBACKS
        // Photon Methods
        public override void OnConnected()
        {

            base.OnConnected();
            joinRoomButton.SetActive(true);

            statusLog.text = "Connected successfully!";
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            UISetup.SetActive(true);
            statusLog.text = "Disconnected. Please check your connection.";
        }

        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                
                statusLog.text = "Your are the host of this room";
                LoadArena();
            }
            else
            {
                statusLog.text = "Joining existing room...";
            }
        }

        #endregion
    }
}
