using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twig : MonoBehaviour
{
    [SerializeField]
    private int hp;  // ���� ���� ü��. 0 �� �Ǹ� �ı�.

    [SerializeField]
    private float destroyTime;  // ���� ���� ����Ʈ (��ƼŬ �ý���) ���� �ð�

    [SerializeField]
    private GameObject go_little_Twig;  // `Little_Twig` �Ҵ�. ���������� �ı��� �� �� ��������. �� ���� �������� ������.
    [SerializeField]
    private GameObject go_twig_hit_effect_prefab;  // `Leaf_Hit_Effect` ���� ���� ���� �� ������ ����Ʈ ������

    [SerializeField]
    private float force;  // ������ �� ���� ���� ���������� �о��� ���� ũ��

    /* ȸ���� ���� */
    private Vector3 originRot;   // ���� ���� ���� ȸ�� ��. (���� ���� ������ ����̰� �� ���̶� ���߿� ������� ���� �� �� �ʿ�)
    private Vector3 wantedRot;   // ���� ���� ���� �� ȸ�� �Ǳ� ���ϴ� ��.
    private Vector3 currentRot;  // wanted_Rot �� �Ǳ� ���� ��� �����س��� ȸ�� ��

    /* �ʿ��� ���� �̸�.  (����� SoundManager.cs �̱������� �ϴϱ� �� �̸� string�� �˸� ��) */
    [SerializeField]
    private string hit_Sound;
    [SerializeField]
    private string broken_Sound;

    void Start()
    {
        originRot = transform.rotation.eulerAngles;  // ���� ���ϰ� Vector3 ��.
        currentRot = originRot;  // currentRot �ʱⰪ
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
        Vector3 direction = (_target.position - transform.position).normalized; // �÷��̾� �������� �� ���ϴ� ���� 

        Vector3 rotationDir = Quaternion.LookRotation(direction).eulerAngles;  // �÷��̾� �������� ������ �ٶ󺸴� ������ ���� ��.

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

    private void CheckDirection(Vector3 _rotationDir)  // ��� �������� ���� ������ ������.
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
