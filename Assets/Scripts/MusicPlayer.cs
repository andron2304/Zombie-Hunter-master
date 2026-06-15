using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            var src = GetComponent<AudioSource>();
            if (!src.isPlaying) src.Play();
        }
        else if (Instance != this)
        {
            // Another MusicPlayer already exists (from previous scene). Destroy this duplicate.
            Destroy(gameObject);
        }
    }
}