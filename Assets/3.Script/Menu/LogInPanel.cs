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

        // ��ư�� ��ǲ�ʵ� ��Ȱ��ȭ ��Ű�� �α��� �޼��� �Ǵ� ������ ���
        idInput.interactable = false;
        logInButton.interactable = false;
    }
    public void OnEnable()
    {
        idInput.interactable = true;
        logInButton.interactable = true;
    }
}
