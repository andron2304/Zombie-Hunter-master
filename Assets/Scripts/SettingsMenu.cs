using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public AudioMixer audioMixer; // optional: expose parameters "MasterVolume", "MusicVolume", "SFXVolume"
    public AudioSource musicSource; // fallback for music volume when no AudioMixer is used
    // Debug helper: when true, route music AudioSource to no mixer group at Start
    public bool bypassMixerAtStart = false;
    // Exposed parameter names (set these to match your AudioMixer exposed parameters)
    public string masterParamName = "masterVolume";
    public string musicParamName = "musicVolume";
    public string sfxParamName = "sfxVolume";

    // Internal key to mark that defaults were initialized once
    const string INIT_KEY = "settings_initialized_v1";

    void Start()
    {
        // Ensure defaults are initialized only once; prevents accidental stored zero values
        if (PlayerPrefs.GetInt(INIT_KEY, 0) == 0)
        {
            PlayerPrefs.SetFloat("masterVolume", 1f);
            PlayerPrefs.SetFloat("musicVolume", 1f);
            PlayerPrefs.SetFloat("sfxVolume", 1f);
            PlayerPrefs.SetInt(INIT_KEY, 1);
            PlayerPrefs.Save();
            Debug.Log("SettingsMenu: PlayerPrefs initialized to defaults (1.0)");
        }

        float master = PlayerPrefs.GetFloat("masterVolume", 1f);
        float music = PlayerPrefs.GetFloat("musicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("sfxVolume", 1f);

        // Ensure sliders are configured to linear 0..1 range
        if (masterSlider != null)
        {
            masterSlider.minValue = 0f;
            masterSlider.maxValue = 1f;
            masterSlider.wholeNumbers = false;
            masterSlider.value = master;
        }
        if (musicSlider != null)
        {
            musicSlider.minValue = 0f;
            musicSlider.maxValue = 1f;
            musicSlider.wholeNumbers = false;
            musicSlider.value = music;
        }
        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.wholeNumbers = false;
            sfxSlider.value = sfx;
        }

        ApplyMasterVolume(master);
        ApplyMusicVolume(music);
        ApplySFXVolume(sfx);
        // Optionally bypass the AudioMixer at Start for quick isolation testing
        if (bypassMixerAtStart)
        {
            BypassMixerForMusic(true);
            Debug.Log("SettingsMenu: bypassMixerAtStart enabled — routed musicSource to null");
        }
        // Dump a quick audio state to help debugging in Editor
        DumpAudioState();
    }

    public void SetMasterVolume(float value)
    {
        Debug.Log("SettingsMenu.SetMasterVolume called: " + value);
        PlayerPrefs.SetFloat("masterVolume", value);
        ApplyMasterVolume(value);
    }

    void ApplyMasterVolume(float value)
    {
        if (audioMixer != null)
        {
            float tmp;
            if (audioMixer.GetFloat(masterParamName, out tmp))
            {
                audioMixer.SetFloat(masterParamName, LinearToDecibel(value));
                float after; audioMixer.GetFloat(masterParamName, out after);
                Debug.Log("SettingsMenu: " + masterParamName + " set to dB=" + after);
            }
            else
                Debug.LogWarning("SettingsMenu: AudioMixer parameter '" + masterParamName + "' not found. Assign or expose it in the Mixer.");
        }
        else
        {
            AudioListener.volume = value;
            Debug.Log("SettingsMenu: audioMixer not assigned — using AudioListener.volume as fallback.");
        }
    }

    public void SetMusicVolume(float value)
    {
        Debug.Log("SettingsMenu.SetMusicVolume called: " + value);
        PlayerPrefs.SetFloat("musicVolume", value);
        ApplyMusicVolume(value);
    }

    void ApplyMusicVolume(float value)
    {
        if (audioMixer != null)
        {
            float tmp;
            if (audioMixer.GetFloat(musicParamName, out tmp))
            {
                audioMixer.SetFloat(musicParamName, LinearToDecibel(value));
                float after;
                audioMixer.GetFloat(musicParamName, out after);
                Debug.Log("SettingsMenu: " + musicParamName + " set to dB=" + after);
                Debug.Log("SettingsMenu: AudioListener.volume=" + AudioListener.volume);
                // Also apply to the AudioSource volume as a fallback so changes are audible
                if (musicSource != null)
                {
                    Debug.Log("SettingsMenu: musicSource.volume BEFORE=" + musicSource.volume);
                    musicSource.volume = value;
                    Debug.Log("SettingsMenu: musicSource.volume AFTER=" + musicSource.volume + " (fallback)");
                    Debug.Log("SettingsMenu: musicSource.outputAudioMixerGroup=" + (musicSource.outputAudioMixerGroup != null ? musicSource.outputAudioMixerGroup.name : "null"));
                }
            }
            else
                Debug.LogWarning("SettingsMenu: AudioMixer parameter '" + musicParamName + "' not found. Expose it in the Mixer or set musicSource.");
        }
        else if (musicSource != null)
        {
            musicSource.volume = value;
        }
        else
        {
            Debug.Log("SettingsMenu: no audioMixer and no musicSource assigned — music volume not applied.");
        }
    }

    public void SetSFXVolume(float value)
    {
        Debug.Log("SettingsMenu.SetSFXVolume called: " + value);
        PlayerPrefs.SetFloat("sfxVolume", value);
        ApplySFXVolume(value);
    }

    void ApplySFXVolume(float value)
    {
        if (audioMixer != null)
        {
            float tmp;
            if (audioMixer.GetFloat(sfxParamName, out tmp))
            {
                audioMixer.SetFloat(sfxParamName, LinearToDecibel(value));
                float after; audioMixer.GetFloat(sfxParamName, out after);
                Debug.Log("SettingsMenu: " + sfxParamName + " set to dB=" + after);
            }
            else
                Debug.LogWarning("SettingsMenu: AudioMixer parameter '" + sfxParamName + "' not found. Expose it in the Mixer.");
        }
        // otherwise: SFX sources should read PlayerPrefs when they play
    }

    // Helpful debug snapshot to print what's assigned and whether params exist
    public void DumpAudioState()
    {
        Debug.Log("--- DumpAudioState ---");
        Debug.Log("audioMixer assigned: " + (audioMixer != null));
        if (audioMixer != null)
        {
            float val;
            if (audioMixer.GetFloat(masterParamName, out val)) Debug.Log(masterParamName + " present: dB=" + val);
            else Debug.LogWarning(masterParamName + " MISSING in AudioMixer");

            if (audioMixer.GetFloat(musicParamName, out val)) Debug.Log(musicParamName + " present: dB=" + val);
            else Debug.LogWarning(musicParamName + " MISSING in AudioMixer");

            if (audioMixer.GetFloat(sfxParamName, out val)) Debug.Log(sfxParamName + " present: dB=" + val);
            else Debug.LogWarning(sfxParamName + " MISSING in AudioMixer");
        }

        if (musicSource != null)
        {
            Debug.Log("musicSource assigned, isPlaying=" + musicSource.isPlaying + ", volume=" + musicSource.volume);
            Debug.Log("musicSource.outputAudioMixerGroup: " + (musicSource.outputAudioMixerGroup != null ? musicSource.outputAudioMixerGroup.name : "null"));
            Debug.Log("musicSource.clip: " + (musicSource.clip != null ? musicSource.clip.name : "null"));
        }
        else
        {
            Debug.LogWarning("musicSource not assigned on SettingsMenu.");
        }
        Debug.Log("--- End DumpAudioState ---");
    }

    // Helper to print slider config and to bypass mixer for quick testing
    public void DumpSliderState()
    {
        if (musicSlider != null)
        {
            Debug.Log("musicSlider.value=" + musicSlider.value + ", min=" + musicSlider.minValue + ", max=" + musicSlider.maxValue + ", wholeNumbers=" + musicSlider.wholeNumbers);
        }
        else
        {
            Debug.LogWarning("musicSlider not assigned to SettingsMenu.");
        }
    }

    // Temporarily bypass the AudioMixer by routing the music AudioSource to no mixer group
    public void BypassMixerForMusic(bool bypass)
    {
        if (musicSource == null)
        {
            Debug.LogWarning("BypassMixerForMusic: musicSource not assigned.");
            return;
        }
        if (bypass)
        {
            musicSource.outputAudioMixerGroup = null;
            Debug.Log("BypassMixerForMusic: musicSource routed to null (bypass)");
        }
        else
        {
            // attempt to restore by finding a group named "Music" on the assigned mixer
            if (audioMixer != null)
            {
                var groups = audioMixer.FindMatchingGroups("Music");
                if (groups != null && groups.Length > 0)
                {
                    musicSource.outputAudioMixerGroup = groups[0];
                    Debug.Log("BypassMixerForMusic: restored to group '" + groups[0].name + "'");
                }
                else Debug.LogWarning("BypassMixerForMusic: could not find group 'Music' on assigned AudioMixer.");
            }
            else Debug.LogWarning("BypassMixerForMusic: audioMixer not assigned; cannot restore group.");
        }
    }

    public void ResetDefaults()
    {
        SetMasterVolume(1f);
        SetMusicVolume(1f);
        SetSFXVolume(1f);
        if (masterSlider != null) masterSlider.value = 1f;
        if (musicSlider != null) musicSlider.value = 1f;
        if (sfxSlider != null) sfxSlider.value = 1f;
    }

    public void ShowPanel(GameObject panel)
    {
        if (panel != null) panel.SetActive(true);
    }

    public void HidePanel(GameObject panel)
    {
        if (panel != null) panel.SetActive(false);
    }

    public void TogglePanel(GameObject panel)
    {
        if (panel != null) panel.SetActive(!panel.activeSelf);
    }

    float LinearToDecibel(float linear)
    {
        linear = Mathf.Clamp(linear, 0.0001f, 1f);
        return Mathf.Log10(linear) * 20f;
    }
}
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Audio;

