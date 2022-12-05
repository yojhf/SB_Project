using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public static bool isActivate = false;

    // 현재 장착된 총
    [SerializeField]
    private Gun currentGun;
    
    // 연사 속도 계산
    private float currentFireRate;

    // 상태 변수
    private bool isReload;
    [HideInInspector]
    private bool isFineSightMode = false;

    public bool isShoot = false;
    
    // 원래 포지션 값
    private Vector3 originPos;  // 기본 0 0 0 값

    // 효과음
    private AudioSource audioSource;

    // 레이저 충돌 정보 받아옴.
    private RaycastHit hitInfo;

    [SerializeField]
    private LayerMask layerMask;

    // 필요한 컴포넌트
    [SerializeField]
    private Camera theCam;

    [SerializeField]
    private GameObject hitEffectPrefab;
    private Crosshair theCrosshair;

    void Start() {
        theCrosshair = FindObjectOfType<Crosshair>();
        audioSource = GetComponent<AudioSource>();
        originPos = Vector3.zero;


    }

    void Update()
    {
        if (!Inventory.inventoryActivated){
            if (isActivate){
                GunFireRateCalc();
                TryFire();
                TryReload();
                TryFineSight();
            }
        }
        
    }

    // 연사속도 재계산
    private void GunFireRateCalc()
    {
        if (currentFireRate > 0){
            currentFireRate -= Time.deltaTime;
        }
    }

    // 발사 시도
    private void TryFire()
    {
        if (Input.GetButton("Fire1") && currentFireRate <= 0f && !isReload){
            isShoot = true;
            Fire();
        }
        else
            isShoot = false;
    }

    // 발사 전 계산
    private void Fire()
    {
        if (!isReload){
            if (currentGun.currentBulletCount > 0)
                Shoot();
            else{
                CancelFineSight();
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    // 발사 후 계산
    private void Shoot()
    {
        theCrosshair.FireAnimation();
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate;
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();
        Hit();
        StopAllCoroutines();                    // 반동 중첩 방지
        StartCoroutine(RetroActionCoroutine()); // 반동 코루틴
    }

    // rotation x 좌표는 마우스 커서에 의해 업데이트로 계속 저장되므로 반동을 주려면 커서를 고정시켜야할 듯하다.
    // private void GunShootCameraRecoil()
    // {
    //     theCam.transform.localEulerAngles -= new Vector3(1.5f, 0f, 0f);
    // }

    // 총에 맞은 객체 확인
    private void Hit()
    {
        if (Physics.Raycast(theCam.transform.position, theCam.transform.forward + 
            new Vector3(Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy),
                        Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy),
                        0f),
            out hitInfo, currentGun.range, layerMask)){
            // .point = 충돌한 곳에 실제 좌표를 반환한다.
            // .normal = 충돌한 객체의 표면을 반환한다.
            // Quaternion.LookRotation() 특정한 객체를 바라본다.
            // var = 반환되는 타입을 모를때 쓴다.
            GameObject clone = Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            if (hitInfo.transform.CompareTag("WeakAnimal"))
                hitInfo.transform.GetComponent<WeakAnimal>().Damage(1, transform.position);
            Destroy(clone, 1f);
        }
    }

    // 재장전 시도
    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount){
            CancelFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }

    public void CancelReload()
    {
        if (isReload){
            StopAllCoroutines();
            isReload = false;
        }
    }

    // 재장전
    IEnumerator ReloadCoroutine()
    {
        if (currentGun.carryBulletCount > 0){
            isReload = true;
            currentGun.anim.SetTrigger("Reload");

            currentGun.carryBulletCount += currentGun.currentBulletCount;
            currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime);

            if (currentGun.carryBulletCount >= currentGun.reloadBulletCount){
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else{
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }

            isReload = false;
        }
        else{
            Debug.Log("소유한 총알이 없습니다.");
            // 총알 없는 소리 넣어도 됨
        }
    }

    // 정조준 시도
    private void TryFineSight()
    {
        if (Input.GetButtonDown("Fire2") && !isReload){
            FineSight();
        }
    }

    // 정조준 취소
    public void CancelFineSight()
    {
        if (isFineSightMode)
            FineSight();
    }

    // 정조준 로직 가동
    private void FineSight()
    {
        isFineSightMode = !isFineSightMode;
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);  // 총 정조준 애니메이션
        theCrosshair.FineSightAnimation(isFineSightMode);           // 크로스 헤어 애니메이션

        if (isFineSightMode){
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
        }
        else{
            StopAllCoroutines();
            StartCoroutine(FineSightDeactivateCoroutine());
        }
    }

    // 정조준 활성화
    IEnumerator FineSightActivateCoroutine()
    {
        while (currentGun.transform.localPosition != currentGun.fineSightOriginPos){
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return null;
        }
    }

    // 정조준 비활성화
    IEnumerator FineSightDeactivateCoroutine()
    {
        while (currentGun.transform.localPosition != originPos){
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }

    // 반동 코루틴
    IEnumerator RetroActionCoroutine()
    {
        // 반동을 줄 위치 정하기
        Vector3 recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z);
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);

        // 정조준이 아닐 때
        if (!isFineSightMode){
            currentGun.transform.localPosition = originPos;

            // 반동 시작
            while(currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f){
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }

            // 원위치
            while(currentGun.transform.localPosition != originPos){
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else{
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;

            // 반동 시작
            while(currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f){
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            // 원위치
            while(currentGun.transform.localPosition != currentGun.fineSightOriginPos){
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.1f);
                yield return null;
            }
        }
    }

    // 사운드 재생
    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public Gun GetGun()
    {
        return currentGun;
    }

    public bool GetFineSightMode()
    {
        return isFineSightMode;
    }

    public void GunChange(Gun _gun)
    {
        if (WeaponManager.currentWeapon != null){
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }

        currentGun = _gun;
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentGun.anim;

        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);

        isActivate = true;
    }
}
