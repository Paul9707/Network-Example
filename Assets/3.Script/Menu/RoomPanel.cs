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
            //�÷��̾� ��Ͽ� �÷��̾� �̸�ǥ �ϳ��� ����
            JoinPlayer(player);
        }
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        /*if (PhotonNetwork.IsMasterClient)
        {
            //���� �϶�
        }
        else
        {
            //������ �ƴҶ�
        }*/
        // �濡 ���� ���� ��, ������ �� �ε� ���ο� ���� �Բ� �� �ε�
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
        // ���� ���� ��ư
        // ������ �� �ε� ���
        //SceneManager.LoadScene("GameScene");

        // Photon�� ���� �ÿ��̾��� ���� ����ȭ �Ͽ� �ε� 
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }

    }
    private void CancelButtonClick()
    {
        PhotonNetwork.LeaveRoom();
        // ���� ������ ���ÿ� ���ε带 ������� �ʰڴ� ��, ���� ���� ������ ������ ������ ���������� ���Ӿ����� ������ ���ڴ�.
        PhotonNetwork.AutomaticallySyncScene = false;
    }
}
