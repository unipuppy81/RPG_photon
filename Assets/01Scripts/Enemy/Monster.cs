using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Monster : MonoBehaviourPunCallbacks
{
    private enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Die
    }

    private EnemyState _enemyState;

    private void Start()
    {
        _enemyState = EnemyState.Idle;
    }

    private void Update()
    {
        switch (_enemyState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Move:
                break;

            case EnemyState.Attack:
                break;

            case EnemyState.Die:
                break;
        }
    }
}
