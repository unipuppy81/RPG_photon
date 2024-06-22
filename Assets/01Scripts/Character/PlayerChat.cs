using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChat : MonoBehaviour
{
    private GameObject ChatInput;
    private GameObject manager;
    void Start()
    {
        manager = GameObject.Find("NetworkManager");
        ChatInput = GameObject.Find("Canvas").transform.Find("ChatPanel").transform.Find("ChatInputView").gameObject;
        ChatInput.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Space");
            if (ChatInput.activeSelf == false)
                ChatInput.SetActive(true);
            else
            {
                manager.GetComponent<NetworkManager>().Send();
                ChatInput.SetActive(false);
            }
        }
    }
}
