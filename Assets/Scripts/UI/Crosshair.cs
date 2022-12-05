using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    // 크로스 헤어 애니메이터
    [SerializeField]
    private Animator anim;

    // 크로스헤어 상태에 따른 총의 정확도.
    private float gunAccuracy;

    // 크로스헤어 비활성화를 위한 부모 객체
    [SerializeField]
    private GameObject go_CrosshairHUD;
    [SerializeField]
    private GunController theGunController;

    public void WalkingAnimation(bool _flag)
    {
        if (!GameManager.isWater){
            WeaponManager.currentWeaponAnim.SetBool("Walk", _flag);
            anim.SetBool("Walking", _flag);
        }
    }

    public void RuningAnimation(bool _flag)
    {
        if (!GameManager.isWater){
            WeaponManager.currentWeaponAnim.SetBool("Run", _flag);
            anim.SetBool("Runing", _flag);
        }
    }

    public void JumpingAnimation(bool _flag)
    {
        if (!GameManager.isWater)
            anim.SetBool("Runing", _flag);
    }

    public void CrouchingAnimation(bool _flag)
    {
        if (!GameManager.isWater)
            anim.SetBool("Crouching", _flag);
    }

    public void FineSightAnimation(bool _flag)
    {
        if (!GameManager.isWater)
            anim.SetBool("FireSight", _flag);
    }

    public void FireAnimation()
    {
        if (!GameManager.isWater){
            if (anim.GetBool("Walking"))
                anim.SetTrigger("Walk_Fire");
            else if (anim.GetBool("Crouching"))
                anim.SetTrigger("Crouch_Fire");
            else
                anim.SetTrigger("Idle_Fire");
        }
    }

    public float GetAccuracy()
    {
        if (anim.GetBool("Walking"))
            gunAccuracy = 0.06f;
        else if (anim.GetBool("Crouching"))
            gunAccuracy = 0.015f;
        else if (theGunController.GetFineSightMode())
            gunAccuracy = 0.001f;
        else
            gunAccuracy = 0.035f;

        return gunAccuracy;
    }
}
