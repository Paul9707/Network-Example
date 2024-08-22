using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FBLoginPanel : MonoBehaviour
{
    #region Public Fields
    public InputField idInput;
    public InputField pwInput;

    public Button createButton;
    public Button loginButton;
    #endregion

    #region Private Fields
    #endregion

    private void Awake()
    {
        createButton.onClick.AddListener(CreateButtonClick);
        loginButton.onClick.AddListener(LoginButtonClick);
    }

    private void Start()
    {
        SetUIInteractable(FireBaseManager.Instance.IsInitialized);
        FireBaseManager.Instance.onInit += () => SetUIInteractable(true);
    }

    public void LoginButtonClick()
    {
        SetUIInteractable(false);
        FireBaseManager.Instance.Login(idInput.text, pwInput.text,
            (user) =>
            {
                FBPanelManager.Instance.PanelOpen<FBUserInfoPanel>().SetUserInfo(user);
            }
            );
    }

    public void CreateButtonClick()
    {
        _ = FBPanelManager.Instance.PanelOpen<FBCreatPanel>();
    }

    /// <summary>
    /// UI 활성화 비활성화를 설정하는 함수
    /// </summary>
    /// <param name="isTrue"></param>
    public void SetUIInteractable(bool isTrue)
    {
        idInput.interactable = isTrue;
        pwInput.interactable = isTrue;
        createButton.interactable = isTrue;
        loginButton.interactable = isTrue;
    }
}
