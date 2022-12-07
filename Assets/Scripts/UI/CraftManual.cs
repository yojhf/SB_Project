using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 생성 가능한 건축물
[System.Serializable]
public class Craft
{
    public string craftName;            // 이름
    public GameObject go_Prefab;        // 실제 설치될 프리팹
    public GameObject go_PreviewPrefab; // 미리보기 프리팹
}

public class CraftManual : MonoBehaviour
{
    // 상태변수
    private bool isActivated = false;           // 탭이 열렸는가?
    private bool isPreviewActivated = false;    // 프리뷰가 생성되었는가?

    [SerializeField]
    private GameObject go_BaseUI;   // 기본 베이스

    [SerializeField]
    private Craft[] craft_fire;     // 모닥불용 탭


    private GameObject go_Preview;  // 미리보기 프리팹을 담을 변수
    private GameObject go_Prefab;   // 실제 생성될 프리팹을 담을 변수

    [SerializeField]
    private Transform tf_Player;    // 플레이어 시점 정보 (Camera)

    // Raycast 변수
    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;

    //버튼 프리뷰 각도 초기화
    private bool new_angle_check = false;
    private int new_angle_y;
    private int new_angle_x;

    // 건축슬롯 클릭 버튼
    public void SlotClick(int _slotNumber)
    {
        go_Preview = Instantiate(craft_fire[_slotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        go_Prefab = craft_fire[_slotNumber].go_Prefab;

        //go_Preview = Instantiate(craft_wall[_slotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        //go_Prefab = craft_wall[_slotNumber].go_Prefab;

        GameManager.isOpenCraftManual = false;
        isPreviewActivated = true;
        go_BaseUI.SetActive(false);
    }

    void Update()
    {
        // 건축 메뉴 활성화
        if (Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
            Window();

        // 좌클릭 누르면 건축물 생성
        if (Input.GetButtonDown("Fire1"))
            Build();

        // 초기화
        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();

        // 프리뷰가 생성 되었을 때
        if (isPreviewActivated)
            PreviewPositionUpdate();
    }

    // 프리뷰 위치에 프리팹 생성
    private void Build()
    {
        if (isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isBuildable())
        {
            Instantiate(go_Prefab, hitInfo.point, go_Preview.transform.rotation);
            Destroy(go_Preview);
            isActivated = false;
            isPreviewActivated = false;
            go_Prefab = null;
            go_Preview = null;
            new_angle_check = false;
            new_angle_x = 0;
            new_angle_y = 0;
        }
    }

    // 플레이어가 바라보는 위치에 건축 프리뷰 생성
    private void PreviewPositionUpdate()
    {
        if (Physics.Raycast(tf_Player.position, tf_Player.forward, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform != null)
            {
                Vector3 _location = hitInfo.point; // 레이저 쏴서 맞은 곳의 좌표 반환
                new_angle_check = true;

                if (Input.GetKeyDown(KeyCode.Q) && new_angle_check == true)
                {
                    new_angle_y += 90;
                    go_Preview.transform.eulerAngles = new Vector3(new_angle_x, new_angle_y, 0);
                }
                else if (Input.GetKeyDown(KeyCode.E) && new_angle_check == true)
                {
                    new_angle_x -= 45;
                    go_Preview.transform.eulerAngles = new Vector3(new_angle_x, new_angle_y, 0);
                }

                // _location.Set(Mathf.Round(_location.x), Mathf.Round(_location.y / 0.1f) * 0.1f, Mathf.Round(_location.z));
                go_Preview.transform.position = _location;
            }
        }
    }

    // 초기화
    private void Cancel()
    {
        if (isPreviewActivated)
            Destroy(go_Preview);

        GameManager.isOpenCraftManual = false;
        isActivated = false;
        isPreviewActivated = false;
        go_Preview = null;
        go_Prefab = null;
        go_BaseUI.SetActive(false);
        new_angle_x = 0;
        new_angle_y = 0;
    }

    // 건축 메뉴 활성화/비활성화
    private void Window()
    {
        if (!isActivated)
            OpenWindow();
        else
            CloseWindow();
    }

    private void OpenWindow()
    {
        GameManager.isOpenCraftManual = true;
        isActivated = true;
        go_BaseUI.SetActive(true);
    }

    private void CloseWindow()
    {
        GameManager.isOpenCraftManual = false;
        isActivated = false;
        go_BaseUI.SetActive(false);
    }
}
