using System.Collections;
using UnityEngine;
using Photon.Pun;

public class DistanceBaseVisibility : MonoBehaviourPunCallbacks
{
    public float maxDistance; // 최대 거리
    private Transform player;
    private LODGroup lodGroup;

    void Start()
    {
        maxDistance = 5.0f;

        lodGroup = GetComponent<LODGroup>();
        StartCoroutine(AssignPlayer());
    }

    private IEnumerator AssignPlayer()
    {
        while (player == null)
        {
            player = FindObjectOfType<Character_Warrior>()?.transform;
            if (player == null)
            {
                yield return new WaitForSeconds(0.5f); // 플레이어를 찾을 때까지 대기
            }
        }
    }

    void Update()
    {
        if (player == null || lodGroup == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > maxDistance)
        {
            if (lodGroup.enabled)
            {
                lodGroup.enabled = false; // maxDistance 이상이면 LOD 그룹 비활성화
            }
        }
        else
        {
            if (!lodGroup.enabled)
            {
                lodGroup.enabled = true; // maxDistance 이내면 LOD 그룹 활성화
            }
        }
    }
}
