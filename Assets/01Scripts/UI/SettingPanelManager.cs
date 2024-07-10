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
    private Button QuestionBtn;

    [SerializeField]
    private GameObject ExitPanel;
    [SerializeField]
    private GameObject SettingPanel;
    [SerializeField]
    private GameObject QuestionPanel;


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


    [Header("Question")]
    [SerializeField]
    private Button showExplainBtn;
    [SerializeField]
    private Button showKeySetBtn;
    [SerializeField]
    private Button questionPanelExitBtn;


    [SerializeField]
    private GameObject explainPanel;
    [SerializeField]
    private GameObject keySetPanel;


    private void Start()
    {
        ExitBtn.onClick.AddListener(ExitPanelActive);
        gameBackBtn.onClick.AddListener(BackGame);
        gameExitBtn.onClick.AddListener(GameExit);

        SettingBtn.onClick.AddListener(SettingPanelActive);
        SettingExitButton.onClick.AddListener(SettingPanelExit);

        QuestionBtn.onClick.AddListener(QuestionPanelActive);
        showExplainBtn.onClick.AddListener(ShowExplainPanel);
        showKeySetBtn.onClick.AddListener(ShowKeySetPanel);

        questionPanelExitBtn.onClick.AddListener(QuestionPanelExit);





        // 슬라이더 값 변경 시 OnBrightnessChange 메서드 호출
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChange);

        brightnessSlider.minValue = 30f;
        brightnessSlider.maxValue = 180f;
        brightnessSlider.value = 130f;

        OnBrightnessChange(brightnessSlider.value);
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
        GameManager.Instance.GameSave();

        Application.Quit();
    }

    private void SettingPanelActive()
    {
        SettingPanel.SetActive(true);
    }
    public void SettingPanelExit()
    {
        SettingPanel.SetActive(false);
    }
    public void OnBrightnessChange(float value)
    {
        // 패널의 투명도 조절
        Color color = brightnessPanel.color;
        color.a = 1f - (value / 180f);
        brightnessPanel.color = color;
    }

    private void QuestionPanelActive()
    {
        QuestionPanel.SetActive(true);
    }

    private void ShowKeySetPanel()
    {
        keySetPanel.SetActive(true);
        explainPanel.SetActive(false);
    }

    private void ShowExplainPanel()
    {
        keySetPanel.SetActive(false);
        explainPanel.SetActive(true);
    }




    private void QuestionPanelExit()
    {
        QuestionPanel.SetActive(false);
    }



}
