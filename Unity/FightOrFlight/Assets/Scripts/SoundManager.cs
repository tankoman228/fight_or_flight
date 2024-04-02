using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    internal class SoundManager : MonoBehaviour
    {
        private static SoundManager _instance;

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
    }
}
