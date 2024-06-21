using Photon.Pun;
using UnityEngine;

public class PlayerAnimatorSync : MonoBehaviourPun, IPunObservable
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 데이터를 보내기
            stream.SendNext(animator.GetBool("UseSkill"));
        }
        else
        {
            // 데이터를 받기
            bool isAttacking = (bool)stream.ReceiveNext();
            animator.SetBool("UseSkill", isAttacking);
        }
    }
}
