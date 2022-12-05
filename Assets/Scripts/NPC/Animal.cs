using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    [SerializeField] protected string animalName; // 동물의 이름
    [SerializeField] protected int hp;            // 동물의 체력

    [SerializeField] protected float walkSpeed;   // 걷기 스피드
    [SerializeField] protected float runSpeed;    // 뛰기 스피드

    // nav 추가 이후 nav.speed에 적용됨.
    // [SerializeField] protected float turningSpeed;  // 회전스피드
    // protected float applySpeed;                   // 현재 스피드

    protected Vector3 destination;  // 랜덤 방향 저장

    // 상태 변수
    protected bool isAction;      // 행동 중인지 아닌지 판별
    protected bool isWalking;     // 걷는지 안 걷는지 판별
    protected bool isRunning;     // 뛰는지 판별
    protected bool isDead;        // 죽었는지 판별

    [SerializeField] protected float walkTime;    // 걷기 시간
    [SerializeField] protected float waitTime;    // 대기 시간
    [SerializeField] protected float runTime;     // 뛰기 시간
    protected float currentTime;                  // 진행 시간

    // 필요한 컴포넌트
    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected BoxCollider boxCol;
    protected AudioSource theAudio;
    protected NavMeshAgent nav;

    [SerializeField] protected AudioClip[] sound_Nomal;
    [SerializeField] protected AudioClip sound_Hurt;
    [SerializeField] protected AudioClip sound_Dead;

    void Start()
    {
        theAudio = GetComponent<AudioSource>();
        nav = GetComponent<NavMeshAgent>();
        currentTime = waitTime;
        isAction = true;
    }

    void Update()
    {
        if(!isDead){
            Move();         // 앞으로 움직이기
            // Rotation();     // 회전 방향
            ElapseTime();   // 랜덤 애니메이션    
        }
        
    }

    private void Move()
    {
        if (isWalking || isRunning){
            // rigid.MovePosition(transform.position + (transform.forward * applySpeed * Time.deltaTime));
            nav.SetDestination(transform.position + destination * 5f);
        }
    }

// nav로 인한 삭제 
    // private void Rotation()
    // {
    //     if (isWalking || isRunning){
    //         // pig의 방향이 한 프레임 마다 direction 값으로 0.01f 씩 가까워짐.
    //         Vector3 _rotation = Vector3.Lerp(transform.eulerAngles, new Vector3(0f, direction.y, 0f), turningSpeed);

    //         // Quaternion.Euler(vector3 값) : 벡터값을 쿼터니언 값으로 바꿔준다.
    //         rigid.MoveRotation(Quaternion.Euler(_rotation));
    //     }
    // }

    protected void ElapseTime()
    {
        if (isAction){
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
                ReSet();
        }
    }

    // 초기화 후 애니메이션 실행
    protected virtual void ReSet() {
        isWalking = false; isRunning = false; isAction = true;
        nav.ResetPath();
        anim.SetBool("Walking", isWalking); anim.SetBool("Running", isRunning);
        nav.speed = walkSpeed;
        destination.Set(Random.Range(-0.2f, 0.2f), 0f, Random.Range(-0.5f, 1f));
    }

    protected void TryWalk()
    {
        isWalking = true;
        currentTime = walkTime;
        nav.speed = walkSpeed;
        anim.SetBool("Walking", isWalking);
        Debug.Log("걷기");
    }

    // 피격 메소드
    public virtual void Damage(int _dmg, Vector3 _targetPos)
    {
        if (!isDead){
            hp -= _dmg;

            if (hp <= 0){
                Dead();
                return;
            }

            PlaySE(sound_Hurt);
            anim.SetTrigger("Hurt");
        }
    }

    // 죽음 메소드
    protected void Dead()
    {
        PlaySE(sound_Dead);
        isWalking = false;
        isRunning = false;
        isDead = true;
        anim.SetTrigger("Dead");
    }

    // 랜덤 사운드
    protected void RandomSound()
    {
        int random = Random.Range(0, 3);    // 일상 사운드 3개
        PlaySE(sound_Nomal[random]);
    }

    // 사운드 호출
    protected void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }
}
