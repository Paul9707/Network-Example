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
        //�޼��� ����
    }

    public GameObject PanelOpen(PanelType type)
    {
        GameObject returnPanel = null;

        foreach (var row in panels)
        {
            bool isMatch = type == row.Key; // ��ųʸ����� �Ķ���Ϳ� Ű�� ��ġ�ϴ��� ����
            if (isMatch)
            {
                returnPanel = row.Value.gameObject;    // ��ġ�ϸ� return�� ��.
            }
            row.Value.gameObject.SetActive(isMatch); // ��ġ�ϴ� �гθ� Ȱ��ȭ
        }
        return returnPanel;
    }

    public T PanelOpen<T>() where T : MonoBehaviour
    {
        T returnPanel = null; 
        foreach (var row in panels)
        {
            bool isMatch = typeof(T) == row.Value.GetType(); // ��ųʸ����� �Ķ���Ϳ� Ű�� ��ġ�ϴ��� ����
            if (isMatch)
            {
                returnPanel = (T)row.Value;
            }
            row.Value.gameObject.SetActive(isMatch);
        }

        return returnPanel;
    }
}
