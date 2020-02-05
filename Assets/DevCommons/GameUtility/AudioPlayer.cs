using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevCommons
{
    namespace Utility
    {
        public class AudioPlayer : MonoBehaviour
        {
            private Action m_OnComplete;
            private AudioPlayerData m_AudioPlayerData;
            private AudioSource m_AudioSource;

            public static AudioPlayer Play(AudioPlayerData audioPlayerData, Action onComplete = null)
            {
                GameObject go = new GameObject("AudioPlayer");
                AudioPlayer audioPlayer = go.AddComponent<AudioPlayer>();

                audioPlayer.m_AudioPlayerData = audioPlayerData;
                audioPlayer.m_OnComplete = onComplete;

                if (audioPlayerData.oneShot)
                    audioPlayer.PlayOneShot();
                else
                    audioPlayer.PlayAudio();

                return audioPlayer;
            }

            public AudioSource GetAudioSource()
            {
                return m_AudioSource;
            }

            public void PlayOneShot()
            {
                if (m_AudioPlayerData.audioClip == null)
                    return;
                m_AudioSource = gameObject.AddComponent<AudioSource>();
                m_AudioSource.playOnAwake = false;
                m_AudioSource.volume = m_AudioPlayerData.volume;
                m_AudioSource.PlayOneShot(m_AudioPlayerData.audioClip);
                StartCoroutine(OnComplete(m_AudioPlayerData.audioClip.length));
            }

            public void PlayAudio()
            {
                if (m_AudioPlayerData.audioClip == null)
                    return;
                m_AudioSource = gameObject.AddComponent<AudioSource>();
                m_AudioSource.playOnAwake = false;
                m_AudioSource.volume = m_AudioPlayerData.volume;
                m_AudioSource.clip = m_AudioPlayerData.audioClip;
                m_AudioSource.loop = m_AudioPlayerData.loop;
                m_AudioSource.Play();
                if (!m_AudioPlayerData.loop)
                    StartCoroutine(OnComplete(m_AudioPlayerData.audioClip.length));
            }

            private IEnumerator OnComplete(float clipLength)
            {
                yield return new WaitForSeconds(clipLength);
                Destroy();
            }

            public void Destroy()
            {
                m_OnComplete?.Invoke();
                Destroy(gameObject, 0.02f);
            }
        }

        [System.Serializable]
        public class AudioPlayerData
        {
            public bool oneShot = true;
            public bool loop = false;
            public AudioClip audioClip = null;
            public float volume = 1.0f;
        }
    }
}
