using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Drawing.Imaging;
using Unity.VisualScripting.Antlr3.Runtime;

public class TradeManager : MonoBehaviourPunCallbacks
{
    public static TradeManager Instance;
    public GameObject yesornoPanel;
    public GameObject tradePanel;
    public TextMeshProUGUI nameText;
    public Button acceptButton;
    public Button rejectButton;

    private Player tradeInitiator; // 거래를 시작한 플레이어
    private Player clickedPlayer; // 거래를 제안 받은 플레이어

    public PhotonView pv;

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

    void Start()
    {
        pv = GetComponent<PhotonView>();
        acceptButton.onClick.AddListener(OnAcceptTrade);
        rejectButton.onClick.AddListener(OnRejectTrade);
    }

    // 거래 요청 받기
    [PunRPC]
    public void ReceiveTradeRequest(Player initPlayer, Player _clickedPlayer)
    {
        tradeInitiator = initPlayer; // 거래를 시작한 플레이어 설정
        clickedPlayer = _clickedPlayer; // 거래를 당하는 플레이어 설정

        // 상대방의 화면에 거래 요청 UI 표시
        if (PhotonNetwork.LocalPlayer == clickedPlayer)
        {
            ShowTradeRequest();
        }
    }

    // 거래 요청 UI 표시
    public void ShowTradeRequest()
    {
        yesornoPanel.SetActive(true);
        nameText.text = tradeInitiator.NickName + " 님이 거래를 요청했습니다.";
    }

    // 거래 요청 수락
    public void OnAcceptTrade()
    {
        yesornoPanel.SetActive(false);

        if(PhotonNetwork.LocalPlayer == clickedPlayer)
        {
            TradePanelActive(tradeInitiator, clickedPlayer);             // 거래 패널 열기
        }

        // 네트워크를 통해 상대방에게도 UI를 표시하도록 RPC 호출
        pv.RPC("TradePanelActive", tradeInitiator, tradeInitiator, clickedPlayer);
    }

    [PunRPC]
    public void TradePanelActive(Player initPlayer, Player _clickedPlayer)
    {
        tradePanel.SetActive(true);
        TradePanelController.Instance.Initialize(initPlayer, _clickedPlayer);
    }

    // 거래 요청 거부
    public void OnRejectTrade()
    {
        RejectTradeRequest();
        yesornoPanel.SetActive(false);
    }

    // 거래 요청 거부 처리
    public void RejectTradeRequest()
    {
        Debug.Log("Trade request rejected");
    }

    // 거래 완료 처리
    [PunRPC]
    public void CompleteTrade()
    {
        Debug.Log("Trade completed");
        // 여기서 아이템을 실제로 전송합니다.
    }


}
