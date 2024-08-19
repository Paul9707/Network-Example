using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviourPunCallbacks
{
 
    public static PanelManager Instance { get; private set; }

    public LogInPanel logIn;
    public MenuPanel menu;
    public LobbyPanel lobby;
    public RoomPanel room;

    private Dictionary<string, GameObject> panelTable;
    #region Unity Message
    private void Awake()
    {
        Instance = this;
        panelTable = new Dictionary<string, GameObject>
        {
            { "Login", logIn.gameObject },
            { "Menu", menu.gameObject },
            { "lobby", lobby.gameObject },
            { "Room", room.gameObject }
        };
        PhotonNetwork.AddCallbackTarget(this);
        PanelOpen("Login");
    }
    public override void OnEnable() 
    {
        //base.OnEnable();
    }
    public override void OnDisable() 
    {
        //base.OnDisable();
    }
    #endregion

    public void PanelOpen(string panelName)
    {
        foreach (var row in panelTable)
        {
            /*if (row.Key == panelName)
            {
                // 패널이름과 파라미터가 같은면 활성화
                row.Value.SetActive(true);
            }
            else
            {
                // 같지않으면 비활성화
                row.Value.SetActive(false);
            }
            */
            row.Value.SetActive(row.Key.Equals(panelName));
        }
    }

    public override void OnConnected()
    {
        PanelOpen("Menu");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        LogManager.Log($"disconnected cause : {cause}");
        PanelOpen("Login");

    }
    public override void OnJoinedLobby()
    {
        PanelOpen("Lobby");
    }

    public override void OnLeftLobby()
    {
        PanelOpen("Menu");
    }

    public override void OnJoinedRoom()
    {
        PanelOpen("Room");
    }

    public override void OnCreatedRoom()
    {
        PanelOpen("Room");
    }

    public override void OnLeftRoom()
    {
        PanelOpen("Menu");
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
       room.JoinPlayer(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        room.LeavePlayer(otherPlayer);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        lobby.UpdateRoomList(roomList);
    }
}
