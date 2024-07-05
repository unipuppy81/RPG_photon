using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelManager : MonoBehaviour
{
    [Header("Setting UI")]
    [SerializeField]
    private Button ExitBtn;
    [SerializeField]
    private Button SettingBtn;
    [SerializeField]
    private Button QuestBtn;

    [SerializeField]
    private GameObject ExitPanel;
    [SerializeField]
    private GameObject SettingPanel;
    [SerializeField]
    private GameObject QuestPanel;


    [Header("Exit")]
    [SerializeField]
    private Button gameBackBtn;
    [SerializeField]
    private Button gameExitBtn;

    [Header("Setting")]
    [SerializeField]
    private Button SettingExitButton;

    [Header("Brightness")]
    [SerializeField] 
    private Image brightnessPanel; // 화면 밝기를 조절할 패널
    [SerializeField]
    private Slider brightnessSlider; // 밝기 조절 슬라이더

    [Header("Graphic")]
    [SerializeField]
    private Button lowSetBtn;
    [SerializeField]
    private Button middleSetBtn;
    [SerializeField]
    private Button highSetBtn;

    private void Start()
    {
        ExitBtn.onClick.AddListener(ExitPanelActive);
        gameBackBtn.onClick.AddListener(BackGame);
        gameExitBtn.onClick.AddListener(GameExit);

        SettingBtn.onClick.AddListener(SettingPanelActive);
        SettingExitButton.onClick.AddListener(SettingPanelExit);





        // 슬라이더 값 변경 시 OnBrightnessChange 메서드 호출
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChange);

        brightnessSlider.value = 0f;
        brightnessSlider.maxValue = 180f;
    }

    private void ExitPanelActive()
    {
        ExitPanel.SetActive(true);
    }

    private void BackGame()
    {
        ExitPanel.SetActive(false);
    }

    private void GameExit()
    {
        Debug.Log("Game Finish");
    }

    private void SettingPanelActive()
    {
        SettingPanel.SetActive(true);
    }
    private void SettingPanelExit()
    {
        SettingPanel.SetActive(false);
    }
    public void OnBrightnessChange(float value)
    {
        // 패널의 투명도 조절
        Color color = brightnessPanel.color;
        color.a = value / 255f; // 알파 값을 0에서 180 사이로 조절
        brightnessPanel.color = color;
    }
}
