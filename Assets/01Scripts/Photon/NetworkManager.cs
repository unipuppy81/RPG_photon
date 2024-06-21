using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Text statusText;
    public InputField nickNameInput;

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
        Debug.Log("00. 포톤 매니저 시작");

        PhotonNetwork.GameVersion = this.gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("01. 포톤 서버에 접속");

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

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnManager.instance.pv.RPC("CreateComputerPlayer", RpcTarget.All);
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
    }
}
