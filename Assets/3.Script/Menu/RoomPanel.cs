using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

public enum PlayerEyes
{
    Eye1,
    Eye2,
    Eye3
}

public class RoomPanel : MonoBehaviourPunCallbacks
{
    public Text roomTitleText;
    public RectTransform playerList;
    public GameObject playerTextPrefab;

    public Button startButton;
    public Button cancelButton;
    public Dropdown diffDropdown;
    public Text diffText;
    //방장일 경우, 플레이어들의 ready 상태를 저장할 dictionary

    private Dictionary<int, bool> playersReady;
    // 모든 플레이어들이 서로를 알고 있도록 사용할 dictionary
    public Dictionary<int, PlayerEntry> playerEntries = new();

    private void Awake()
    {
        startButton.onClick.AddListener(StartButtonClick);
        cancelButton.onClick.AddListener(CancelButtonClick);
        
        diffDropdown.ClearOptions();
        foreach (object diff in Enum.GetValues(typeof(Difficulty)))
        {
            Dropdown.OptionData option = new Dropdown.OptionData(diff.ToString());
            diffDropdown.options.Add(option);
        }
        
        diffDropdown.onValueChanged.AddListener(DifficultValueChange);

       
        
    }

    public override void OnDisable()
    {
        base.OnDisable();
        foreach (Transform child in playerList)
        {
            Destroy(child.gameObject);
        }

    }

    public override void OnEnable()
    {
        base.OnEnable();
        roomTitleText.text = PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.IsMasterClient)
        {
            //방장인 상태일때 초기화
            playersReady = new Dictionary<int, bool>();
        }
        else
        {
            //방장이 아닐때
            diffText.text = ((Difficulty)diffDropdown.value).ToString();
        }
        
        // 방장이 아니면 게임 시작 버튼 및 난이도 조절을 비활성화
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        diffDropdown.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        diffText.gameObject.SetActive(false == PhotonNetwork.IsMasterClient);
        
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            //플레이어 목록에 플레이어 이름표 하나씩 생성
            JoinPlayer(player);

