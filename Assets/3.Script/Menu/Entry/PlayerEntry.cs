using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntry : MonoBehaviour
{
    #region public
    public Text playerNameText;
    public Toggle readyToggle;
    public GameObject readyLabel;
    public List<Toggle> eyeToggles;

    public Player player;
    public bool IsMine => player == PhotonNetwork.LocalPlayer;
    
    #endregion

  /*  private void Awake()
    {
        readyToggle.onValueChanged.AddListener(ReadyToggleClick);
        //readyToggle.isOn = false; => onValueChanged�� ȣ��
        readyToggle.SetIsOnWithoutNotify(false); // =>isOn�� �����ص� onValueChanged�� ȣ����� ����


    }
 
    private void ReadyToggleClick(bool isOn)
    {
        // Ŀ���� ������Ƽ�� isOn�� �߰��ϴ� ������ �ۼ����� ���
    }*/
}
    
