using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Type type;

    public enum Type  // ���๰ Ÿ�� ����
    {
        Normal, // ���๰�� �ƴ� �͵�
        Wall,   // ��
        Foundation, // ���
        Pillar  // ���
    }
}