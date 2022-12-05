using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 스피드 조정 변수
    [SerializeField]
    private float walkSpeed;            // 걷기 속도
    [SerializeField]
    private float runSpeed;             // 달리기 속도
    [SerializeField]
    private float crouchSpeed;          // 앉을 때 속도
    [SerializeField]
    private float swimSpeed;
    [SerializeField]
    private float swimFastSpeed;
    [SerializeField]
    private float upSwimSpeed;
    private float applySpeed;           // 현재 속도

    [SerializeField]
    private float jumpForce;            // 점프할 높이

    // 상태 변수
    private bool isWalk = false;
    private bool isRun = false;         // 뛰고 있는가?
    private bool isCrouch = false;      // 앉고 있는가?
    private bool isGround = true;       // 지면과 닿는가?

    // 움직임 체크 변수
    private Vector3 lastPos;

    // 앉았을 때 얼마나 앉을지 결정하는 변수.
    [SerializeField]
    private float crouchPosY;           // 앉을 높이(Y)
    private float originPosY;           // 일어서 있을 때 높이
    private float applyCrouchPosY;      // 현재 높이

    // 땅 착지 여부
    private CapsuleCollider capsuleCollider;

    // 카메라와 캐릭터의 마우스 회전 민감도
    [SerializeField]
    private float lookSensitivity;

    // 카메라 한계
    [SerializeField]
    private float cameraRotationLimit;          // 고개를 돌릴 때 최대 각도
    private float currentCameraRotationX = 0;   // 현재 카메라 회전값

    // 필요한 컴포넌트
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;
    private GunController theGunController;
    private Crosshair theCrosshair;
    private StatusController theStatusController;
    
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        theGunController = FindObjectOfType<GunController>();
        theCrosshair = FindObjectOfType<Crosshair>();
        theStatusController = FindObjectOfType<StatusController>();

        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    void Update()
    {
        if (GameManager.canPlayerMove){
            IsGround();             // 지면 체크
            TryJump();              // 점프
            TryCrouch();            // 앉기
            Move();                 // 움직이기
            CameraRotation();       // 캐릭터 좌우 회전
            CharacterRotation();    // 카메라 상하 회전

            if (!GameManager.isWater){
                TryRun();               // 뛰기
            }
            else{
                WaterCheck();           // 물 속에서 뛰기
            }
        }
    }

    private void WaterCheck()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            applySpeed = swimFastSpeed;
        else
            applySpeed = swimSpeed;
    }

    void FixedUpdate() {
        MoveCheck();
    }

    // 앉기 시도
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGround){
            Crouch();
        }
    }

    // 앉기 동작
    private void Crouch()
    {
        isCrouch = !isCrouch;
        theCrosshair.CrouchingAnimation(isCrouch);

        if (isCrouch){
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else{
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());
    }

    // 부드러운 앉기 동작 실행
    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while(_posY != applyCrouchPosY){
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 15)
                break;
            yield return null;  // 한 프레임 쉼
        }
        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0);
    }

    // 지면 체크
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
        theCrosshair.JumpingAnimation(!isGround);
    }

    // 점프 시도
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround && theStatusController.GetStamina() && !GameManager.isWater)
            Jump();
        else if (Input.GetKey(KeyCode.Space) && GameManager.isWater)
            UpSwim();
    }

    private void UpSwim()
    {
        myRigid.velocity = transform.up * upSwimSpeed;
    }

    // 점프 동작
    private void Jump()
    {
        // 앉은 상태에서 점프 시 앉기 해제
        if (isCrouch)
            Crouch();
        theStatusController.DecreaseStamina(100);
        myRigid.velocity = transform.up * jumpForce;
    }

    // 달리기 시도
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            Running();

        if (Input.GetKeyUp(KeyCode.LeftShift))
            RunningCancel();
    }

    // 달리기 실행
    private void Running()
    {
        if (!theStatusController.GetStamina())
        {
            RunningCancel();
            return;
        }

        // 앉은 상태에서 뛸 시 앉기 해제
        if (isCrouch)
            Crouch();
        
        theGunController.CancelFineSight(); // 정조준일 경우 해제
    
        isRun = true;
        theCrosshair.RuningAnimation(isRun);
        theStatusController.DecreaseStamina(5);
        applySpeed = runSpeed;
    }

    // 달리기 취소
    private void RunningCancel()
    {
        isRun = false;
        theCrosshair.RuningAnimation(isRun);
        applySpeed = walkSpeed;
    }

    // 움직임 실행
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    // 움직임 체크
    private void MoveCheck()
    {
        bool moveCheck = Input.GetButton("Horizontal") || Input.GetButton("Vertical");
        bool intervalPos = Vector3.Distance(lastPos, transform.position) >= 0.0001f;

        if (!isRun && !isCrouch && isGround){
            if (intervalPos || moveCheck)
                isWalk = true;
            else
                isWalk = false;
            
            theCrosshair.WalkingAnimation(isWalk);
            
            lastPos = transform.position;
        }
    }

    // 좌우 캐릭터 회전
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        // Quaternion.Euler(Vector3의 rotation 값) <- 해주면 쿼터니언 값으로 바꿔줌.
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY)); // 이거 말고 다른 방법 사용해도 됨.
    }

    // 상하 카메라 회전
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        // Mathf.Clamp(제안시킬 변수, 최소값, 최대값) -> (a, -30, 30)일 경우 a는 -30 ~ 30 값 안에서 사용 가능
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
}
