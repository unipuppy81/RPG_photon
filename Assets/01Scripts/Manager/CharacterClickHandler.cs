using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class CharacterClickHandler : MonoBehaviourPun
{
    public GameObject uiPanel; // 오른쪽 클릭 시 나타날 UI 패널
    public Canvas canvas; // UI 캔버스

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
                    Debug.Log("Click Player");

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
                }
            }
        }
    }
}
