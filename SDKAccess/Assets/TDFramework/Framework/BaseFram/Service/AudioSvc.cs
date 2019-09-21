/****************************************************
    文件：AudioSvc.cs
	作者：TravelerTD
    日期：2019/09/01 22:21:05
	功能：声音播放服务
*****************************************************/

using TDFramework;
using UnityEngine;

public class AudioSvc : MonoSingleton<AudioSvc> {
    /// <summary>
    /// 背景音乐
    /// </summary>
    public AudioSource bgAudio;
    /// <summary>
    /// UI 音乐
    /// </summary>
    public AudioSource uiAudio;

    /// <summary>
    /// 初始化声音播放服务
    /// </summary>
    public void InitSvc() {
        TDLog.Log("Init AudioSvc...");
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name">音乐名</param>
    /// <param name="isLoop">是否循环播放</param>
    public void PlayBGMusic(string name, bool isLoop = true) {
        AudioClip ac = ResSvc.Instance.LoadAudio(ConStr.AudioPath + name);
        // 当前没有音乐或音乐不一样
        if (bgAudio.clip == null || bgAudio.clip.name != ac.name) {
            bgAudio.clip = ac; // 设置当前音乐
            bgAudio.loop = isLoop;
            bgAudio.Play();
        }
    }

    /// <summary>
    /// 播放 UI 操作音效
    /// </summary>
    /// <param name="name">音效名</param>
    public void PlayUIAudio(string name) {
        AudioClip ac = ResSvc.Instance.LoadAudio(ConStr.AudioPath + name);
        uiAudio.clip = ac; // 设置当前音乐
        uiAudio.Play();
    }

    /// <summary>
    /// 播放人物音效
    /// </summary>
    /// <param name="name">音效名</param>
    /// <param name="audioSrc">播放组件</param>
    public void PlayCharAudio(string name, AudioSource audioSrc) {
        AudioClip audio = ResSvc.Instance.LoadAudio(ConStr.AudioPath + name);
        audioSrc.clip = audio;
        audioSrc.Play();
    }

    /// <summary>
    /// 停止播放背景音乐
    /// </summary>
    public void StopBGMusic() {
        if (bgAudio != null) {
            bgAudio.Stop();
        }
    }

}