using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    private List<Transform> positionsList = new List<Transform>();

    public static SpawnManager instance;
    public PhotonView pv;
    public Transform[] spawnPositons;

    public int currentGhostCount = 0;
    public int maxGhostCount = 2;

    private bool isCreatingPlayer = false;

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
        GameObject spawnParent = GameObject.Find("EnemySpawnPos");

        if (spawnParent != null)
        {
            // EnemySpawnPos의 모든 자식 Transform을 가져옵니다.
            spawnPositons = spawnParent.GetComponentsInChildren<Transform>();

            // 첫 번째 요소는 부모 자체의 Transform이므로 이를 제외합니다.
            spawnPositons = spawnPositons.Skip(1).ToArray();

            foreach (Transform pos in spawnPositons)
                positionsList.Add(pos);
        }
        else
        {
            Debug.LogError("EnemySpawnPos 게임 오브젝트를 찾을 수 없습니다.");
        }
    }

    [PunRPC]
    public void CreateComputerPlayer()
    {
        // 생성 중이면 더 이상 실행하지 않음
        if (isCreatingPlayer)
            return;

        isCreatingPlayer = true;

        int count = maxGhostCount - currentGhostCount;

        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, spawnPositons.Length);

            PhotonNetwork.InstantiateRoomObject("Ghost", spawnPositons[idx].position, Quaternion.identity);

            currentGhostCount++;
        }

        // 생성이 끝난 후 플래그를 리셋
        isCreatingPlayer = false;
    }

    [PunRPC]
    public void RemoveGhost()
    {
        currentGhostCount--;
        if (currentGhostCount <= 0)
        {
            currentGhostCount = 0;
        }
    }
}