// public class SettingsMenu : MonoBehaviour
// {
//     public GameObject settingsPanel;
//     public Slider masterSlider;
//     public Slider musicSlider;
//     public AudioMixer audioMixer; // optional — expose "MasterVolume" and "MusicVolume"

//     const string MASTER_PREF = "masterVolume";
//     const string MUSIC_PREF = "musicVolume";

//     void Start()
//     {
//         if (settingsPanel != null)
//             settingsPanel.SetActive(false);

//         if (masterSlider != null)
//         {
//             masterSlider.onValueChanged.AddListener(SetMasterVolume);
//             masterSlider.value = PlayerPrefs.GetFloat(MASTER_PREF, 1f);
//         }

//         if (musicSlider != null)
//         {
//             musicSlider.onValueChanged.AddListener(SetMusicVolume);
//             musicSlider.value = PlayerPrefs.GetFloat(MUSIC_PREF, 1f);
//         }

//         ApplyVolumes();
//     }

//     public void ToggleSettings()
//     {
//         if (settingsPanel == null) return;
//         settingsPanel.SetActive(!settingsPanel.activeSelf);
//     }

//     public void SetMasterVolume(float value)
//     {
//         PlayerPrefs.SetFloat(MASTER_PREF, value);
//         ApplyVolumes();
//     }

//     public void SetMusicVolume(float value)
//     {
//         PlayerPrefs.SetFloat(MUSIC_PREF, value);
//         ApplyVolumes();
//     }

//     void ApplyVolumes()
//     {
//         float master = PlayerPrefs.GetFloat(MASTER_PREF, 1f);
//         float music = PlayerPrefs.GetFloat(MUSIC_PREF, 1f);

//         if (audioMixer != null)
//         {
//             // AudioMixer expects dB. Convert linear [0..1] to approximately -80..0 dB.
//             audioMixer.SetFloat("MasterVolume", ToDecibel(master));
//             audioMixer.SetFloat("MusicVolume", ToDecibel(music));
//         }
//         else
//         {
//             AudioListener.volume = master;
//             // Try to find a GameObject named "Music" with an AudioSource for background music
//             var musicGO = GameObject.Find("Music");
//             if (musicGO != null)
//             {
//                 var src = musicGO.GetComponent<AudioSource>();
//                 if (src != null)
//                     src.volume = music;
//             }
//         }
//     }

//     float ToDecibel(float linear)
//     {
//         linear = Mathf.Clamp01(linear);
//         if (linear <= 0.0001f) return -80f;
//         return Mathf.Log10(linear) * 20f;
//     }
// }
