using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static bool isPlayGame = false;
    public static bool isChatting = false;
    public static bool isTradeChatting = false;
    public static bool isTradeChatInput = false;
}
