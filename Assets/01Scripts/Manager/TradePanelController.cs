using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class TradePanelController : MonoBehaviourPunCallbacks
{
    public PhotonView tradePanelPhotonView;

    public static TradePanelController Instance;

    public TextMeshProUGUI initNameText;
    public TextMeshProUGUI clickedNameText;

    public GameObject tradeButtonObj;
    public GameObject nonTradeButtonObj;
    public GameObject tradeExitButtonObj;

    public GameObject TradeResultSuccessPanel;
    public GameObject TradeResultFailPanel;

    private Button tradeButton;         // 등록하기 버튼
    private Button nonTradeButton;      // 등록취소 버튼
    private Button tradeExitButton;     // 거래 그만하기 버튼

    private Player initPlayer;
    private Player clickedPlayer;

    private bool initOffered = false; // 내가 등록 버튼을 눌렀는지 여부
    private bool clickedOffered = false; // 상대방이 등록 버튼을 눌렀는지 여부

    private const string INIT_OFFERED = "InitOffered";
    private const string CLICKED_OFFERED = "ClickedOffered";

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

        // 초기화 시 두 플레이어의 등록 상태를 초기화
        Hashtable initProps = new Hashtable { { INIT_OFFERED, false } };
        initPlayer.SetCustomProperties(initProps);

        Hashtable clickedProps = new Hashtable { { CLICKED_OFFERED, false } };
        clickedPlayer.SetCustomProperties(clickedProps);
    }

    private void OnEnable()
    {
        GameManager.isTradeChatting = true;
    }

    private void OnDisable()
    {
        GameManager.isTradeChatting = false;
    }
    // 등록 버튼 클릭 시, 서로의 bool 은 서버에서 처리
    public void OnTradeButtonClicked()
    {
        if (tradeButtonObj.activeSelf)
        {
            tradeButtonObj.SetActive(false);
            nonTradeButtonObj.SetActive(true);
        }

        if (PhotonNetwork.LocalPlayer == initPlayer)
        {
            Hashtable props = new Hashtable { { INIT_OFFERED, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        else if (PhotonNetwork.LocalPlayer == clickedPlayer)
        {
            Hashtable props = new Hashtable { { CLICKED_OFFERED, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        tradePanelPhotonView.RPC("CheckTradeStatus", RpcTarget.All);
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
            Hashtable props = new Hashtable { { INIT_OFFERED, false } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        else if (PhotonNetwork.LocalPlayer == clickedPlayer)
        {
            Hashtable props = new Hashtable { { CLICKED_OFFERED, false } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        tradePanelPhotonView.RPC("CheckTradeStatus", RpcTarget.All);
    }


    // 거래 그만하기 버튼 클릭 시
    public void OnTradeExitButtonClicked()
    {
        ResetTradeState();

        tradePanelPhotonView.RPC("TradeFail", RpcTarget.All);

        gameObject.SetActive(false); // 패널 숨기기
    }

    // 거래 상태 초기화 - 클라이언트
    private void ResetTradeState()
    {
        if (PhotonNetwork.LocalPlayer == initPlayer)
        {
            Hashtable props = new Hashtable { { INIT_OFFERED, false } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        else if (PhotonNetwork.LocalPlayer == clickedPlayer)
        {
            Hashtable props = new Hashtable { { CLICKED_OFFERED, false } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        tradePanelPhotonView.RPC("ResetPartnerTradeState", RpcTarget.Others);
    }

    // 거래 상태 초기화 - 서버 및 동기화
    [PunRPC]
    public void ResetPartnerTradeState()
    {
        if (PhotonNetwork.LocalPlayer == initPlayer)
        {
            Hashtable props = new Hashtable { { CLICKED_OFFERED, false } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        else if (PhotonNetwork.LocalPlayer == clickedPlayer)
        {
            Hashtable props = new Hashtable { { INIT_OFFERED, false } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }

    // 거래 상태 확인 및 처리
    [PunRPC]
    public void CheckTradeStatus()
    {
        if (initPlayer.CustomProperties.ContainsKey(INIT_OFFERED) &&
            clickedPlayer.CustomProperties.ContainsKey(CLICKED_OFFERED))
        {
            initOffered = (bool)initPlayer.CustomProperties[INIT_OFFERED];
            clickedOffered = (bool)clickedPlayer.CustomProperties[CLICKED_OFFERED];

            if (initOffered && clickedOffered)
            {
                AcceptTrade();
            }
        }
    }

    // 거래 성공 처리
    private void AcceptTrade()
    {
        tradePanelPhotonView.RPC("CompleteTrade", RpcTarget.All);
    }

    // 거래 완료 처리
    [PunRPC]
    public void CompleteTrade()
    {
        Debug.Log("Trade completed");
        // 여기서 아이템을 실제로 전송합니다.

        tradePanelPhotonView.RPC("TradeSuccess", RpcTarget.All);
        gameObject.SetActive(false); // 패널 숨기기
    }

    [PunRPC]
    public void TradeFail()
    {
        TradeResultFailPanel.SetActive(true);
    }

    [PunRPC]
    public void TradeSuccess()
    {
        TradeResultSuccessPanel.SetActive(true);
    }
}
