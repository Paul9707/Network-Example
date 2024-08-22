using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogInPanel : MonoBehaviour
{
    public InputField idInput;
    public InputField pwInput;

    public Button createButton;
    public Button logInButton;
    public Button logOutButton;

    private void Awake()
    {
        // logInButton.onClick.AddListener(PhotonConnectButtonClick);
        createButton.onClick.AddListener(CreateButtonClick);
        logInButton.onClick.AddListener(LoginButtonClick);
        logOutButton.onClick.AddListener(LogoutButtonClick);
    }


    private IEnumerator Start()
    {
        idInput.interactable = false;
        pwInput.interactable = false;
        createButton.interactable = false;
        logInButton.interactable = false;

        yield return new WaitUntil(() => FireBaseManager.Instance.IsInitialized);

        idInput.interactable = true;
        pwInput.interactable = true;
        createButton.interactable = true;
        logInButton.interactable = true;
    }

    public void CreateButtonClick()
    {
        createButton.interactable = false;
        FireBaseManager.Instance.Create(idInput.text, pwInput.text, (user) =>
        {
            print("회원가입 성공");
            createButton.interactable = true;
        });
    }

    public void LoginButtonClick()
    {
        logInButton.interactable = false;
        FireBaseManager.Instance.Login(idInput.text, pwInput.text, (user) => { 
            logInButton.interactable = true;
        });
    }

    public void LogoutButtonClick()
    {
        FireBaseManager.Instance.Logout();
    }

    public void PhotonConnectButtonClick()
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
