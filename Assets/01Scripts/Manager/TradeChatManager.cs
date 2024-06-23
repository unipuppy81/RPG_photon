using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class TradeChatManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField ChatInput;
    public PhotonView tradeChatPV;

    public TextMeshProUGUI[] chatList;


    private void OnEnable()
    {
        ChatInput.text = "";
    }

    // 채팅 전송 함수
    public void Send()
    {
        if (ChatInput.text.Equals(""))
            return;
        string msg = "[" + PhotonNetwork.NickName + "] " + ChatInput.text;
        tradeChatPV.RPC("TradeChatRPC", RpcTarget.All, msg);
        ChatInput.text = "";
    }

    [PunRPC]
    void TradeChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < chatList.Length; i++)
            if (chatList[i].text == "")
            {
                isInput = true;
                chatList[i].text = msg;
                break;
            }
        if (!isInput)
        {
            for (int i = 1; i < chatList.Length; i++) chatList[i - 1].text = chatList[i].text;
            chatList[chatList.Length - 1].text = msg;
        }
    }
}
