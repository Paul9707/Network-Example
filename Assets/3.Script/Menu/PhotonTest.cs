using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonTest : MonoBehaviour
{
    private ClientState photonState = 0; // Cash 용도로 활용하기 때문에 private으로 막아둔다.

    private void Update()
    {
        if (PhotonNetwork.NetworkClientState != photonState)
        {
            LogManager.Log($"State Changed : {PhotonNetwork.NetworkClientState}");
            photonState = PhotonNetwork.NetworkClientState;
        }
    }
}


/*
    Photon : 릴레이 서버
            ㄴ 마스터 클라이언트가 나가면 랜덤으로 마스터 클라이언트 부여
            ㄴ
 
 
 */
