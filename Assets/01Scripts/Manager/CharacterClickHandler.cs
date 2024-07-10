using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class CharacterClickHandler : MonoBehaviourPun
{
    public GameObject uiPanel; // 오른쪽 클릭 시 나타날 UI 패널
    public Canvas canvas; // UI 캔버스
    public PhotonView myPV;

    private PhotonView clickedPhotonView; // 클릭한 플레이어의 PhotonView

    [Header("Information")]
    [SerializeField]
    private GameObject clickHandlerPanel;
    [SerializeField]
    private GameObject informationPanel;

    [SerializeField]
    private Button informationBtn;
    [SerializeField]
    private Button exitInformationBtn;

    [SerializeField]
    private TextMeshProUGUI _nameText;
    [SerializeField]
    private TextMeshProUGUI _hpText;
    [SerializeField]
    private TextMeshProUGUI _atkText;
    [SerializeField]
    private TextMeshProUGUI _defText;
    [SerializeField]
    private TextMeshProUGUI _speedText;
    


    private string informationName;
    private float informationMaxHp;
    private float informationAtk;
    private float informationDef;
    private float informationSpeed;


    private void Start()
    {
        informationBtn.onClick.AddListener(InformationButton);
        exitInformationBtn.onClick.AddListener(ExitInformationPanel);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // 오른쪽 마우스 버튼 클릭 감지
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                PhotonView photonView = hit.collider.GetComponent<PhotonView>();
                if (photonView != null && !photonView.IsMine && hit.collider.CompareTag("Player"))
                {
                    clickedPhotonView = photonView;

                    // 클릭한 위치를 UI 캔버스의 로컬 좌표로 변환
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvas.transform as RectTransform,
                        Input.mousePosition,
                        canvas.worldCamera,
                        out Vector2 localPoint
                    );

                    // UI 패널 위치 설정
                    uiPanel.GetComponent<RectTransform>().localPosition = localPoint;
                    uiPanel.SetActive(true);


                    Character_Warrior cw = hit.collider.gameObject.GetComponent<Character_Warrior>();
                    informationMaxHp = cw.MaxHp;
                    informationAtk= cw.Atk; 
                    informationDef= cw.Def;
                    informationSpeed = cw.WalkSpeed;
                    informationName = clickedPhotonView.Owner.NickName;

                    InformationSet(informationName, informationMaxHp, informationAtk, informationDef, informationSpeed);
                }
            }
        }
    }

    private void InformationButton()
    {
        informationPanel.SetActive(true);
        clickHandlerPanel.SetActive(false);
    }

    private void ExitInformationPanel()
    {
        informationPanel.SetActive(false);
    }

    private void InformationSet(string _name, float _maxHp, float _atk, float _def, float _speed)
    {
        _nameText.text = "닉네임 : " + _name.ToString();
        _hpText.text = "HP : " + _maxHp.ToString();
        _atkText.text = "Atk : " + _atk.ToString();
        _defText.text = "Def : " + _def.ToString();
        _speedText.text = "Speed : " + _speed.ToString();
    }

    // 거래 제안 버튼 클릭 시 호출될 메서드
    public void OnTradeProposalButtonClicked()
    {
        // clickedPhotonView에 연결된 플레이어에게 거래 제안 메시지를 전송
        if (clickedPhotonView != null && clickedPhotonView.Owner != null)
        {
            // 거래 요청 보내기
            TradeManager.Instance.pv.RPC("ReceiveTradeRequest", clickedPhotonView.Owner, PhotonNetwork.LocalPlayer, clickedPhotonView.Owner);
            uiPanel.SetActive(false); // UI 패널 숨기기
        }
        else
        {
            Debug.LogError("Clicked PhotonView or its owner is null.");
        }
    }

    // Photon RPC를 통해 거래 제안 메시지를 전송
    [PunRPC]
    private void ReceiveTradeProposal(Player initPlayer)
    {
        //TradeManager.Instance.SendTradeRequest(initPlayer, PhotonNetwork.LocalPlayer);
    }

}
