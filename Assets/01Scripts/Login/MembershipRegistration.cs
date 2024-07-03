using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class MembershipManager : MonoBehaviourPunCallbacks
{
    [Header("Login")]
    [SerializeField]
    private TextMeshProUGUI StatusText;
    [SerializeField]
    private TMP_InputField loginIdInput;
    [SerializeField]
    private TMP_InputField loginPasswordInput;

    [Header("Sign Up")]
    [SerializeField]
    private TMP_InputField idInput;   // 아이디 입력 필드
    [SerializeField]
    private TMP_InputField passwordInput;   // 비밀번호 입력 필드
    [SerializeField]
    private TMP_InputField nicknameInput;   // 닉네임 입력 필드 (회원가입용)

    [SerializeField]
    private Button loginButton;         // 로그인 버튼
    [SerializeField]
    private Button SignupPanelButton;
    [SerializeField]
    private Button registerButton;      // 회원 가입 버튼
    [SerializeField]
    private Button ExitButton;

    [SerializeField]
    private GameObject SignUpPanel;

    private void Start()
    {
        registerButton.onClick.AddListener(RegisterUser);
        loginButton.onClick.AddListener(LoginUser);
        SignupPanelButton.onClick.AddListener(SignupPanel);
        ExitButton.onClick.AddListener(ExitSignupPanel);

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "v1.0";
    }

    // 로그인
    private void LoginUser()
    {
        string userId = loginIdInput.text;    // 입력된 사용자명
        string password = loginPasswordInput.text;    // 입력된 비밀번호

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("사용자명과 비밀번호는 비어있을 수 없습니다.");
            return;
        }

        // 저장된 사용자 데이터 가져오기
        if (PlayerPrefs.HasKey(userId))
        {
            string savedPassword = PlayerPrefs.GetString(userId);

            if (savedPassword == password)
            {
                // 로그인 성공, 닉네임 가져오기
                string nickname = PlayerPrefs.GetString(userId + "_nickname");

                // PhotonNetwork에 닉네임 설정 및 연결
                PhotonNetwork.NickName = nickname;

                PhotonNetwork.LoadLevel("GameScene"); // 게임 씬으로 전환
                //PhotonNetwork.ConnectUsingSettings();

                loginButton.interactable = false;
            }
            else
            {
                Debug.LogWarning("비밀번호가 일치하지 않습니다.");
            }
        }
        else
        {
            Debug.LogWarning("사용자가 존재하지 않습니다.");
        }
    }

    // 회원가입
    private void RegisterUser()
    {
        string username = idInput.text;    // 입력된 사용자명
        string password = passwordInput.text;    // 입력된 비밀번호
        string nickname = nicknameInput.text;    // 입력된 닉네임

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nickname))
        {
            Debug.LogWarning("사용자명, 비밀번호 및 닉네임은 비어있을 수 없습니다.");
            return;
        }

        // 사용자가 이미 존재하는지 확인합니다.
        if (PlayerPrefs.HasKey(username))
        {
            Debug.LogWarning("이미 아이디가 존재합니다.");
            return;
        }

        // PlayerPrefs를 사용하여 사용자 데이터를 저장합니다.
        PlayerPrefs.SetString(username, password);
        PlayerPrefs.SetString(username + "_nickname", nickname);
        PlayerPrefs.Save();

        Debug.Log("사용자 등록 완료: " + username);

        ExitSignupPanel();
    }

    // Event
    private void SignupPanel()
    {
        SignUpPanel.SetActive(true);

        loginIdInput.text = "";
        loginPasswordInput.text = "";
    }

    private void ExitSignupPanel()
    {
        SignUpPanel.SetActive(false);

        idInput.text = "";
        passwordInput.text = "";
        nicknameInput.text = "";
    }

    /*
    public override void OnConnectedToMaster()
    {
        StatusText.text = "서버와 연결 됨";
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        StatusText.text = "게임에 접속을 시작하겠습니다...";

        PhotonNetwork.JoinRandomRoom(); // 방에 랜덤으로 입장 시도
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        StatusText.text = $"연결이 끊어졌습니다: {cause}";
        loginButton.interactable = true;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
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
        StatusText.text = "접속 완료";

        //LoadingSceneController.LoadScene("GameScene");
        PhotonNetwork.LoadLevel("GameScene"); // 게임 씬으로 전환
    }
    */
}
