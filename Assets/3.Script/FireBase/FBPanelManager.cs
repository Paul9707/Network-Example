using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBPanelManager : MonoBehaviour
{
    public enum PanelType
    {
        Login,
        Create,
        Info,
        Update
    }

    #region public
    public FBLoginPanel login;
    public FBCreatPanel creat;
    public FBUserInfoPanel userInfo;
    public FBUserInfoUpdatePanel update;

    public FBDialog dialog;
    #endregion

    #region private
    private Dictionary<PanelType, MonoBehaviour> panels;
    #endregion
    public static FBPanelManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        panels = new()
        {
            { PanelType.Login, login },
            { PanelType.Create, creat },
            { PanelType.Info, userInfo },
            { PanelType.Update, update }
        };
       
    }

    private void Start()
    {
        _ = PanelOpen(PanelType.Login);
        dialog.gameObject.SetActive(false);
    }
    public void Dialog(string message)
    {
        dialog.text.text = message;
        dialog.gameObject.SetActive(true);
        //메세지 전달
    }

    public GameObject PanelOpen(PanelType type)
    {
        GameObject returnPanel = null;

        foreach (var row in panels)
        {
            bool isMatch = type == row.Key; // 딕셔너리에서 파라미터와 키가 일치하는지 여부
            if (isMatch)
            {
                returnPanel = row.Value.gameObject;    // 일치하면 return을 함.
            }
            row.Value.gameObject.SetActive(isMatch); // 일치하는 패널만 활성화
        }
        return returnPanel;
    }

    public T PanelOpen<T>() where T : MonoBehaviour
    {
        T returnPanel = null; 
        foreach (var row in panels)
        {
            bool isMatch = typeof(T) == row.Value.GetType(); // 딕셔너리에서 파라미터와 키가 일치하는지 여부
            if (isMatch)
            {
                returnPanel = (T)row.Value;
            }
            row.Value.gameObject.SetActive(isMatch);
        }

        return returnPanel;
    }
}
