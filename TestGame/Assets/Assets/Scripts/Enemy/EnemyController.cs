using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float chaseRadius = 10f; // ������ ������������� ������
    [SerializeField] float wanderRadius = 5f; // ������ ���������
    [SerializeField] LayerMask wallLayer; // ���� ��� ����

    NavMeshAgent agent;
    Vector3 startPosition;
    bool isWandering = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        startPosition = transform.position;

        // ��������� �������� ��� ���������
        StartCoroutine(Wander());
    }

    private void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // ���� ���������� �� ������ ������ ������� �������������, ��������� � ������
        if (distanceToTarget <= chaseRadius)
        {
            agent.SetDestination(target.position);
            isWandering = false;
        }
    }

    private IEnumerator Wander()
    {
        while (true)
        {
            // ���������� ��������, ���� �� ���������� ������
            if (!isWandering)
            {
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection += startPosition;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
                Vector3 finalPosition = hit.position;
                agent.SetDestination(finalPosition);
                isWandering = true;
            }

            // ��������� ��������� ����� ����� ��������� ���������� (��������, 2 �������)
            yield return new WaitForSeconds(2f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ���� ���� ���������� �� ������, ������ �����������
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector3 newDirection = Vector3.Reflect(agent.velocity.normalized, collision.contacts[0].normal);
            agent.SetDestination(transform.position + newDirection * 2f); // ��������� ��������, ����� �������� ������������
        }
    }
}
