using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 무기의 기본 구성 (부모 클래스)
public abstract class CloseWeaponController : MonoBehaviour
{
    // 현재 장착된 Hand형 타입 무기.
    [SerializeField]
    protected CloseWeapon currentCloseWeapon;
    
    // 공격중?
    protected bool isAttack = false;
    protected bool isSwing = false;

    protected RaycastHit hitInfo;

    [SerializeField]
    protected LayerMask layerMask;

    protected PlayerController thePlayerController;

    // 공격 여부 확인
    protected void TryAttack()
    {
        if (!Inventory.inventoryActivated)
        {
            if (Input.GetButton("Fire1"))
            {
                if (!isAttack)
                {
                    if (CheckObject())
                    {
                        if (currentCloseWeapon.isAxe && hitInfo.transform.tag == "Tree")
                        {
                            StartCoroutine(thePlayerController.TreeLookCoroutine(hitInfo.transform.GetComponent<TreeComponent>().GetTreeCenterPosition()));
                            StartCoroutine(AttackCoroutine("Chop", currentCloseWeapon.workDelayA, currentCloseWeapon.workDelayB, currentCloseWeapon.workDelay));
                            return;
                        }
                    }

                    // 코루틴 실행
                    StartCoroutine(AttackCoroutine("Attack", currentCloseWeapon.attackDelayA, currentCloseWeapon.attackDelayB, currentCloseWeapon.attackDelay));
                }
            }    
        }
        
    }

    // 공격 진행 코루틴
    protected IEnumerator AttackCoroutine(string swingType, float _delayA, float _delayB, float _delay)
    {
        isAttack = true;

        currentCloseWeapon.anim.SetTrigger("Attack");

        yield return new WaitForSeconds(currentCloseWeapon.attackDelayA);
        isSwing = true;

        // 공격 활성화 시점.
        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(currentCloseWeapon.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentCloseWeapon.attackDelay - currentCloseWeapon.attackDelayA - currentCloseWeapon.attackDelayB);



        // work
        currentCloseWeapon.anim.SetTrigger(swingType);

        yield return new WaitForSeconds(_delayA);
        isSwing = true;

        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(_delayB);
        isSwing = false;

        yield return new WaitForSeconds(_delay - _delayA - _delayB);

        isAttack = false;
    }

    // 피격 여부 코루틴 (추상 코루틴)
    protected abstract IEnumerator HitCoroutine();

    // Raycast를 통한 피격 확인
    protected bool CheckObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range, layerMask)){
            return true;
        }
        return false;
    }

    // 완성 함수이지만, 추가 편집한 함수.
    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        if (WeaponManager.currentWeapon != null){
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }

        currentCloseWeapon = _closeWeapon;
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;

        currentCloseWeapon.transform.localPosition = Vector3.zero;
        currentCloseWeapon.gameObject.SetActive(true);
    }
}
