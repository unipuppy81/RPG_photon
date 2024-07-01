using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class LoginManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public TMP_InputField NickNameInput;
    public TextMeshProUGUI StatusText;
    public Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(OnLoginButtonClicked);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "v1.0";
    }

    void OnLoginButtonClicked()
    {
        if (string.IsNullOrEmpty(NickNameInput.text))
        {
            StatusText.text = "닉네임을 입력하세요.";
            return;
        }

        PhotonNetwork.NickName = NickNameInput.text;
        StatusText.text = "마스터 서버에 접속중...";
        PhotonNetwork.ConnectUsingSettings();
        startButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        StatusText.text = "마스터 서버와 연결 됨";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        StatusText.text = "로비에 접속 완료. 게임을 시작합니다...";
        PhotonNetwork.JoinRandomRoom(); // 방에 랜덤으로 입장 시도
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected: {cause}");
        StatusText.text = $"연결이 끊어졌습니다: {cause}";
        startButton.interactable = true;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random room, creating a new room...");
        CreateRoom();
    }

    void CreateRoom()
    {
        string roomName = "Room_" + Random.Range(0, 1000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.CleanupCacheOnLeave = true;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created successfully");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room successfully");
        StatusText.text = "방에 접속 완료. 게임을 시작합니다...";
        PhotonNetwork.LoadLevel("GameScene"); // 게임 씬으로 전환
    }
}
