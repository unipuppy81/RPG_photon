using UnityEngine;
using Photon.Pun;
public class LookAtCamera : MonoBehaviourPunCallbacks
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // 주 카메라를 찾음
    }

    void Update()
    {
        // 텍스트가 항상 카메라를 바라보게 함
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }
}
