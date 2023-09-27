using UnityEngine;
using UnityEngine.AI;

public class test : MonoBehaviour
{
    [SerializeField] float chaseRadius = 10f; // ������ ������������� ������
    [SerializeField] float walkRadius = 5f;   // ������ ���������� ���������
    [SerializeField] string wallTag = "Walls"; // ��� ��������, ������� ��������� �������.

    private Transform player;                  // ������ �� ������
    private NavMeshAgent agent;                // ��������� NavMeshAgent
    private Vector3 randomDestination;         // ��������� ����� ����������
    private bool isChasing = false;            // ���� ��� ������������ ��������� �������������

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // ������� ������ �� ���� "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // ��������� ��������� ���������
        SetNewRandomDestination();
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ���� ���������� �� ������ ������ ������� �������������, ���������� ������
        if (distanceToPlayer <= chaseRadius)
        {
            isChasing = true;
            agent.SetDestination(player.position);
        }
        else if (isChasing)
        {
            // ���� ������������� ���� ������������, �� ����� ����� �� �������, �������� � ��������� ���������
            isChasing = false;
            SetNewRandomDestination();
        }
        else if (!isChasing && Vector3.Distance(transform.position, randomDestination) < 0.1f)
        {
            // ���� �� ���������� ������ � �������� ��������� ����� ����������, ������������� �����
            SetNewRandomDestination();
        }
    }

    private void SetNewRandomDestination()
    {
        // ���������� ��������� ����� � ������� ���������
        randomDestination = RandomNavMeshLocation(walkRadius);

        // ���������, ��� �� ����� �������
        RaycastHit hit;
        if (Physics.Raycast(transform.position, randomDestination - transform.position, out hit, Vector3.Distance(transform.position, randomDestination)) &&
            hit.collider.CompareTag(wallTag))
        {
            // ���� ���� �����, ���������� ����� ����� ��������� �����.
            Invoke("SetNewRandomDestination", Random.Range(1f, 3f));
        }
        else
        {
            agent.SetDestination(randomDestination);
        }
    }

    private Vector3 RandomNavMeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, 1);

        return hit.position;
    }
}
