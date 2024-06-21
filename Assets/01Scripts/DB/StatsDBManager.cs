using UnityEngine;
using Photon.Pun;
public class StatsDBManager : MonoBehaviourPunCallbacks
{
    public static StatsDBManager instance { get; private set; }


    [SerializeField]
    public DataBase statsDB;

    private void Awake()
    {
        // 인스턴스가 이미 존재하면 새로 생성하지 않고 기존 인스턴스를 사용합니다.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지되도록 설정합니다.
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 생성을 방지합니다.
            return;
        }
    }
}