            //방에 들어왔을때 키가 있는지 확인
            if (player.CustomProperties.ContainsKey("Ready"))
            {
                SetPlayerReady(player.ActorNumber, (bool)player.CustomProperties["Ready"]);
            }
        }
        // 방에 입장 했을 때, 방장의 씬 로드 여부에 따라 함께 씬 로드
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void JoinPlayer(Player newPlayer)
    {
        /*GameObject playerName = Instantiate(playerTextPrefab, playerList, false);
        playerName.name = newPlayer.NickName;
        playerName.GetComponent<Text>().text = newPlayer.NickName;*/

        var playerEntry = Instantiate(playerTextPrefab, playerList, false).GetComponent<PlayerEntry>();
        playerEntry.player = newPlayer;
        playerEntry.playerNameText.text = newPlayer.NickName;

        var toggle = playerEntry.readyToggle;
        var eyeToggles = playerEntry.eyeToggles;
        if (PhotonNetwork.LocalPlayer.ActorNumber == newPlayer.ActorNumber)
        {
            //TODO: 내 엔트리일 경우에만 토글의 onValueChanged 이벤트를 핸들링.
            toggle.onValueChanged.AddListener(ReadyToggleClick);
           
            for(int i = 0; i < eyeToggles.Count; i++)
            {
                eyeToggles[i].onValueChanged.AddListener(EyeToggleClick);
            }
        }
        else { toggle.gameObject.SetActive(false); } // 내가 아닌 다른 플레이어의 엔트리

        playerEntries[newPlayer.ActorNumber] = playerEntry;

        if (PhotonNetwork.IsMasterClient)
        {
            playersReady[newPlayer.ActorNumber] = false;
            CheckReady();

        }
        OnRoomPropertiesUpdate(PhotonNetwork.CurrentRoom.CustomProperties);
        SortPlayers();
    }

    public void LeavePlayer(Player leavePlayer)
    {
        GameObject leaveTarget = playerEntries[leavePlayer.ActorNumber].gameObject;
        playerEntries.Remove(leavePlayer.ActorNumber);
        Destroy(leaveTarget);

        if (PhotonNetwork.IsMasterClient)
        {
            playersReady.Remove(leavePlayer.ActorNumber);
            CheckReady();
        }
        SortPlayers();
    }

    public void SortPlayers()
    {
        foreach (int actorNumber in playerEntries.Keys)
        {
            playerEntries[actorNumber].transform.SetSiblingIndex(actorNumber);
            //SetSiblingIndex => Hierarchy상 내 부모안에서 다른 개체 중 순서를 지정하고 싶을 때 사용

        }
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

    // 내 Ready 상태가 변경될 때 Custum Properties 변경
    public void ReadyToggleClick(bool isOn)
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;

        // PhotonNetwork의 custumProperties는 Hashtable 구조를 활용
        // 그러나 .Net의 Hashtable이 아닌 간소화 형태의 Hashtable 클래스를 직접 제공
        Hashtable custumProps = localPlayer.CustomProperties;

        custumProps["Ready"] = isOn;

        localPlayer.SetCustomProperties(custumProps);
    }

    public void EyeToggleClick(bool isOn)
    {
        if (!isOn)
        {
            return;
        }
        Player localPlayer = PhotonNetwork.LocalPlayer;
        Hashtable customProps = localPlayer.CustomProperties;
        List<Toggle> eyeToggles = playerEntries[localPlayer.ActorNumber].eyeToggles;
        for (int i = 0; i < eyeToggles.Count; i++)
        {
            Toggle toggle = eyeToggles[i];
            if (toggle.isOn)
            {
                customProps["EyeToggleValue"] = i;
                localPlayer.SetCustomProperties(customProps);
                break;
            }
        }
    }
    //다른 플레이어가 ReadyToggle을 클릭했을 경우 네 클라이언트에도 반영해야 한다.
    public void SetPlayerReady(int actorNumber, bool isReady)
    {
        playerEntries[actorNumber].readyLabel.gameObject.SetActive(isReady);
        if (PhotonNetwork.IsMasterClient)
        {
            playersReady[actorNumber] = isReady;
            CheckReady();
        }
    }

    // 방장일 경우 다른 플레이어들이 모두 ready 상태인지 확인 하여 
    // Start 버튼의 활성화 여부를 결정
    private void CheckReady()
    {
        // 여러 요소중 한개라도 false이면 false여야 할때
        // 즉 모든 요소가 && 연산을 해야 할 때

        bool allReady = playersReady.Values.All(x => x);
        // bool anyReady = playersReady.Values.Any(x => x);
        /*bool allReady = true; // 초기상태 true

        // 5명의 플레이어중 3번째 플레이어의 isReady 가 false, 나머진 true =>

        foreach (bool isReady in playersReady.Values)
        {
            if (isReady)
            {
                continue;
            }
            else
            {
                allReady = false;
                break;
            }
        }*/

        startButton.interactable = allReady;
        //if (playersReady.ContainsValue(false))
        //{
        //    allReady = false;
        //}
        //else
        //{
        //    allReady = true;
        //}
    }

    private void DifficultValueChange(int value)
    {
        if (false == PhotonNetwork.IsMasterClient) return;

        var customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        customProps["Diff"] = value;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        //print($"커스텀 프로퍼티 변경됐습니다. : {PhotonNetwork.Time}");
        if (changedProps.ContainsKey("Ready"))
        {
            SetPlayerReady(targetPlayer.ActorNumber, (bool)changedProps["Ready"]);
        }

    }

    public override void OnRoomPropertiesUpdate(Hashtable props)
    {
        if (props.ContainsKey("Diff"))
        {
            print($"room difficulty changed : {props["Diff"]}");
            diffText.text = ((Difficulty)props["Diff"]).ToString();
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // 방장이 나갔을때 호출 되므로, 방 참가되어 있는 상태에서 방장의 역할을 수행 할 수 있도록
        // 유효성 검사 및 추가 조치를 할 필요가 있음

        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {
            startButton.gameObject.SetActive(true);
            diffDropdown.gameObject.SetActive(true);
            diffText.gameObject.SetActive(false);
          
        }
        else
        {
            startButton.gameObject.SetActive(false);
            diffDropdown.gameObject.SetActive(false);
            diffText.gameObject.SetActive(true);
        }
    }
}
