using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Mixer")]
    public AudioMixer audioMixer;
    public string sfxParamName = "sfxVolume";

    [Header("Playback")]
    public AudioSource sfxSource; // single source used for PlayOneShot
    public AudioClip shootClip;
    public AudioClip zombieDeathClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        // If an AudioMixer is assigned, try to route the sfxSource to an 'SFX' group for centralized control
        if (audioMixer != null && sfxSource != null)
        {
            var groups = audioMixer.FindMatchingGroups("SFX");
            if (groups != null && groups.Length > 0)
            {
                sfxSource.outputAudioMixerGroup = groups[0];
                Debug.Log("SFXManager: routed sfxSource to mixer group '" + groups[0].name + "'");
            }
            else
            {
                Debug.Log("SFXManager: no 'SFX' group found on assigned AudioMixer — leave outputAudioMixerGroup as-is.");
            }
        }
    }

    // Play specific SFX helpers
    public void PlayShoot()
    {
        PlayClip(shootClip);
    }

    public void PlayZombieDeath()
    {
        PlayClip(zombieDeathClip);
    }

    public void PlayClip(AudioClip clip)
    {
        if (clip == null) return;

        float vol = PlayerPrefs.GetFloat("sfxVolume", 1f);

        // If an AudioMixer is assigned and has the exposed parameter, write it (keeps mixer in sync)
        if (audioMixer != null)
        {
            float tmp;
            if (audioMixer.GetFloat(sfxParamName, out tmp))
            {
                audioMixer.SetFloat(sfxParamName, LinearToDecibel(vol));
            }
        }

        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, vol);
        }
        else
        {
            AudioSource.PlayClipAtPoint(clip, Vector3.zero, vol);
        }
    }

    public void UpdateVolumeFromPrefs()
    {
        float vol = PlayerPrefs.GetFloat("sfxVolume", 1f);
        if (audioMixer != null)
        {
            float tmp;
            if (audioMixer.GetFloat(sfxParamName, out tmp))
                audioMixer.SetFloat(sfxParamName, LinearToDecibel(vol));
        }
        if (sfxSource != null)
            sfxSource.volume = vol;
    }

    static float LinearToDecibel(float linear)
    {
        linear = Mathf.Clamp(linear, 0.0001f, 1f);
        return Mathf.Log10(linear) * 20f;
    }
}
