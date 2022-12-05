using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField] private float viewAngle;       // 시야각      (120도)
    [SerializeField] private float viewDisatance;   // 시야 거리   (10m)
    [SerializeField] private LayerMask targetMask;  // 타겟 마스크 (Player)

    private Pig thePig;

    void Start() {
        thePig = GetComponent<Pig>();
    }

    void Update()
    {
        View();
    }

    // 시야각 구하기
    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }

    private void View()
    {
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f); 
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red);
        Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.red);

        // 해당 반경 안에 targetMask 존재 시 전체 다 가져옴
        Collider[] _target = Physics.OverlapSphere(transform.position, viewDisatance, targetMask);

        for (int i = 0; i < _target.Length; i++){
            
            Transform _targetTf = _target[i].transform;

            if (_targetTf.name == "Player"){
                
                // 두 오브젝트 사이 거리 구하기 (normalized는 정규화)
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                Debug.DrawRay(transform.position + (transform.up*2f), _direction, Color.yellow);

                // 현 객체의 forward를 기준으로 _direction 위치와의 각을 구함.
                float _angle = Vector3.Angle(_direction, transform.forward);

                // 구해진 각과 (viewAngle(130)/2)=65 값을 비교.
                if (_angle < viewAngle * 0.5f){
                    
                    RaycastHit _hit;
                    
                    if (Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDisatance)){
                        if (_hit.transform.name == "Player"){
                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);
                            thePig.Run(_hit.transform.position);
                        }
                    }
                }
            }
        }
    }
}
