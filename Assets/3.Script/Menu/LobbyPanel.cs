using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    public RectTransform roomListRect;
    private List<RoomInfo> currentRoomList = new List<RoomInfo>();
    public Button roomButtonPrefab;
    public Button backButton;

    private void Awake()
    {
        backButton.onClick.AddListener(BackButtonClick);
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        List<RoomInfo> destroryCandidate /* �ı��� �ĺ��� */ =
            currentRoomList.FindAll((x) => false == roomList.Contains(x));

        foreach (RoomInfo roomInfo in roomList)
        {
            if (currentRoomList.Contains(roomInfo)) continue;
            AddRoomButton(roomInfo);
        }

        foreach (Transform child in roomListRect)  
        {
            if (destroryCandidate.Exists((x) => x.Name == child.name))
            {
                Destroy(child.gameObject);
            }
        }
        currentRoomList = roomList;
                                        
    }

    private void OnDisable()
    {
        foreach (Transform child in roomListRect) // Transform�� var�� ������ ��� ������ ���� ������ object�� �ڽ��� �Ǿ ��ȯ�ϱ� ������ ��ڽ��� ���� ����� �Ѵ�.
        {
            Destroy(child.gameObject);
        }
    }

    public void AddRoomButton(RoomInfo roomInfo) // RoomInfo List�� ���� ���������� �Ѱ��� �� ���� ��ư�� ����
    {
        Button joinButton = Instantiate(roomButtonPrefab, roomListRect, false);
        joinButton.gameObject.name = roomInfo.Name;
        joinButton.onClick.AddListener(() => PhotonNetwork.JoinRoom(roomInfo.Name));
        joinButton.GetComponentInChildren<Text>().text = roomInfo.Name;
    }

    private void BackButtonClick()
    {
        PhotonNetwork.LeaveLobby();
    }

    private void Reset()
    {
        roomListRect = GameObject.Find("RoomListRect").GetComponent<RectTransform>();
        backButton = GameObject.Find("Back BT").GetComponent<Button>();

    }
}
