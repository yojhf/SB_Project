using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twig : MonoBehaviour
{
    [SerializeField]
    private int hp;  // 나뭇 가지 체력. 0 이 되면 파괴.

    [SerializeField]
    private float destroyTime;  // 나뭇 가지 이펙트 (파티클 시스템) 삭제 시간

    [SerializeField]
    private GameObject go_little_Twig;  // `Little_Twig` 할당. 나뭇가지가 파괴될 때 두 동강나게. 더 작은 나뭇가지 프리팹.
    [SerializeField]
    private GameObject go_twig_hit_effect_prefab;  // `Leaf_Hit_Effect` 나뭇 가지 때릴 때 생성할 이펙트 프리팹

    [SerializeField]
    private float force;  // 생성된 두 개의 작은 나뭇가지를 밀어줄 힘의 크기

    /* 회전값 변수 */
    private Vector3 originRot;   // 나뭇 가지 원래 회전 값. (나뭇 가지 때리면 기울이게 할 것이라서 나중에 원래대로 돌아 올 때 필요)
    private Vector3 wantedRot;   // 나뭇 가지 때릴 때 회전 되길 원하는 값.
    private Vector3 currentRot;  // wanted_Rot 이 되기 위해 계속 업뎃해나갈 회전 값

    /* 필요한 사운드 이름.  (재생은 SoundManager.cs 싱글톤으로 하니까 곡 이름 string만 알면 됨) */
    [SerializeField]
    private string hit_Sound;
    [SerializeField]
    private string broken_Sound;

    void Start()
    {
        originRot = transform.rotation.eulerAngles;  // 보기 편하게 Vector3 로.
        currentRot = originRot;  // currentRot 초기값
    }

    public void Damage(Transform _playerTf)
    {
        hp--;

        Hit();

        StartCoroutine(HitSwayCoroutine(_playerTf));

        if (hp <= 0)
        {
            Destruction();
        }
    }

    private void Hit()
    {
        SoundManager.instance.PlaySE(hit_Sound);

        GameObject twig_particles = Instantiate(go_twig_hit_effect_prefab,
            gameObject.GetComponent<BoxCollider>().bounds.center + (Vector3.up * 0.5f),
            Quaternion.identity);

        Destroy(twig_particles, destroyTime);
    }

    IEnumerator HitSwayCoroutine(Transform _target)
    {
        Vector3 direction = (_target.position - transform.position).normalized; // 플레이어 나뭇가지 로 향하는 방향 

        Vector3 rotationDir = Quaternion.LookRotation(direction).eulerAngles;  // 플레이어 나뭇가지 방향을 바라보는 방향의 각도 값.

        CheckDirection(rotationDir);

        while (!CheckThreadhold())
        {
            currentRot = Vector3.Lerp(currentRot, wantedRot, 0.25f);
            transform.rotation = Quaternion.Euler(currentRot);
            yield return null;
        }

        wantedRot = originRot;

        while (!CheckThreadhold())
        {
            currentRot = Vector3.Lerp(currentRot, originRot, 0.15f);
            transform.rotation = Quaternion.Euler(currentRot);
            yield return null;
        }
    }

    private bool CheckThreadhold()
    {
        if (Mathf.Abs(wantedRot.x - currentRot.x) <= 0.5f && Mathf.Abs(wantedRot.z - currentRot.z) <= 0.5f)
            return true;
        return false;
    }

    private void CheckDirection(Vector3 _rotationDir)  // 어느 방향으로 나뭇 가지를 눕힐지.
    {
        Debug.Log(_rotationDir);

        if (_rotationDir.y > 180)
        {
            if (_rotationDir.y > 300)  // 300 ~ 360 
                wantedRot = new Vector3(-50f, 0f, -50f);
            else if (_rotationDir.y > 240) // 240 ~ 300
                wantedRot = new Vector3(0f, 0f, -50f);
            else    // 180 ~ 240
                wantedRot = new Vector3(50f, 0f, -50f);
        }
        else if (_rotationDir.y <= 180)
        {
            if (_rotationDir.y < 60)  // 0 ~ 60
                wantedRot = new Vector3(-50f, 0f, 50f);
            else if (_rotationDir.y > 120)  // 120 ~ 180
                wantedRot = new Vector3(0f, 0f, 50f);
            else  // 60 ~ 120
                wantedRot = new Vector3(50f, 0f, 50f);
        }
    }

    private void Destruction()
    {
        SoundManager.instance.PlaySE(broken_Sound);

        GameObject little_twig_1 = Instantiate(go_little_Twig,
                            gameObject.GetComponent<BoxCollider>().bounds.center + (Vector3.up * 0.5f),
                            Quaternion.identity);
        GameObject little_twig_2 = Instantiate(go_little_Twig,
                            gameObject.GetComponent<BoxCollider>().bounds.center - (Vector3.up * 0.5f),
                            Quaternion.identity);

        little_twig_1.GetComponent<Rigidbody>().AddForce(Random.Range(-force, force), 0, Random.Range(-force, force));
        little_twig_2.GetComponent<Rigidbody>().AddForce(Random.Range(-force, force), 0, Random.Range(-force, force));

        Destroy(little_twig_1, destroyTime);
        Destroy(little_twig_2, destroyTime);

        Destroy(gameObject);
    }
}
