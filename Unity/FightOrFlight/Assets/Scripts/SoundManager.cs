using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    internal class SoundManager : MonoBehaviour
    {
        private AudioSource musicSource;
        private static string currentMode;
        private static SoundManager _instance;
        private Dictionary<string, AudioClip[]> musicDictionary = new Dictionary<string, AudioClip[]>();

        public static SoundManager Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                // Do not modify _instance here. It will be assigned in awake
                return new GameObject("(singleton) SoundManager").AddComponent<SoundManager>();
            }
        }

        void Awake()
        {
            // Only one instance of SoundManager at a time!
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            musicSource = gameObject.AddComponent<AudioSource>();

            musicDictionary.Add("menu", Resources.LoadAll<AudioClip>("Music/Menu"));
            musicDictionary.Add("lobby", Resources.LoadAll<AudioClip>("Music/Lobby"));
            musicDictionary.Add("timer", Resources.LoadAll<AudioClip>("Music/Timer"));
            musicDictionary.Add("game", Resources.LoadAll<AudioClip>("Music/Game"));
        }

        public static void PlaySound(GameObject gameObject, string soundName)
        {
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            AudioClip clip = Resources.Load<AudioClip>("Sounds/" + soundName);
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("Sound clip " + soundName + " not found in Resources folder.");
            }
        }

        public static void changeMusic(string mode)
        {
            if (mode == currentMode)
            {
                return;
            }

            currentMode = mode;

            Instance.StopAllCoroutines();
            Instance.PlayRandomTrack();
        }

        private void PlayRandomTrack()
        {
            StartCoroutine(PlayRandomTrackCoroutine());
        }

        private IEnumerator PlayRandomTrackCoroutine()
        {
            if (currentMode != null && musicDictionary.ContainsKey(currentMode))
            {
                if (_instance.musicSource != null)
                    _instance.musicSource.Stop();

                AudioClip[] musicClips = musicDictionary[currentMode];
                int randomIndex = UnityEngine.Random.Range(0, musicClips.Length);
                if (randomIndex >= musicClips.Length)
                {
                    randomIndex--;
                    Debug.LogError("chatGPT was not RIGHT");
                }

                musicSource.clip = musicClips[randomIndex];
                musicSource.Play();

                yield return new WaitForSeconds(musicSource.clip.length);

                StartCoroutine(PlayRandomTrackCoroutine());
            }
        }
    }
}
