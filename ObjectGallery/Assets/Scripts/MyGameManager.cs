using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.PunBasics;
using UnityEngine.UI;


public class MyGameManager : MonoBehaviour
{

    public string launchingSceneName;
    public Text roomInfo;
    private GameObject userPrefab;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(launchingSceneName);
        }
        else
        {
            roomInfo.text = PhotonNetwork.CurrentRoom.Name;
            if(PhotonNetwork.InRoom)
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    userPrefab = PhotonNetwork.Instantiate("Player",
                                                            Vector3.zero,
                                                            Quaternion.identity, 0);

                }
            }
        }
    }
}
