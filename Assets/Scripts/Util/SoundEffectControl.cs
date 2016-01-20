using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectControl : MonoBehaviour
{

    private static SoundEffectControl instance;
    //private static GameObject gameobject2;

    private AudioSource bg_audio;
    private AudioSource sfsx_audio;
    
    /* 생성할때 */
    public static SoundEffectControl Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameobject = new GameObject("SoundManager");
                instance = gameobject.AddComponent<SoundEffectControl>();
                //gameobject2 = new GameObject("SoundManager2");
                //gameobject2.AddComponent<AudioSource>();
                /* 모든 씬에서 사용하므로 파괴 불가로 설정 */
                DontDestroyOnLoad(gameobject);
                //DontDestroyOnLoad(gameobject2);
            }
            return instance;
        }
    }
    void Awake() 
    {
        bg_audio = gameObject.GetComponent<AudioSource>();
        sfsx_audio = gameObject.GetComponent<AudioSource>();
    }
    /* 배경음 출력 */
    public void PlayBackgroundMusic(string soundName)
    {
        if (bg_audio.clip != null)
        {
            bg_audio.Stop();
            bg_audio.clip = null;
        }
        bg_audio.clip = (AudioClip)Resources.Load("Sounds/" + soundName);
        bg_audio.loop = true;
        bg_audio.Play();
    }
    /* 이펙트 출력 */
    public void PlayEffectSound(string soundName)
    {
        sfsx_audio.PlayOneShot((AudioClip)Resources.Load("Sounds/" + soundName));
    }
    public void PlayEffectSound(string soundName, float soundVolume)
    {
        sfsx_audio.volume = soundVolume;
        sfsx_audio.PlayOneShot((AudioClip)Resources.Load("Sounds/" + soundName));
    }
    /* 사운드 볼륨 조절 */
    public void ChangeVolume(string type, float value)
    {
        if (type.CompareTo("BGM") == 0)
            bg_audio.volume = value;
        else if (type.CompareTo("SFSX") == 0)
            sfsx_audio.volume = value;
    }
    public bool IsEnd() 
    {
        return bg_audio.isPlaying;
    }
    public float ReturnTotalTime()
    {
        return bg_audio.clip.length;
    }
    public float ReturnPlayTime()
    {
        return bg_audio.time;
    }
}