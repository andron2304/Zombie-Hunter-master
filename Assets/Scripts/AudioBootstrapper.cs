using UnityEngine;
using UnityEngine.SceneManagement;

// Ensures audio manager singletons exist across scene loads.
[DefaultExecutionOrder(-1000)]
public static class AudioBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        // Run once after first scene load
        EnsureAudioManagers();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureAudioManagers();
    }

    static void EnsureAudioManagers()
    {
        // Ensure SFXManager
        if (SFXManager.Instance == null)
        {
            Debug.Log("AudioBootstrapper: SFXManager missing — creating auto-instance.");
            var go = new GameObject("SFXManager_Auto");
            var mgr = go.AddComponent<SFXManager>();
            // leave fields for the user to assign in Inspector later
            Object.DontDestroyOnLoad(go);
        }

        // Ensure MusicPlayer
        if (MusicPlayer.Instance == null)
        {
            Debug.Log("AudioBootstrapper: MusicPlayer missing — creating auto-instance.");
            var go = new GameObject("MusicPlayer_Auto");
            var src = go.AddComponent<AudioSource>();
            // default settings: do not loop by default (user can change), but play on awake so music can start if clip assigned
            src.playOnAwake = true;
            var player = go.AddComponent<MusicPlayer>();
            Object.DontDestroyOnLoad(go);
        }
    }
}
