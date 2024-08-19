using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogInPanel : MonoBehaviour
{
    public InputField idInput;
    public Button logInButton;

    private void Awake()
    {
        logInButton.onClick.AddListener(OnLogInButtonClick);
    }


    private void Start()
    {
        idInput.text = $"Player {Random.Range(100, 1000)}";

    }
    public void OnLogInButtonClick()
    {
        PhotonNetwork.LocalPlayer.NickName = idInput.text;
        PhotonNetwork.ConnectUsingSettings();

        // 버튼과 인풋필드 비활성화 시키고 로그인 메세지 또는 아이콘 출력
        idInput.interactable = false;
        logInButton.interactable = false;
    }
    public void OnEnable()
    {
        idInput.interactable = true;
        logInButton.interactable = true;
    }
}
