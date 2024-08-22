using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FBUserInfoUpdatePanel : MonoBehaviour
{
    #region public Fields
    public InputField displayName;
    public InputField pwInput;
    public Text emailText;
    public Text uidText;

    public Button updateButton;
    public Button cancelButton;
    #endregion

    private void Awake()
    {
        updateButton.onClick.AddListener(UpdateButtonClick);
        cancelButton.onClick.AddListener(CancelButtonClick);
    }
    public void SetUserInfo(FirebaseUser user)
    {
        displayName.text = user.DisplayName;
        emailText.text = user.Email;
        uidText.text = user.UserId;
    }

    public void UpdateButtonClick()
    {
        FireBaseManager.Instance.UpdateUser(displayName.text, pwInput.text,
            () =>
            {
                FBPanelManager.Instance.Dialog("정보가 수정되었습니다.");
                FBPanelManager.Instance.PanelOpen<FBUserInfoPanel>().SetUserInfo(FireBaseManager.Instance.Auth.CurrentUser);
            }
            );
    }

    public void CancelButtonClick()
    {
        _ = FBPanelManager.Instance.PanelOpen<FBUserInfoPanel>();
    }
}
