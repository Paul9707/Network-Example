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
    //������ ���, �÷��̾���� ready ���¸� ������ dictionary

    private Dictionary<int, bool> playersReady;
    // ��� �÷��̾���� ���θ� �˰� �ֵ��� ����� dictionary
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
            //������ �����϶� �ʱ�ȭ
            playersReady = new Dictionary<int, bool>();
        }
        else
        {
            //������ �ƴҶ�
            diffText.text = ((Difficulty)diffDropdown.value).ToString();
        }
        
        // ������ �ƴϸ� ���� ���� ��ư �� ���̵� ������ ��Ȱ��ȭ
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        diffDropdown.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        diffText.gameObject.SetActive(false == PhotonNetwork.IsMasterClient);
        
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            //�÷��̾� ��Ͽ� �÷��̾� �̸�ǥ �ϳ��� ����
            JoinPlayer(player);

            //�濡 �������� Ű�� �ִ��� Ȯ��
            if (player.CustomProperties.ContainsKey("Ready"))
            {
                SetPlayerReady(player.ActorNumber, (bool)player.CustomProperties["Ready"]);
            }
        }
        // �濡 ���� ���� ��, ������ �� �ε� ���ο� ���� �Բ� �� �ε�
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
            //TODO: �� ��Ʈ���� ��쿡�� ����� onValueChanged �̺�Ʈ�� �ڵ鸵.
            toggle.onValueChanged.AddListener(ReadyToggleClick);
           
            for(int i = 0; i < eyeToggles.Count; i++)
            {
                eyeToggles[i].onValueChanged.AddListener(EyeToggleClick);
            }
        }
        else { toggle.gameObject.SetActive(false); } // ���� �ƴ� �ٸ� �÷��̾��� ��Ʈ��

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
            //SetSiblingIndex => Hierarchy�� �� �θ�ȿ��� �ٸ� ��ü �� ������ �����ϰ� ���� �� ���

        }
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

    // �� Ready ���°� ����� �� Custum Properties ����
    public void ReadyToggleClick(bool isOn)
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;

        // PhotonNetwork�� custumProperties�� Hashtable ������ Ȱ��
        // �׷��� .Net�� Hashtable�� �ƴ� ����ȭ ������ Hashtable Ŭ������ ���� ����
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
    //�ٸ� �÷��̾ ReadyToggle�� Ŭ������ ��� �� Ŭ���̾�Ʈ���� �ݿ��ؾ� �Ѵ�.
    public void SetPlayerReady(int actorNumber, bool isReady)
    {
        playerEntries[actorNumber].readyLabel.gameObject.SetActive(isReady);
        if (PhotonNetwork.IsMasterClient)
        {
            playersReady[actorNumber] = isReady;
            CheckReady();
        }
    }

    // ������ ��� �ٸ� �÷��̾���� ��� ready �������� Ȯ�� �Ͽ� 
    // Start ��ư�� Ȱ��ȭ ���θ� ����
    private void CheckReady()
    {
        // ���� ����� �Ѱ��� false�̸� false���� �Ҷ�
        // �� ��� ��Ұ� && ������ �ؾ� �� ��

        bool allReady = playersReady.Values.All(x => x);
        // bool anyReady = playersReady.Values.Any(x => x);
        /*bool allReady = true; // �ʱ���� true

        // 5���� �÷��̾��� 3��° �÷��̾��� isReady �� false, ������ true =>

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
        //print($"Ŀ���� ������Ƽ ����ƽ��ϴ�. : {PhotonNetwork.Time}");
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
        // ������ �������� ȣ�� �ǹǷ�, �� �����Ǿ� �ִ� ���¿��� ������ ������ ���� �� �� �ֵ���
        // ��ȿ�� �˻� �� �߰� ��ġ�� �� �ʿ䰡 ����

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
