using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject focusMe;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    [SerializeField] Color[] colourPalette1;
    [SerializeField] Color[] colourPalette2;
    [SerializeField] Color[] colourPalette3;
    [SerializeField] Button[] buttons;
    [SerializeField] TextMeshProUGUI[] texts;
    [SerializeField] Image[] imagesForeground;
    [SerializeField] Image[] imagesBackground;
    [SerializeField] Image[] imagesBackgroundDark;
    [SerializeField] Image pauseMenuBG;

    void Start()
    {
        InitValues();
        ListResolutions();
    }

    void InitValues()
    {
        audioMixer.SetFloat("_masterVolume", PlayerPrefs.GetFloat("_masterVolume") - 40);
        audioMixer.SetFloat("_musicVolume", PlayerPrefs.GetFloat("_musicVolume") - 40);
        audioMixer.SetFloat("_SFXVolume", PlayerPrefs.GetFloat("_SFXVolume") - 40);
        masterSlider.value = PlayerPrefs.GetFloat("_masterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("_musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("_SFXVolume");

        qualityDropdown.value = PlayerPrefs.GetInt("_qualityLevel");
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("_qualityLevel"));
    }
    public void Play()
    {
        SceneManager.LoadScene("Game");
    }
    public void FocusFocusMe()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(focusMe);
    }
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("_masterVolume", volume - 40);
        PlayerPrefs.SetFloat("_masterVolume", volume);
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("_musicVolume", volume - 40);
        PlayerPrefs.SetFloat("_musicVolume", volume);
    }
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("_SFXVolume", volume - 40);
        PlayerPrefs.SetFloat("_SFXVolume", volume);
    }
    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("_qualityLevel", index);
    }
    void ListResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new();

        int currentIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                currentIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
    public void SetColourPalette(int index)
    {
        switch (index)
        {
            case 1:
                ColourSwitcher(colourPalette1);
                break;
            case 2:
                ColourSwitcher(colourPalette2);
                break;
            case 3:
                ColourSwitcher(colourPalette3);
                break;
            default:
                Debug.Log("Wtf?");
                break;
        }
    }
    void ColourSwitcher(Color[] palette)
    {
        ButtonColourSwitcher(palette);
        TextColourSwitcher(palette);
        ImageColourSwitcher(palette);
    }
    void ButtonColourSwitcher(Color[] palette)
    {
        Camera.main.backgroundColor = palette[0];
        pauseMenuBG.color = palette[0];
        foreach (Button button in buttons)
        {
            var colors = button.colors;
            colors.normalColor = palette[3];
            colors.highlightedColor = palette[2];
            colors.pressedColor = palette[1];
            colors.selectedColor = palette[0];
            colors.disabledColor = palette[4];
            button.colors = colors;
        }
    }
    void TextColourSwitcher(Color[] palette)
    {
        foreach (TextMeshProUGUI text in texts)
            text.color = palette[3];
    }
    void ImageColourSwitcher(Color[] palette)
    {
        foreach(Image image in imagesForeground)
            image.color = palette[2];

        foreach(Image image in imagesBackground)
            image.color = palette[1];

        foreach (Image image in imagesBackgroundDark)
            image.color = palette[5];
    }
}