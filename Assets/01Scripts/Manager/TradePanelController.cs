using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class TradePanelController : MonoBehaviourPun
{
    public static TradePanelController Instance;

    public TextMeshProUGUI initNameText;
    public TextMeshProUGUI clickedNameText;

    public Button tradeButton;
    public Button tradeExitButton; // 거래 그만하기 버튼

    private Player initPlayer;
    private Player clickedPlayer;

    private bool initOffered = false; // 내가 거래 버튼을 눌렀는지 여부
    private bool clickedOffered = false; // 상대방이 거래 버튼을 눌렀는지 여부

    private PhotonView tradePhotonView;
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

    // 거래하기 패널 초기화
    public void Initialize(Player _initiator, Player _clickedPlayer)
    {
        initPlayer = _initiator;
        clickedPlayer = _clickedPlayer;

        initNameText.text = initPlayer.NickName;
        clickedNameText.text = clickedPlayer.NickName;
    }

    // 거래하기 제안 버튼 클릭 시
    public void OnOfferButtonClicked()
    {
        if (!initOffered) // 내가 거래 버튼을 이미 누른 상태라면 다시 누를 수 없음
        {
            Debug.Log("Offer button clicked");
            initOffered = true;
            photonView.RPC("ReceiveTradeOffer", RpcTarget.Others);
            CheckTradeStatus();
        }
    }

    // 상대방에서 거래하기 제안 수신
    [PunRPC]
    private void ReceiveTradeOffer()
    {
        clickedOffered = true;
        CheckTradeStatus();
    }

    // 취소 버튼 클릭 시
    public void OnCancelButtonClicked()
    {
        if (initOffered) // 내가 거래하기 버튼을 누른 상태라면
        {
            Debug.Log("Cancel button clicked");
            initOffered = false;
            clickedOffered = false; // 상대방의 거래 상태도 초기화
            photonView.RPC("CancelTradeOffer", RpcTarget.Others);
            gameObject.SetActive(false); // 패널 숨기기
        }
    }

    // 거래하기 그만하기 버튼 클릭 시
    private void OnStopTradeButtonClicked()
    {
        Debug.Log("Stop trade button clicked");
        initOffered = false;
        clickedOffered = false; // 거래하기 준비 상태 초기화
        gameObject.SetActive(false); // 패널 숨기기
    }

    // 거래하기 취소 메시지 수신
    [PunRPC]
    private void CancelTradeOffer()
    {
        clickedOffered = false; // 상대방의 거래 상태 초기화
    }

    // 거래 상태 확인 및 처리
    private void CheckTradeStatus()
    {
        if (initOffered && clickedOffered)
        {
            AcceptTrade();
        }
    }

    // 거래하기 수락 처리
    private void AcceptTrade()
    {
        Debug.Log("Trade accepted by both players");
        photonView.RPC("CompleteTrade", RpcTarget.Others);
        gameObject.SetActive(false); // 패널 숨기기
    }

    // 거래하기 완료 처리
    [PunRPC]
    private void CompleteTrade()
    {
        Debug.Log("Trade completed");
        // 여기서 아이템을 실제로 전송합니다.
    }
}
