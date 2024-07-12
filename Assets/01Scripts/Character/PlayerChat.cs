using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerChat : MonoBehaviourPunCallbacks
{
    [Header("NormalChat")]
    public GameObject networkManager;

    public GameObject ChatInput;
    private TMP_InputField inputField;

    private Image chatImage;

    [Header("TradeChat")]
    public GameObject tradeChatManager;

    public GameObject tradeChatInput;
    private TMP_InputField tradeInputField;

    private PhotonView view;
    void Start()
    {
        networkManager = GameObject.Find("NetworkManager");
        tradeChatManager = GameObject.Find("Canvas").transform.Find("TradePanel").transform.Find("TradeChatPanel").gameObject;

        ChatInput = GameObject.Find("Canvas").transform.Find("ChatPanel").transform.Find("ChatInputView").gameObject;
        tradeChatInput = GameObject.Find("Canvas").transform.Find("TradePanel").transform.Find("TradeChatPanel").transform.Find("TradeChatInput").gameObject;

        chatImage = GameObject.Find("Canvas").transform.Find("ChatPanel").gameObject.GetComponent<Image>();
        chatImage.color = new Color(chatImage.color.r, chatImage.color.g, chatImage.color.b, 0.75f);

        view = this.GetComponent<PhotonView>();
 
        inputField = ChatInput.GetComponent<TMP_InputField>();
        tradeInputField = tradeChatInput.GetComponent<TMP_InputField>();

        ChatInput.SetActive(false);
    }

    void Update()
    {
        if (view.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Return) && !GameManager.isTradeChatting)
            {
                if (ChatInput.activeSelf == false)
                {
                    ChatInput.SetActive(true);
                    chatImage.color = new Color(chatImage.color.r, chatImage.color.g, chatImage.color.b, 1f);
                    inputField.Select();
                    inputField.ActivateInputField();         
                    GameManager.isChatting = true;
                }
                else
                {
                    networkManager.GetComponent<NetworkManager>().Send();
                    inputField.DeactivateInputField();
                    ChatInput.SetActive(false);
                    chatImage.color = new Color(chatImage.color.r, chatImage.color.g, chatImage.color.b, 0.75f);
                    GameManager.isChatting = false;
                }
            }
            else if(Input.GetKeyDown(KeyCode.Return) && GameManager.isTradeChatting)
            {
                if (!GameManager.isTradeChatInput)
                {
                    tradeInputField.Select();
                    tradeInputField.ActivateInputField();
                    GameManager.isTradeChatInput = true;
                }
                else if (GameManager.isTradeChatInput)
                {
                    tradeChatManager.GetComponent<TradeChatManager>().Send();
                    tradeInputField.DeactivateInputField();
                    GameManager.isTradeChatInput = false;
                }

            }
        }
      
    }
}
