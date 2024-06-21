using UnityEngine;

public class DontDestroyOnSceneChange : MonoBehaviour
{
    private void Awake()
    {
        // 현재 오브젝트를 씬 전환 시 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);
    }
}