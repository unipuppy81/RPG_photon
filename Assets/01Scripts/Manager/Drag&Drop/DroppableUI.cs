using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DroppableUI : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    private Image image;
    private RectTransform rect;

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }


    /// <summary>
    /// 마우스 포인터가 현재 아이템 슬롯 영역 내부로 들어갈 때 1회 호출
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer Enter");
        image.color = Color.yellow;
    }


    /// <summary>
    /// 마우스 포인터가 현재 아이템 슬롯 영역을 빠져나갈때 1회호출
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer Exit");
        image.color = Color.white;
    }


    /// <summary>
    /// 현재 아이템 슬롯 영역 내부에서 드롭을 했을 때 1회 호출
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        // pointerDrag는 현재 드래그하고있는 대상(=아이템)
        if (eventData.pointerDrag != null)
        {
            // 드래그 대상의 원래 부모 저장
            Transform originalParent = eventData.pointerDrag.transform.parent;

            // 드래그 대상을 복제하여 현재 오브젝트로 설정하고, 위치를 현재 오브젝트와 동일하게 설정
            GameObject draggedObject = Instantiate(eventData.pointerDrag, transform);
            draggedObject.transform.localPosition = Vector3.zero;

            // 드래그 대상의 부모를 현재 오브젝트로 설정
            eventData.pointerDrag.transform.SetParent(transform);

            // 원래 부모로 복사한 드래그 대상을 다시 설정
            eventData.pointerDrag.transform.SetParent(originalParent);

            CanvasGroup canvasGroup = draggedObject.GetComponent<CanvasGroup>();
            TextMeshProUGUI tmp = draggedObject.GetComponentInChildren<TextMeshProUGUI>();
            Slot slot = draggedObject.GetComponent<Slot>();
            //Image _image = draggedObject.GetComponentInChildren<Image>();

            //image = _image;
            slot.enabled = false;
            tmp.text = "";
            canvasGroup.alpha = 1.0f;
        }

    }

}
