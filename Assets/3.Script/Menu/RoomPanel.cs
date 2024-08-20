using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RoomPanel : MonoBehaviour
{
    public Text roomTitleText;
    public RectTransform playerList;
    public GameObject playerTextPrefab;

    public Button startButton;
    public Button cancelButton;

    private void Awake()
    {
        startButton.onClick.AddListener(StartButtonClick);
        cancelButton.onClick.AddListener(CancelButtonClick);
    }

    private void OnDisable()
    {
        foreach (Transform child in playerList)
        {
            Destroy(child.gameObject);
        }

    }

    private void OnEnable()
    {
        roomTitleText.text = PhotonNetwork.CurrentRoom.Name;

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            //플레이어 목록에 플레이어 이름표 하나씩 생성
            JoinPlayer(player);
        }
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        /*if (PhotonNetwork.IsMasterClient)
        {
            //방장 일때
        }
        else
        {
            //방장이 아닐때
        }*/
        // 방에 입장 했을 때, 방장의 씬 로드 여부에 따라 함께 씬 로드
        PhotonNetwork.AutomaticallySyncScene = true;

    }

    public void JoinPlayer(Player newPlayer)
    {
        GameObject playerName = Instantiate(playerTextPrefab, playerList, false);
        playerName.name = newPlayer.NickName;
        playerName.GetComponent<Text>().text = newPlayer.NickName;
    }

    public void LeavePlayer(Player leavePlayer)
    {
        GameObject leaveTarget = playerList.Find(leavePlayer.NickName).gameObject;
        Destroy(leaveTarget);
    }

    private void StartButtonClick()
    {
        // 게임 시작 버튼
        // 기존의 씬 로드 방식
        //SceneManager.LoadScene("GameScene");

        // Photon을 통해 플에이어들과 씬을 동기화 하여 로드 
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }

    }
    private void CancelButtonClick()
    {
        PhotonNetwork.LeaveRoom();
        // 방을 떠남과 동시에 씬로드를 허용하지 않겠다 즉, 내가 방을 떠나는 순간에 게임이 시작했을때 게임씬으로 입장을 막겠다.
        PhotonNetwork.AutomaticallySyncScene = false;
    }
}
