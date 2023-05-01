using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;

    public Slider volumeSliderAmbient;
    public Slider volumeSliderSFX;

    const string MIXER_AMBIENT = "AmbientVolume";
    const string MIXER_SFX = "SFXVolume";

    private void Awake()
    {
        volumeSliderAmbient.onValueChanged.AddListener(SetAmbientVolume);
        volumeSliderSFX.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetAmbientVolume(float value)
    {
        mixer.SetFloat(MIXER_AMBIENT, Mathf.Log10(value) * 20);

        //PlayerPrefs.SetFloat(MIXER_AMBIENT, Mathf.Log10(value) * 20);

        //PlayerPrefs.Save();
    }

    private void SetSFXVolume(float value)
    {
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);

        //PlayerPrefs.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);

        //PlayerPrefs.Save();
    }

    public void LoadAmbientVolume()
    {
        float volume = PlayerPrefs.GetFloat(MIXER_AMBIENT, 0f);

        SetAmbientVolume(volume);
    }

    public void LoadSFXVolume()
    {
        float volume = PlayerPrefs.GetFloat(MIXER_SFX, 0f);

        SetSFXVolume(volume);
    }
}
