using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField]
    private int hp; // 바위의 체력

    [SerializeField]
    private float destroyTime;      // 파편 제거 시간

    [SerializeField]
    private SphereCollider col;     // 구체 콜라이더


    [SerializeField]
    private GameObject go_rock;     // 일반 바위.
    [SerializeField]
    private GameObject go_debris;   // 깨진 바위
    [SerializeField]
    private GameObject go_effect_prefab;    // 채굴 이팩트
    [SerializeField]
    private GameObject go_rock_item_prefab; // 돌맹이 아이템

    // 돌맹이 아이템 등장 개수
    private int count;

    // 필요한 사운드 이름
    [SerializeField]
    private string strike_Sound;
    [SerializeField]
    private string destroy_Sound;

    private void Start() {
        count = Random.Range(2, 5); // 돌맹이 등장 개수 설정
    }

    // 데미지를 입을 때
    public void Mining()
    {
        SoundManager.instance.PlaySE(strike_Sound);

        var clone = Instantiate(go_effect_prefab, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;
        if (hp <= 0){
            Destruction();
        }
    }

    // 부서졌을 때
    private void Destruction()
    {
        SoundManager.instance.PlaySE(destroy_Sound);

        col.enabled = false;
        // 돌맹이 소환
        for (int i = 1; i <= count; i++){
            Instantiate(go_rock_item_prefab, 
                        new Vector3(go_rock.transform.position.x, go_rock.transform.position.y+0.5f, go_rock.transform.position.z),
                        Quaternion.identity);
        }
        
        Destroy(go_rock);

        go_debris.SetActive(true);
        Destroy(go_debris, destroyTime);
    }
}