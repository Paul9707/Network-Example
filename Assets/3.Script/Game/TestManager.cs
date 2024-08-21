using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public Transform startPostions;

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            StartCoroutine(NormalStart());
        }
        else 
        {
            // ������ �� ���� �� ���� ������ �ǳ� �پ����Ƿ�, �ڵ����� ����׷뿡 �����Ŵ
            StartCoroutine(DebugStart());
        }

    }

    private IEnumerator NormalStart()
    {
        // PhotonNetwork�� ��� �÷��̾��� �ε� ���¸� �Ǵ��Ͽ� �ѹ����� �ؾ� �ϴµ�
        // ���� �׷� ����� �����Ǿ����� �����Ƿ�, 1�� ��� �� ���� ���� ���� ����
        yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.GetPlayerNumber() != -1);
        //GameObject playerPrefab = Resources.Load<GameObject>("Players/Player");
        //Instantiate(playerPrefab, startPostions.GetChild(0).position, Quaternion.identity);

        // ������ ���� ��� -> ������ ��õ���� ����, ���� ���� �� �ް����� �ʾ��� ��쿡�� �����̵��� �ʴ´�.
        // ���ӿ� ������ �濡�� �ο��� �� ��ȣ.
        // Ȱ���ϱ� ���ؼ��� ���� ���� PlayerNumbering ������Ʈ�� �����ؾ���.
        int playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();

        Transform playerPos = startPostions.GetChild(playerNumber).transform;
        GameObject playerObj = PhotonNetwork.Instantiate("Players/Player", playerPos.position, playerPos.rotation);
        playerObj.GetComponent<PlayerController>().eyes[(int)PhotonNetwork.LocalPlayer.CustomProperties["EyeToggleValue"]].SetActive(true);
        /*switch (PhotonNetwork.LocalPlayer.CustomProperties["EyeToggleValue"])
        {
            case 0:
                *//*playerObj.GetComponent<PlayerController>().eyes[(int)PhotonNetwork.LocalPlayer.CustomProperties["EyeToggleValue"]].SetActive(true);*//*
                break;
            case 1:
                playerObj.GetComponent<PlayerController>().eyes[1].SetActive(true);
                break;
            case 2:
                playerObj.GetComponent<PlayerController>().eyes[2].SetActive(true);
                break;
            
        }*/
        playerPos.name = $"Player {playerNumber}";
    }

    public static bool debugReady = false;
    private IEnumerator DebugStart()
    {
        // ����� ������ start ����
        gameObject.AddComponent<PhotonDebuger>();
        yield return new WaitUntil(() => debugReady);
        StartCoroutine(NormalStart());
        
    }
}
