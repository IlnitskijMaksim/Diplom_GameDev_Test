using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDamage : MonoBehaviour
{
    [SerializeField] private float entityDamage;
    [SerializeField] private float damageInterval = 2f; // �������� ����� �������
    private float lastDamageTime; // ����� ���������� �����

    private void OnCollisionStay2D(Collision2D collision)
    {
        string entityTag = collision.gameObject.tag;

        // ���������, ������ �� ���������� ������� � ������� ���������� �����
        if (Time.time - lastDamageTime >= damageInterval)
        {
            PlayerStats health = collision.gameObject.GetComponent<PlayerStats>();
            if (health != null)
            {
                health.GiveDamage(entityDamage);

                // ��������� ����� ���������� �����
                lastDamageTime = Time.time;
            }
        }
    }
}
