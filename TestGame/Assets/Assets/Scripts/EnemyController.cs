using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public string playerTag = "Player"; // ��� ������
    public float detectionRadius = 5f; // ������ ����������� ������
    public float moveSpeed = 2f; // �������� �������� slime

    private Transform player;
    private bool isPlayerInRange = false;

    private void Start()
    {
        // ����� ������ �� ���� ��� ������
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
    }

    private void Update()
    {
        if (player == null)
        {
            // ����� �� ������, �������
            return;
        }

        // ��������� ���������� ����� slime � �������
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // ���� ����� ��������� � ������� �����������
        if (distanceToPlayer <= detectionRadius)
        {
            isPlayerInRange = true;
        }
        else
        {
            isPlayerInRange = false;
        }

        // ���� ����� � ������� �����������, ������� slime � ������
        if (isPlayerInRange)
        {
            // ��������� ����������� �������� � ������
            Vector2 direction = (player.position - transform.position).normalized;

            // ������� slime � ����������� ������
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }
}
