using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class NpcDistance : MonoBehaviour
{
    public List<GameObject> playerArr = new List<GameObject>();       // 플레이어 오브젝트

    [SerializeField]
    private TextMeshPro displayText;        // 표시할 텍스트 UI 요소
    [SerializeField]
    private TextMeshPro nameText;

    public float detectRadius = 2.0f; // 텍스트를 표시할 거리

    public LayerMask playerLayer;

    private ObjData objData;

    
    private void Awake()
    {
        StartCoroutine(DelayedSetup());
    }

    void Update()
    {
        // 주변의 플레이어 탐지
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, detectRadius, playerLayer);

        // 현재 프레임의 플레이어 목록
        List<GameObject> currentPlayers = new List<GameObject>();

        foreach (Collider player in hitPlayers)
        {
            currentPlayers.Add(player.gameObject);
        }

        // playerArr에 없는 플레이어를 추가
        foreach (GameObject player in currentPlayers)
        {
            if (!playerArr.Contains(player))
            {
                Character_Warrior cw = player.GetComponent<Character_Warrior>();
                cw.isCommunicate = true;
                playerArr.Add(player);
            }
        }

        // currentPlayers에 없는 플레이어를 playerArr에서 제거
        for (int i = playerArr.Count - 1; i >= 0; i--)
        {
            if (!currentPlayers.Contains(playerArr[i]))
            {
                playerArr.RemoveAt(i);
            }
        }

        // 텍스트 표시 여부
        if (playerArr.Count > 0)
        {
            displayText.gameObject.SetActive(true);
        }
        else
        {
            displayText.gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        // 텍스트가 항상 카메라를 바라보게 설정
        //displayText.transform.LookAt(displayText.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }

    private IEnumerator DelayedSetup()
    {
        yield return new WaitUntil(() => GameManager.isPlayGame);


        objData = GetComponent<ObjData>();

        if (objData != null)
        {
            nameText.text = objData.objName;
        }

    }
}
