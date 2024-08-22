using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FBUserInfoPanel : MonoBehaviour
{
    #region public Fields
    public Text greetingText;
    public Text displayNameText;
    public Text EmailText;
    public Text phoneNumText;
    public Text uid;

    public Button logoutButton;
    public Button updateButton;
    public Button gameStartButton;
    #endregion

    private void Awake()
    {
        logoutButton.onClick.AddListener(LogoutButtonClick);
        updateButton.onClick.AddListener(UpdateButtonClick);
        gameStartButton.onClick.AddListener(GameStartButtonClick);
    }

    /// <summary>
    /// ���� ������ ������ UI�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="user"></param>
    public void SetUserInfo(FirebaseUser user)
    {
        greetingText.text = $"�ȳ��ϼ���, {user.DisplayName}��";
        displayNameText.text = user.DisplayName;
        EmailText.text = user.Email;
        phoneNumText.text = user.PhoneNumber;
        uid.text = user.UserId;
    }

    public void LogoutButtonClick()
    {
        FireBaseManager.Instance.Logout();
        FBPanelManager.Instance.PanelOpen<FBLoginPanel>().SetUIInteractable(true);
    }
    public void UpdateButtonClick()
    {
        FBPanelManager.Instance.PanelOpen<FBUserInfoUpdatePanel>().SetUserInfo(FireBaseManager.Instance.Auth.CurrentUser);
    }
    public void GameStartButtonClick()
    {
        SceneManager.LoadScene("GameScene");
    }
}
