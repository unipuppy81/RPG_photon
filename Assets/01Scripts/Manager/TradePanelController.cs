using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class TradePanelController : MonoBehaviourPunCallbacks
{
    public PhotonView tradePanelPhotonView;

    public static TradePanelController Instance;

    public TextMeshProUGUI initNameText;
    public TextMeshProUGUI clickedNameText;

    public TextMeshProUGUI initReadyText;
    public TextMeshProUGUI clickedReadyText;

    public GameObject tradeButtonObj;
    public GameObject nonTradeButtonObj;
    public GameObject tradeExitButtonObj;

    public GameObject slotPrefab;

    public GameObject TradeResultSuccessPanel;
    public GameObject TradeResultFailPanel;

    private Button tradeButton;         // 등록하기 버튼
    private Button nonTradeButton;      // 등록취소 버튼
    private Button tradeExitButton;     // 거래 그만하기 버튼

    public Player initPlayer;
    public Player clickedPlayer;

    public List<string> itemNameList;
    public List<int> itemCountList;

    private bool initOffered = false; // 내가 등록 버튼을 눌렀는지 여부
    private bool clickedOffered = false; // 상대방이 등록 버튼을 눌렀는지 여부

    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        tradeButton = tradeButtonObj.GetComponent<Button>();
        nonTradeButton = nonTradeButtonObj.GetComponent<Button>();
        tradeExitButton = tradeExitButtonObj.GetComponent<Button>();

        tradePanelPhotonView = GetComponent<PhotonView>();
    }

    // 등록하기 패널 초기화
    public void Initialize(Player _initiator, Player _clickedPlayer)
    {
        initPlayer = _initiator;
        clickedPlayer = _clickedPlayer;

        initNameText.text = initPlayer.NickName;
        clickedNameText.text = clickedPlayer.NickName;

        tradeButtonObj.SetActive(true);
        nonTradeButtonObj.SetActive(false);

        initOffered = false;
        clickedOffered = false;

        initReadyText.text = "준비중";
        clickedReadyText.text = "준비중";
    }

    private void OnEnable()
    {
        GameManager.isTradeChatting = true;
    }

    private void OnDisable()
    {
        GameManager.isTradeChatting = false;
    }

    public void UpdateSlot(string _itemName, int _itemCount, Vector3 _thisPosition, Quaternion _thisRotation, Vector3 _thisLocalScale , Vector3 _position, Vector2 _sizeDelta, byte[] _itemSpriteBytes)
    {
        if(PhotonNetwork.LocalPlayer == initPlayer)
        {
            tradePanelPhotonView.RPC("UpdateTradeSlot", clickedPlayer, _itemName, _itemCount, _thisPosition, _thisRotation, _thisLocalScale, _position, _sizeDelta, _itemSpriteBytes);
        }
        else if (PhotonNetwork.LocalPlayer == clickedPlayer)
        {
            tradePanelPhotonView.RPC("UpdateTradeSlot", initPlayer, _itemName, _itemCount, _thisPosition, _thisRotation, _thisLocalScale, _position, _sizeDelta, _itemSpriteBytes);
        }
    }

    // 아이템 옮길때 동기화
    [PunRPC]
    public void UpdateTradeSlot(string _itemName, int _itemCount, Vector3 _thisPosition, Quaternion _thisRotation, Vector3 _thisLocalScale, Vector3 _position, Vector2 _sizeDelta, byte[] _itemSpriteBytes)
    {
        //  RPC를 통해 받은 정보로 슬롯 업데이트 수행
        // 거래 패널의 슬롯을 찾아서 업데이트합니다.
        GameObject newSlot = Instantiate(slotPrefab, transform);
        newSlot.transform.localPosition = _position;

        TradeManager.Instance.resetGameObject.Add(newSlot);

        // DraggableUI 컴포넌트 제거
        DraggableUI draggableUI = newSlot.GetComponent<DraggableUI>();
        if (draggableUI != null)
        {
            Destroy(draggableUI);
        }

        TextMeshProUGUI tmpCount = newSlot.GetComponentInChildren<TextMeshProUGUI>();

        RectTransform rectTransform = newSlot.GetComponent<RectTransform>();
        rectTransform.position = _thisPosition;
        rectTransform.rotation = _thisRotation;
        rectTransform.localScale = _thisLocalScale;

        rectTransform.sizeDelta = _sizeDelta;



        Slot slot = newSlot.GetComponent<Slot>();
        slot.itemName = _itemName;
        slot.itemCount = _itemCount;

        // ItemImage 오브젝트에서 Image 컴포넌트 가져오기
        Image slotImage = newSlot.GetComponent<Image>();


        // Texture2D로 변환
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(_itemSpriteBytes); // byte 배열에서 이미지 로드


        // Texture2D를 Sprite로 변환
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        slotImage.sprite = sprite;

        TextMeshProUGUI tmp = newSlot.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = _itemCount.ToString();


        CanvasGroup canvasGroup = newSlot.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;
    }

    // 등록 버튼 클릭 시, 서로의 bool => 서버에서 처리
    public void OnTradeButtonClicked()
    {
        if (tradeButtonObj.activeSelf)
        {
            tradeButtonObj.SetActive(false);
            nonTradeButtonObj.SetActive(true);
        }

        if (PhotonNetwork.LocalPlayer == initPlayer)
        {
            initOffered = true;

            initReadyText.text = "준비됨";

            tradePanelPhotonView.RPC("CheckTradeStatus", clickedPlayer, initOffered);
        }
        else if (PhotonNetwork.LocalPlayer == clickedPlayer)
        {
            clickedOffered = true;

            clickedReadyText.text = "준비됨"; 


            tradePanelPhotonView.RPC("CheckTradeStatus", initPlayer, clickedOffered);
        }
    }

    // 등록 취소 버튼 클릭 시
    public void OnTradeButtonNonClicked()
    {
        if (nonTradeButtonObj.activeSelf)
        {
            tradeButtonObj.SetActive(true);
            nonTradeButtonObj.SetActive(false);
        }

        if (PhotonNetwork.LocalPlayer == initPlayer)
        {
            initOffered = false;

            initReadyText.text = "준비중";

            tradePanelPhotonView.RPC("CheckTradeStatus", clickedPlayer, initOffered);
        }
        else if (PhotonNetwork.LocalPlayer == clickedPlayer)
        {
            clickedOffered = false;

            clickedReadyText.text = "준비중";

            tradePanelPhotonView.RPC("CheckTradeStatus", initPlayer, clickedOffered);
        }
    }


    // 거래 그만하기 버튼 클릭 시
    public void OnTradeExitButtonClicked()
    {
        if (PhotonNetwork.LocalPlayer == initPlayer)
        {
            tradePanelPhotonView.RPC("TradeFail", clickedPlayer);
        }
        else if (PhotonNetwork.LocalPlayer == clickedPlayer)
        {
            tradePanelPhotonView.RPC("TradeFail", initPlayer);
        }

        foreach (GameObject obj in TradeManager.Instance.resetGameObject)
        {
            Destroy(obj);
        }

        gameObject.SetActive(false); // 패널 숨기기
    }

    // 거래 상태 확인 및 처리
    [PunRPC]
    public void CheckTradeStatus(bool sendBool)
    {
        if(PhotonNetwork.LocalPlayer == initPlayer)
        {
            clickedOffered = sendBool;

            boolText();
        }

        if(PhotonNetwork.LocalPlayer == clickedPlayer)
        {
            initOffered = sendBool;

            boolText();
        }

        if (initOffered && clickedOffered)
        {
            AcceptTrade();

            if (PhotonNetwork.LocalPlayer == initPlayer)
            {
                tradePanelPhotonView.RPC("AcceptTrade", clickedPlayer);
                tradePanelPhotonView.RPC("TradeSuccess", clickedPlayer);
            }
            else if (PhotonNetwork.LocalPlayer == clickedPlayer)
            {
                tradePanelPhotonView.RPC("AcceptTrade", initPlayer);
                tradePanelPhotonView.RPC("TradeSuccess", initPlayer);
            }

            TradeResultSuccessPanel.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    // 텍스트 설정
    private void boolText()
    {

        if (initOffered)
        {
            initReadyText.text = "준비됨";
        }
        else if (!initOffered)
        {
            initReadyText.text = "준비중";
        }

        if (clickedOffered)
        {
            clickedReadyText.text = "준비됨";
        }
        else if (!clickedOffered)
        {
            clickedReadyText.text = "준비중";
        }
    }

    // 거래 성공 처리
    [PunRPC]
    private void AcceptTrade()
    {
        for (int i = 0; i < itemNameList.Count; i++)
        {
            string itemName = itemNameList[i];
            int itemCount = itemCountList[i];

            ItemDataManager.Instance.TradeRemoveItem(itemName, itemCount);
        }

        if (PhotonNetwork.LocalPlayer == initPlayer)
        {
            for(int i = 0; i < itemNameList.Count; i++)
            {
                string itemName = itemNameList[i];
                int itemCount = itemCountList[i];

                tradePanelPhotonView.RPC("SendItem", clickedPlayer, itemName, itemCount);
            }
        }
        else if (PhotonNetwork.LocalPlayer == clickedPlayer)
        {
            for (int i = 0; i < itemNameList.Count; i++)
            {
                string itemName = itemNameList[i];
                int itemCount = itemCountList[i];

                tradePanelPhotonView.RPC("SendItem", initPlayer, itemName, itemCount);
            }
        }

        foreach (GameObject obj in TradeManager.Instance.resetGameObject)
        {
            Destroy(obj);
        }
    }

    // 거래 완료 처리
    [PunRPC]
    public void SendItem(string s, int i)
    {
        // 아이템을 실제로 전송합니다.
        ItemDataManager.Instance.TradeAddItem(s, i);
    }

    [PunRPC]
    public void TradeFail()
    {
        foreach (GameObject obj in TradeManager.Instance.resetGameObject)
        {
            Destroy(obj);
        }

        this.gameObject.SetActive(false);
        TradeResultFailPanel.SetActive(true);
    }

    [PunRPC]
    public void TradeSuccess()
    {
        gameObject.SetActive(false);

        TradeResultSuccessPanel.SetActive(true);
    }
}
