using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerChat : MonoBehaviourPunCallbacks
{
    public GameObject ChatInput;
    private GameObject manager;
    private TMP_InputField t;
    private PhotonView view;
    void Start()
    {
        manager = GameObject.Find("NetworkManager");
        view = GetComponent<PhotonView>();
        ChatInput = GameObject.Find("Canvas").transform.Find("ChatPanel").transform.Find("ChatInputView").gameObject;
        t = ChatInput.GetComponent<TMP_InputField>();
        ChatInput.SetActive(false);
    }

    void Update()
    {
        if (view.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (ChatInput.activeSelf == false)
                {
                    ChatInput.SetActive(true);
                    t.Select();
                    t.ActivateInputField();
                    GameManager.isChatting = true;
                }
                else
                {
                    manager.GetComponent<NetworkManager>().Send();
                    t.DeactivateInputField();
                    ChatInput.SetActive(false);
                    GameManager.isChatting = false;
                }
            }
        }
      
    }
}
