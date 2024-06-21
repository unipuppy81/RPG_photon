using UnityEngine;

public class TerrainAlignment : MonoBehaviour
{
    public Transform characterTransform;
    public LayerMask terrainLayer; // 지형 레이어를 설정합니다.
    public float raycastHeight = 1.0f; // 레이캐스트를 발사할 높이
    public float groundOffset = 0.1f; // 지형 위의 캐릭터 오프셋
    public float maxHeightDifference = 1.0f; // 캐릭터가 오를 수 있는 최대 높이 차이

    private Vector3 previousPosition;

    void Start()
    {
        previousPosition = characterTransform.position;
    }

    void Update()
    {
        AlignCharacterToTerrain();
    }

    void AlignCharacterToTerrain()
    {
        Vector3 rayOrigin = characterTransform.position + Vector3.up * raycastHeight;
        Ray ray = new Ray(rayOrigin, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastHeight * 2, terrainLayer))
        {
            // 현재 높이와 이전 위치의 높이 차이를 계산합니다.
            float heightDifference = Mathf.Abs(hit.point.y - previousPosition.y);

            if (heightDifference <= maxHeightDifference)
            {
                // 캐릭터의 위치를 지형의 높이에 맞춥니다.
                Vector3 newPosition = hit.point + Vector3.up * groundOffset;
                characterTransform.position = newPosition;

                // 지형의 법선에 맞춰 캐릭터의 회전을 조정합니다.
                Quaternion targetRotation = Quaternion.FromToRotation(characterTransform.up, hit.normal) * characterTransform.rotation;
                characterTransform.rotation = targetRotation;

                // 이전 위치를 업데이트합니다.
                previousPosition = characterTransform.position;
            }
            else
            {
                // 이동을 막고 이전 위치로 되돌립니다.
                characterTransform.position = previousPosition;
            }
        }
    }
}
