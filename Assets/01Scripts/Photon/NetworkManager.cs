using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/*
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

    [Header("Chat")]
    public TMP_InputField ChatInput;
    public GameObject chatPanel, chatView;

    private TextMeshProUGUI[] chatList;

    public GameObject playerSpawnPosObj;
    public Transform[] spawnPositions;

    public PhotonView PV;

    private void Awake()
    {
        // 싱글턴 패턴 적용
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("게임 씬 네트워크 매니저 시작");
        chatList = chatView.GetComponentsInChildren<TextMeshProUGUI>();

        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Photon에 연결되지 않았습니다.");
            return;
        }

        //OnJoinedRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("04. 방 입장 완료");

        // 채팅 패널 on
        chatPanel.SetActive(true);

        ChatInput.text = "";
        foreach (TextMeshProUGUI chat in chatList)
            chat.text = "";


        Debug.Log("Spawn Warrior");
        PhotonNetwork.Instantiate("Warrior", playerSpawnPosObj.transform.position, Quaternion.identity);


        StartCoroutine(DelayedSetup());
    }


    private IEnumerator DelayedSetup()
    {
        yield return new WaitUntil(() => GameManager.isPlayGame);

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnManager.instance.pv.RPC("CreateComputerPlayer", RpcTarget.All);
            PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>[방장] " + PhotonNetwork.NickName + "님이 참가하셨습니다</color>");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + otherPlayer.NickName + "님이 나갔습니다</color>");
    }

    public void Send()
    {
        if (ChatInput.text.Equals(""))
            return;
        string msg = "[" + PhotonNetwork.NickName + "] " + ChatInput.text;
        PV.RPC("ChatRPC", RpcTarget.All, msg);
        ChatInput.text = "";
    }

    [PunRPC]
    void ChatRPC(string msg)
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

    [PunRPC]
    public void RequestSceneChange(int playerID, PhotonView pv, string sceneName)
    {

        // 요청된 플레이어 ID에게 씬 전환 명령을 전달
        Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(playerID);
        if (targetPlayer != null)
        {
            pv.RPC("ChangeSceneForLocalPlayer", targetPlayer, sceneName);
        }
        
    }

    public override void OnLeftRoom()
    {

    }

}
*/

public class NetworkManager : SingletonPhoton<NetworkManager>
{
    [Header("Loading")]
    [SerializeField]
    private GameObject loadingScreen;
    [SerializeField]
    private TextMeshProUGUI loadingText;
    [SerializeField]
    private Slider loadingSlider;

    [Header("NickName")]
    public TextMeshProUGUI StatusText;
    public TMP_InputField NickNameInput;
    public Button startButton;
    public GameObject startPanel;


    [Header("Chat")]
    public TMP_InputField ChatInput;
    public GameObject chatPanel, chatView;

    private TextMeshProUGUI[] chatList;


    // 플레이어 생성 위치
    public GameObject playerSpawnPosObj;

    // 적 생성 위치
    public Transform[] spawnPositions;
    private int idx;


    [Header("Photon")]
    public readonly string gameVersion = "v1.0";
    public PhotonView PV;

    void Awake()
    {
        // 방장이 혼자 씬을 로딩하면, 나머지 사람들은 자동으로 싱크가 됨
        PhotonNetwork.AutomaticallySyncScene = true;

    }

    void Start()
    {
        ShowLoadingScreen();

        Debug.Log("00. 포톤 매니저 시작");

        chatList = chatView.GetComponentsInChildren<TextMeshProUGUI>();

        //startButton.onClick.AddListener(JoinRoom);
        //OnLogin();

        PhotonNetwork.GameVersion = this.gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }
    void OnLogin()
    {
        PhotonNetwork.ConnectUsingSettings();
        startButton.interactable = false;
        StatusText.text = "마스터 서버에 접속중...";
    }

    void JoinRoom()
    {
        if (NickNameInput.text.Equals(""))
            PhotonNetwork.LocalPlayer.NickName = "unknown";
        else
            PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;

        PhotonNetwork.JoinRandomRoom();
        Debug.Log("01. 포톤 서버에 접속");

    }

    void Connect() => PhotonNetwork.ConnectUsingSettings();



    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("01. 포톤 서버에 접속");

        //StatusText.text = "Online: 마스터 서버와 연결 됨";
        //startButton.interactable = true;

        //PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("02. 랜덤 룸 접속 실패");

        // 룸 속성 설정
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 5;

        // 룸 생성 -> 자동 입장
        PhotonNetwork.CreateRoom("room_1", ro);

    }


    public override void OnCreatedRoom()
    {
        Debug.Log("03. 방 생성 완료");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("04. 방 입장 완료");


        // 닉네임 패널 off
        //startPanel.SetActive(false);


        // 채팅 패널 on
        chatPanel.SetActive(true);

        ChatInput.text = "";
        foreach (TextMeshProUGUI chat in chatList)
            chat.text = "";

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnManager.instance.pv.RPC("CreateComputerPlayer", RpcTarget.All);
            PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>[방장] " + PhotonNetwork.NickName + "님이 참가하셨습니다</color>");
        }

        PhotonNetwork.Instantiate("Warrior", playerSpawnPosObj.transform.position, Quaternion.identity);

        
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        this.CreateRoom();
    }

    void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.CleanupCacheOnLeave = true; // 방장이 나가도 오브젝트 유지

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    // 방장이 변경되었을 때 호출되는 콜백 메서드
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("New master client: " + newMasterClient.NickName);
        // 필요한 초기화 작업을 추가할 수 있습니다.
    }

    // 플레이어가 나갔을 때 호출되는 콜백 메서드
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " has left the room.");
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + otherPlayer.NickName + "님이 나갔습니다</color>");

    }

    private void ShowLoadingScreen()
    {
        // 로딩 화면을 보여주는 메서드
        loadingScreen.SetActive(true);
        StartCoroutine(IncreaseSliderValue());
        loadingText.text = "게임 접속 중...";
    }
    IEnumerator IncreaseSliderValue()
    {
        float timer = 0f;
        float startValue = loadingSlider.value;
        float endValue = 1f;

        while (timer < 4.0f)
        {
            timer += Time.deltaTime;
            float progress = timer / 4.0f;
            loadingSlider.value = Mathf.Lerp(startValue, endValue, progress);
            yield return null;
        }

        // duration이 끝난 후 최대 값으로 설정
        loadingSlider.value = 1f;
    }
    public void HideLoadingScreen()
    {
        // 로딩 화면을 숨기는 메서드
        loadingScreen.SetActive(false);
    }

    // 플레이어 접속 시 채팅 창 출력
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
    }

    // 채팅 전송 함수
    public void Send()
    {
        if (ChatInput.text.Equals(""))
            return;
        string msg = "[" + PhotonNetwork.NickName + "] " + ChatInput.text;
        PV.RPC("ChatRPC", RpcTarget.All, msg);
        ChatInput.text = "";
    }

    [PunRPC]
    void ChatRPC(string msg)
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
