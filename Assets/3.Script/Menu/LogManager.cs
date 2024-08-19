using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance { get; private set; }

    public RectTransform logContent;
    public Text logText;

    private void Awake()
    {
        Instance = this;

    }

    public static void Log(string message)
    {
        if (Instance != null)
        {
            Text logText = Instantiate(Instance.logText, Instance.logContent, false); // 정적 함수이기 떄문에 Instanc를 통해 접근한다. -> false는 자신의 위치를 지우고 부모의 위치에 생성하겠다.
            logText.text = message; 
        }
        else
        {
            print(message);
        }
    }
}
