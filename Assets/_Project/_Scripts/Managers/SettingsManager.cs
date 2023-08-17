using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsManager : Singleton<SettingsManager>
{
    #region Variables

    [field: SerializeField] public CanvasGroup SettingsUI { get; set; }
    [field: SerializeField, BoxGroup("Sound and Vibration")] public GameObject SoundOnButton { get; set; }
    [field: SerializeField, BoxGroup("Sound and Vibration")] public GameObject SoundOffButton { get; set; }
    [field: SerializeField, BoxGroup("Sound and Vibration")] public GameObject VibrationOnButton { get; set; }
    [field: SerializeField, BoxGroup("Sound and Vibration")] public GameObject VibrationOffButton { get; set; }
    [field: SerializeField, BoxGroup("Debug Sliders")] public Slider BulletAmountSlider { get; set; }
    [field: SerializeField, BoxGroup("Debug Sliders")] public Slider AimSensitivitySlider { get; set; }
    [field: SerializeField, BoxGroup("Debug Sliders")] public Slider GunRecoilSlider { get; set; }
    [field: SerializeField, BoxGroup("Debug Sliders")] public Slider GodModeSlider { get; set; }
    [field: SerializeField, BoxGroup("Slider Value Texts")] public TextMeshProUGUI BulletAmountSliderValueText { get; set; }
    [field: SerializeField, BoxGroup("Slider Value Texts")] public TextMeshProUGUI AimSensitivitySliderValueText { get; set; }
    [field: SerializeField, BoxGroup("Slider Value Texts")] public TextMeshProUGUI GunRecoilSliderValueText { get; set; }
    [field: SerializeField, BoxGroup("Slider Value Texts")] public TextMeshProUGUI GodModeSliderValueText { get; set; }
    [SerializeField] private GameplayData _gameplayData;

    public UnityAction OnSaveSettings;
    public bool IsSoundActivated { get; private set; }
    public bool IsVibrationActivated { get; private set; }
    private bool IsSettingsOpened { get; set; }

    #endregion

    #region Get Starting Data

    private void Start() => GetStartingData();

    private void GetStartingData()
    {
        if (!PlayerPrefs.HasKey("SoundSettings"))
        {
            EnableSound(false);
        }
        else
        {
            if (PlayerPrefs.GetInt("SoundSettings") == 0)
            {
                DisableSound();
            }
            else if (PlayerPrefs.GetInt("SoundSettings") == 1)
            {
                EnableSound(false);
            }
        }

        if (!PlayerPrefs.HasKey("VibrationSettings"))
        {
            EnableVibration(false);
        }
        else
        {
            if (PlayerPrefs.GetInt("VibrationSettings") == 0)
            {
                DisableVibration(false);
            }
            else if (PlayerPrefs.GetInt("VibrationSettings") == 1)
            {
                EnableVibration(false);
            }
        }

        if (!PlayerPrefs.HasKey("BulletAmount"))
        {
            BulletAmountSlider.value = 10;
            _gameplayData.BulletAmount = (int)BulletAmountSlider.value;
        }
        else
        {
            BulletAmountSlider.value = PlayerPrefs.GetFloat("BulletAmount");
            _gameplayData.BulletAmount = (int)BulletAmountSlider.value;
        }

        if (!PlayerPrefs.HasKey("AimSensitivity"))
        {
            AimSensitivitySlider.value = 10f;
            _gameplayData.AimSensitivity = AimSensitivitySlider.value;
        }
        else
        {
            AimSensitivitySlider.value = PlayerPrefs.GetFloat("AimSensitivity");
            _gameplayData.AimSensitivity = AimSensitivitySlider.value;
        }

        if (!PlayerPrefs.HasKey("AimShakeStrength"))
        {
            GunRecoilSlider.value = 10f;
            _gameplayData.AimShakeStrength = GunRecoilSlider.value;
        }
        else
        {
            GunRecoilSlider.value = PlayerPrefs.GetFloat("AimShakeStrength");
            _gameplayData.AimShakeStrength = GunRecoilSlider.value;
        }

        if (!PlayerPrefs.HasKey("GodMode"))
        {
            GodModeSlider.value = 0f;
            _gameplayData.GodMode = GodModeSlider.value;
        }
        else
        {
            GodModeSlider.value = PlayerPrefs.GetFloat("GodMode");
            _gameplayData.GodMode = GodModeSlider.value;
        }
        
        SetSliderValueTextsOnStart();
    }

    #endregion

    #region Settings

    public void OpenSettingsUI()
    {
        UIManager.Instance.FadeCanvasGroup(SettingsUI, 0.5f, true);
    }

    public void CloseSettingsUI()
    {
        UIManager.Instance.FadeCanvasGroup(SettingsUI, 0.5f, false);
        SaveSettings();
    }

    public void EnableSound(bool playAudio)
    {
        SoundOnButton.SetActive(true);
        SoundOffButton.SetActive(false);
        IsSoundActivated = true;
        PlayerPrefs.SetInt("SoundSettings", 1);
        AudioManager.Instance.Listener.enabled = true;
    }

    public void DisableSound()
    {
        SoundOnButton.SetActive(false);
        SoundOffButton.SetActive(true);
        IsSoundActivated = false;
        PlayerPrefs.SetInt("SoundSettings", 0);
        AudioManager.Instance.Listener.enabled = false;
    }

    public void EnableVibration(bool playAudio)
    {
        VibrationOnButton.SetActive(true);
        VibrationOffButton.SetActive(false);
        IsVibrationActivated = true;
        PlayerPrefs.SetInt("VibrationSettings", 1);
        Vibration.VibratePeek();
    }

    public void DisableVibration(bool playAudio)
    {
        VibrationOnButton.SetActive(false);
        VibrationOffButton.SetActive(true);
        IsVibrationActivated = false;
        PlayerPrefs.SetInt("VibrationSettings", 0);
    }

    public void SaveSettings()
    {
        _gameplayData.BulletAmount = (int)BulletAmountSlider.value;
        _gameplayData.AimSensitivity = AimSensitivitySlider.value;
        _gameplayData.AimShakeStrength = GunRecoilSlider.value;
        _gameplayData.GodMode = GodModeSlider.value;
        PlayerPrefs.SetFloat("BulletAmount", BulletAmountSlider.value);
        PlayerPrefs.SetFloat("AimSensitivity", AimSensitivitySlider.value);
        PlayerPrefs.SetFloat("AimShakeStrength", GunRecoilSlider.value);
        PlayerPrefs.SetFloat("GodMode", GodModeSlider.value);
        OnSaveSettings?.Invoke();
    }

    public void SetBulletAmountSliderValueText()
    {
        BulletAmountSliderValueText.text = Mathf.RoundToInt(BulletAmountSlider.value).ToString();
    }

    public void SetAimSensitivitySliderValueText()
    {
        AimSensitivitySliderValueText.text = Mathf.RoundToInt(AimSensitivitySlider.value).ToString();
    }

    public void SetGunRecoilSliderValueText()
    {
        GunRecoilSliderValueText.text = Mathf.RoundToInt(GunRecoilSlider.value).ToString();
    }

    public void SetGodModeSliderValueText()
    {
        GodModeSliderValueText.text = Mathf.RoundToInt(GodModeSlider.value).ToString();
    }

    private void SetSliderValueTextsOnStart()
    {
        SetBulletAmountSliderValueText();
        SetAimSensitivitySliderValueText();
        SetGunRecoilSliderValueText();
        SetGodModeSliderValueText();
    }

    #endregion
}