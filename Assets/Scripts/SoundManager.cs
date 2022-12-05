using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;     // 곡의 이름.
    public AudioClip clip;  // 곡 클립.
}

public class SoundManager : MonoBehaviour
{
    static public SoundManager instance;    // 싱글톤
    #region singleton
    void Awake() // 객체 생성 시 최초 실행.
    {
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);  // 파괴 금지 지정
        }
        else
            Destroy(gameObject);
    }
    #endregion singleton
    
    public AudioSource[] audioSourceEffect; // 이팩트 사운드
    public AudioSource audioSourceBgm;      // 브금 사운드

    public string[] playSoundName;          // 재생 중인 사운드 이름

    public Sound[] effectSounds;            // 이팩트 사운드 클립
    public Sound[] bgmSounds;               // 브금 사운드 클립

    private void Start() {
        playSoundName = new string[audioSourceEffect.Length];
    }

    // 사운드 실행
    public void PlaySE(string _name)
    {
        for (int i = 0; i < effectSounds.Length; i++){

            // 호출된 사운드 이름이 존재하는지 확인
            if (_name == effectSounds[i].name){
                for (int j = 0; j < audioSourceEffect.Length; j++){

                    // 호출되지 않은 사운드 찾기
                    if (!audioSourceEffect[j].isPlaying){
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffect[j].clip = effectSounds[i].clip;
                        audioSourceEffect[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용 AudioSource가 사용 중입니다.");
                return;
            }
        }
        Debug.Log(_name + "사운드가 SoundManager에 등록되지 않았습니다.");
    }

    // 전체 사운드 끄기
    public void StopAllSE()
    {
        for (int i = 0; i < audioSourceEffect.Length; i++)
        {
            audioSourceEffect[i].Stop();
        }
    }

    // 지정된 사운드 끄기
    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffect.Length; i++)
        {
            if (playSoundName[i] == _name){
                audioSourceEffect[i].Stop();
                return;
            }
        }
        Debug.Log("재생 중인" + _name + "사운드가 없습니다.");
    }
}
