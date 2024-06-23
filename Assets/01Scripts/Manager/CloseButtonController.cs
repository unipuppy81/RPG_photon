using UnityEngine.UI;
using UnityEngine;

public class CloseButtonController : MonoBehaviour
{
    public Button closeButton; // 패널을 끄는 버튼

    void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }
    }

    void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
