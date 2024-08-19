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
        List<RoomInfo> destroryCandidate /* 파괴될 후보군 */ =
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
        foreach (Transform child in roomListRect) // Transform을 var로 선언할 경우 오류가 난다 이유는 object로 박싱이 되어서 반환하기 때문에 언박싱을 직접 해줘야 한다.
        {
            Destroy(child.gameObject);
        }
    }

    public void AddRoomButton(RoomInfo roomInfo) // RoomInfo List를 통해 순차적으로 한개씩 방 입장 버튼을 생성
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
