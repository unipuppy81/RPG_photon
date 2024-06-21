using UnityEngine;
using Photon.Pun;

public static class CombatCalculator
{
    private const float defenseAdjustment = 50f;

    [PunRPC]
    public static float CalculateDamage(float attack, float defense)
    {
        float damage = attack * (1 - (defense / (defense + defenseAdjustment)));

        Debug.Log(damage);

        return Mathf.Max(0, damage);
    }
}
