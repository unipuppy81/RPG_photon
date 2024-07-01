using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpwanManagerTown : MonoBehaviourPunCallbacks
{
    Transform playerSpawnPos;
    void Start()
    {
        playerSpawnPos = GameObject.Find("PlayerSpawnPos").GetComponent<Transform>();

        PhotonNetwork.Instantiate("Warrior", playerSpawnPos.transform.position, Quaternion.identity);
    }
}
