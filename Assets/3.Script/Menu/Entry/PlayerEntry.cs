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
        //readyToggle.isOn = false; => onValueChanged가 호출
        readyToggle.SetIsOnWithoutNotify(false); // =>isOn을 변경해도 onValueChanged가 호출되지 않음


    }
 
    private void ReadyToggleClick(bool isOn)
    {
        // 커스텀 프로퍼티에 isOn을 추가하는 로직을 작성했을 경우
    }*/
}
    
