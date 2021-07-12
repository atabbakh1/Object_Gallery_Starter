using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

using System.Linq;

public class MultiplayerLauncher : MonoBehaviourPunCallbacks, ILobbyCallbacks
{

    [SerializeField]
    private string gameVersion = "0.0.1";


    [Space(5)]
    public Text debugText;

    [Space(5)]
    public GameObject roomSetupUI;
    public GameObject joinRoomButton;

    [Space(5)]
    public string sceneToLoad;
    public string roomName;
    public int maxPlayersPerRoom = 15;



    private string userName;
    private int roomCounter = 0;
    private RoomOptions roomOptions ;


    private void Start()
    {
        userName = Random.Range(0, 1000).ToString();

        print("CONNECTING...");

        roomSetupUI.SetActive(false);
        ConnectToPhoton();

    }

    public void JoinRoomClicked()
    {
        JoinRoom();
    }



    #region NETWORK METHODS
    public void ConnectToPhoton()
    {
        debugText.text = "Connecting to network...";
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.ConnectUsingSettings();
        print("I just CONNECTED to network");
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            roomOptions = new RoomOptions();
            //set the player name
            PhotonNetwork.LocalPlayer.NickName = userName;
            roomOptions.MaxPlayers = (byte)maxPlayersPerRoom;
            roomOptions.IsOpen = true;
            TypedLobby myLobby = new TypedLobby(roomName + roomCounter.ToString(), LobbyType.Default);
            PhotonNetwork.JoinOrCreateRoom(roomName + roomCounter.ToString(), roomOptions, myLobby, null);

        }
    }


    public void LoadScene()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 0)
        {
            PhotonNetwork.LoadLevel(sceneToLoad);
            print("I just LOADED the scene: " + sceneToLoad);
        }
    }

    #endregion

    #region CALLBACK OVERRIDES

    public override void OnConnected()
    {
        base.OnConnected();


        debugText.color = Color.green;
        debugText.text = "Connected";

        //turn on room joining UI
        roomSetupUI.SetActive(true);

    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        //controlPanel.SetActive(true);
        debugText.color = Color.white;
        debugText.text = "Disconnected from network due to: " + cause.ToString();
    }


    public override void OnJoinedRoom()
    {
        print("Room JOINING successful");
        print(PhotonNetwork.CurrentRoom.Name);

        //load the scene here since OnJoinedRoom get called on both JOIN and CREATE
        LoadScene();
    }

    public override void OnCreatedRoom()
    {

        joinRoomButton.SetActive(false);

        debugText.color = Color.green;
        debugText.text = "You are the host of this room";

        PhotonNetwork.CurrentRoom.IsOpen = true;
        print("Room CREATION successful");

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {

        //debugText.color = Color.red;
        //debugText.text = "Room was not created. " + message;
        //Debug.Log("I failed to create room: " + roomName + roomCounter.ToString());
       // Debug.Log("I failed to create room because: " + message);

       // print(message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //if room joining fails then create the next one in the sequence 
        Debug.Log("I failed to join room: " + roomName + roomCounter.ToString());
        Debug.Log("I failed to join room because: " + message);

        roomCounter += 1;
        roomOptions.MaxPlayers = (byte)maxPlayersPerRoom;
        roomOptions.IsOpen = true;
        TypedLobby myLobby = new TypedLobby(roomName + roomCounter.ToString(), LobbyType.Default);
        PhotonNetwork.JoinOrCreateRoom(roomName + roomCounter.ToString(), roomOptions, myLobby, null);
    }

    #endregion
}
