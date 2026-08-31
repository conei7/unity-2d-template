using UnityEngine;
using UnityEngine.Audio;

namespace Unity2DTemplate
{
    public sealed class AudioManager : MonoBehaviour
    {
        private const string BgmVolumeKey = "Audio.BgmVolume";
        private const string SeVolumeKey = "Audio.SeVolume";
        private const string BgmVolumeParameter = "BGMVolume";
        private const string SeVolumeParameter = "SEVolume";
        private const float DefaultVolume = 0.8f;
        private const float MutedDecibels = -80f;

        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource seSource;

        public static AudioManager Instance { get; private set; }

        public float BgmVolume { get; private set; }
        public float SeVolume { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            BgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey, DefaultVolume);
            SeVolume = PlayerPrefs.GetFloat(SeVolumeKey, DefaultVolume);
        }

        private void Start()
        {
            ApplyMixerVolume(BgmVolumeParameter, BgmVolume);
            ApplyMixerVolume(SeVolumeParameter, SeVolume);
        }

        public void SetBgmVolume(float volume)
        {
            BgmVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(BgmVolumeKey, BgmVolume);
            ApplyMixerVolume(BgmVolumeParameter, BgmVolume);
        }

        public void SetSeVolume(float volume)
        {
            SeVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(SeVolumeKey, SeVolume);
            ApplyMixerVolume(SeVolumeParameter, SeVolume);
        }

        public void PlayBgm(AudioClip clip)
        {
            if (clip == null || bgmSource == null)
            {
                return;
            }

            if (bgmSource.clip == clip && bgmSource.isPlaying)
            {
                return;
            }

            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        public void StopBgm()
        {
            if (bgmSource != null)
            {
                bgmSource.Stop();
            }
        }

        public void PlaySe(AudioClip clip)
        {
            if (clip != null && seSource != null)
            {
                seSource.PlayOneShot(clip);
            }
        }

        public void SaveVolumeSettings()
        {
            PlayerPrefs.Save();
        }

        private void ApplyMixerVolume(string parameterName, float volume)
        {
            if (audioMixer == null)
            {
                return;
            }

            float decibels = volume <= 0.0001f
                ? MutedDecibels
                : Mathf.Log10(volume) * 20f;

            audioMixer.SetFloat(parameterName, decibels);
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
            {
                SaveVolumeSettings();
            }
        }

        private void OnApplicationQuit()
        {
            SaveVolumeSettings();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
