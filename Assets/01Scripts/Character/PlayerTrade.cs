using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerTrade : MonoBehaviourPun
{
    void Update()
    {
        
    }

    // 타겟 플레이어를 찾는 임시 메서드
    Player FindTargetPlayer()
    {
        // 실제 게임에서는 타겟 플레이어를 찾는 로직을 구현해야 합니다.
        // 여기서는 예시로 첫 번째 다른 플레이어를 반환합니다.
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                return player;
            }
        }
        return null;
    }
}
