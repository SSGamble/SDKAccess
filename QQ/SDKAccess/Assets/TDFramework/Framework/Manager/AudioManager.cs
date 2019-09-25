/****************************************************
    文件：AudioManager.cs
	作者：TravelerTD
    日期：2019/8/7 16:20:50
	功能：⾳效管理
*****************************************************/

using UnityEngine;

namespace TDFramework {
    public class AudioManager : MonoSingleton<AudioManager> {
        private AudioListener mAudioListener;
        private AudioSource mMusicSource = null; // BGM

        private void CheckAudioListener() {
            if (!mAudioListener) {
                mAudioListener = FindObjectOfType<AudioListener>();
                if (!mAudioListener) {
                    mAudioListener = gameObject.AddComponent<AudioListener>();
                }
            }
        }

        #region 音效
        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="soundName"></param>
        public void PlaySound(string soundName) {
            CheckAudioListener();
            var audioSource = gameObject.AddComponent<AudioSource>();
            var coinSound = Resources.Load<AudioClip>(soundName);
            audioSource.clip = coinSound;
            audioSource.Play();
        }

        public void SoundOff() {
            // 获取 AudioManager GameObject 下的所有的 AudioSource，然后只要不等于 mMusicSource（不是BGM） 就全部静音
            var audioSources = GetComponents<AudioSource>();
            foreach (var audioSource in audioSources) {
                if (audioSource != mMusicSource) {
                    audioSource.Pause();
                    audioSource.mute = true;
                }
            }
        }

        public void SoundOn() {
            var audioSources = GetComponents<AudioSource>();
            foreach (var audioSource in audioSources) {
                if (audioSource != mMusicSource) {
                    audioSource.UnPause();
                    audioSource.mute = false;
                }
            }
        }
        #endregion

        #region 背景音乐
        /// <summary>
        /// 播放 BGM
        /// </summary>
        /// <param name="musicName"></param>
        /// <param name="loop"></param>
        public void PlayMusic(string musicName, bool loop) {
            CheckAudioListener();
            if (!mMusicSource) {
                mMusicSource = gameObject.AddComponent<AudioSource>();
            }
            var coinSound = Resources.Load<AudioClip>(musicName);
            mMusicSource.clip = coinSound;
            mMusicSource.loop = loop; // 循环播放
            mMusicSource.Play();
        }

        public void StopMusic() {
            mMusicSource.Stop();
        }

        public void PauseMusic() {
            mMusicSource.Pause();
        }

        public void ResumeMusic() {
            mMusicSource.UnPause();
        }

        public void MusicOff() {
            mMusicSource.Pause();
            mMusicSource.mute = true;
        }

        public void MusicOn() {
            mMusicSource.UnPause();
            mMusicSource.mute = false;
        }
        #endregion


    }
}