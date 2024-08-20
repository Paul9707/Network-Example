using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    public Text playerName;
    public InputField playerNameInput;
    public Button playerNameChangeButton;

    [Header("Main Menu")]
    public RectTransform mainMenuPanel;
    public Button createRoomButton;
    public Button findRoomButton;
    public Button randomRoomButton;
    public Button logoutButton;

    [Header("Create Room Menu")]
    public RectTransform createRoomMenuPanel;
    public InputField roomNameInput;
    public InputField playerNumInput;
    public Button createButton;
    public Button cancelButton;

    private void Awake()
    {
        createRoomButton.onClick.AddListener(CreateRoomButtonClick);
        findRoomButton.onClick.AddListener(FindRoomButtonClick);
        randomRoomButton.onClick.AddListener(RandomRoomButtonClick);
        logoutButton.onClick.AddListener(LogOutButtonClick);
        createButton.onClick.AddListener(CreateButtonClick);
        cancelButton.onClick.AddListener(CancelButtonClick);

    }

    private void OnEnable()
    {
        playerName.text = $"�ȳ��ϼ���, {PhotonNetwork.LocalPlayer.NickName} ��";
        mainMenuPanel.gameObject.SetActive(true);
        createRoomMenuPanel.gameObject.SetActive(false);
    }

    private void CreateRoomButtonClick() // �� ������ư
    {
        mainMenuPanel.gameObject.SetActive(false);
        createRoomMenuPanel.gameObject.SetActive(true);
    }
    private void FindRoomButtonClick() // �� ����� �޾ƿ��� ���� �κ� ����.
    {
        PhotonNetwork.JoinLobby();
    }

    private void RandomRoomButtonClick() // ������ �濡 ���� 
    {
        RoomOptions option = new()
        {
           
            MaxPlayers = 8,
        };
        PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: option);
    }

    private void LogOutButtonClick() // �α׾ƿ�
    {
        mainMenuPanel.gameObject.SetActive(false);
        PhotonNetwork.Disconnect();
        //PanelManager.Instance.PanelOpen("Login");
    }

    private void CreateButtonClick() // ����� ��ư 
    {
        string roomName = roomNameInput.text;
        int maxPlayer = int.Parse(playerNumInput.text);
        /*if (int.TryParse(playerNumInput.text, out maxPlayer))
        {
            
        }*/

        if (string.IsNullOrEmpty(roomName))
        {
            // ���� �� ��ȣ�� ���� �� �����Ƿ� ��� �� �� ������ �˻簡 �ʿ��մϴ�.
            roomName = $"Room {Random.Range(0, 1000)}";
        }

        if (maxPlayer <= 0)
        {
            maxPlayer = 8;
        }

        PhotonNetwork.CreateRoom(roomName,
                new RoomOptions() // �⺻ �����ڿ� ���ÿ� �ʱ�ȭ -> �޸� �Ƴ���
                {
                    
                    MaxPlayers = maxPlayer,
                }
            ); 

    }

    private void CancelButtonClick() // �� ������ư�� ��� ��ư
    {
        mainMenuPanel.gameObject.SetActive(true);
        createRoomMenuPanel.gameObject.SetActive(false);
    }

}
