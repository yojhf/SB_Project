using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    // 체력
    [SerializeField]
    private int hp;
    private int currentHp;

    // 스태미나
    [SerializeField]
    private int sp;
    private int currentSp;

    // 스태미나 증가량
    [SerializeField]
    private int splncreaseSpeed;

    // 스태미나 재회복 딜레이
    [SerializeField]
    private int spRechargeTime;
    private int currentSpRechargeTime;

    // 스태미나 감소 여부
    private bool spUsed;

    // 방어력
    [SerializeField]
    private int dp;
    private int currentDp;

    // 배고픔
    [SerializeField]
    private int hungry;
    private int currentHungry;

    // 배고픔이 줄어드는 속도
    [SerializeField]
    private int hungryDecreaseTime;
    private int currentHungryDecreaseTime;

    // 목마름
    [SerializeField]
    private int thirsty;
    private int currentThirsty;

    // 목마름이 줄어드는 속도
    [SerializeField]
    private int thirstyDecreaseTime;
    private int currentThirstyDecreaseTime;

    // 만족도
    [SerializeField]
    private int satisfy;
    private int currentSatisfy;

    // 필요한 이미지 
    [SerializeField]
    private Image[] images_Gauge;

    private const int HP = 0, DP = 1, SP = 2, HUNGRY = 3, THIRSTY = 4, SATISFY = 5;

    void Start()
    {
        currentHp = hp;
        currentSp = sp;
        currentDp = dp;
        currentHungry = hungry;
        currentThirsty = thirsty;
        currentSatisfy = satisfy;
    }

    void Update()
    {
        GaugeUpdate();      // 현재 게이지 업데이트
        SPRechargeTime();   // 스태미나 감소 확인
        SPRecover();        // 스태미나 증가
        Hungry();           // 배고픔 관리
        Thirsty();          // 목마름 관리
    }

    private void GaugeUpdate()
    {
        images_Gauge[HP].fillAmount = (float)currentHp / hp;
        images_Gauge[SP].fillAmount = (float)currentSp / sp;
        images_Gauge[DP].fillAmount = (float)currentDp / dp;
        images_Gauge[HUNGRY].fillAmount = (float)currentHungry / hungry;
        images_Gauge[THIRSTY].fillAmount = (float)currentThirsty / thirsty;
        images_Gauge[SATISFY].fillAmount = (float)currentSatisfy / satisfy;
    }

    private void Hungry()
    {
        if (currentHungry >= 0){
            if (currentHungryDecreaseTime <= hungryDecreaseTime){
                currentHungryDecreaseTime++;
            }
            else{
                currentHungry--;
                currentHungryDecreaseTime = 0;
            }
        }
//        else Debug.Log("배고픔 수치가 0이 되었습니다.");
    }

    private void Thirsty()
    {
        if (currentThirsty >= 0){
            if (currentThirstyDecreaseTime <= thirstyDecreaseTime){
                currentThirstyDecreaseTime++;
            }
            else{
                currentThirsty--;
                currentThirstyDecreaseTime = 0;
            }
        }
//        else Debug.Log("목마름 수치가 0이 되었습니다.");
    }

    private void SPRechargeTime()
    {
        if (spUsed){
            if (currentSpRechargeTime < spRechargeTime){
                currentSpRechargeTime++;
            }
            else
                spUsed = false;
        }
    }

    private void SPRecover()
    {
        if (!spUsed && currentSp < sp){
            currentSp += splncreaseSpeed;
        }
    }

    // 스태미나 감소 
    public void DecreaseStamina(int _count)
    {
        spUsed = true;
        currentSpRechargeTime = 0;

        if (currentSp - _count > 0){
            currentSp -= _count;
        }
        else
            currentSp = 0;
    }

    // 스태미나 여부 반환
    public bool GetStamina()
    {
        if (currentSp > 0)
            return true;
        else
            return false;
    }

    public void IncreaseHP(int _num)
    {
        currentHp += _num;

        if (currentHp >= hp){
            currentHp = hp;
        }
    }

    public void DecreaseHP(int _num)
    {
        if (currentDp > 0)
            DecreaseDP(_num);
        else
            currentHp -= _num;

        if (currentHp <= 0){
            // Dead 관련 코드
            Debug.Log("체력 : " + currentHp);
        }
    }

    public void IncreaseSP(int _num)
    {
        currentSp += _num;
        
        if (currentSp >= sp){
            currentSp = sp;
        }
    }

    public void IncreaseDP(int _num)
    {
        currentDp += _num;
        
        if (currentDp >= dp){
            currentDp = dp;
        }
    }

    public void DecreaseDP(int _num)
    {
        currentDp -= _num;

        if (currentDp <= 0){
            currentHp -= currentDp;
            currentDp = 0;
        }
    }

    public void IncreaseHUNGRY(int _num)
    {
        currentHungry += _num;
        
        if (currentHungry >= hungry){
            currentHungry = hungry;
        }
    }

    public void IncreaseTHIRSTY(int _num)
    {
        currentThirsty += _num;
        
        if (currentThirsty >= thirsty){
            currentThirsty = thirsty;
        }
    }
}
