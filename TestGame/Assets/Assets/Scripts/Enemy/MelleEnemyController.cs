using UnityEngine;
using UnityEngine.AI;

public class test : MonoBehaviour
{
    [SerializeField] float chaseRadius = 10f; // ������ ������������� ������
    [SerializeField] float walkRadius = 5f;   // ������ ���������� ���������
    [SerializeField] float moveSpeed = 3f; // �������� ��������
    [SerializeField] string wallTag = "Walls"; // ��� ��������, ������� ��������� �������.

    private Transform player;                  // ������ �� ������
    private NavMeshAgent agent;                // ��������� NavMeshAgent
    private Vector3 randomDestination;         // ��������� ����� ����������
    private bool isChasing = false;            // ���� ��� ������������ ��������� �������������

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // ������������� �������� ��������
        agent.speed = moveSpeed;

        // ������� ������ �� ���� "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // ��������� ��������� ���������
        SetNewRandomDestination();
    }

    private void Update()
    {
        if (player != null)
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
            else if (!isChasing && Vector3.Distance(transform.position, randomDestination) < 0.3f)
            {
                // ���� �� ���������� ������ � �������� ��������� ����� ����������, ������������� �����
                SetNewRandomDestination();
            }
        }
        else
        {
            isChasing = false;
        }
    }

    private void SetNewRandomDestination()
    {
        // ����� ���������� ����� ����� ����������, ���������, ��� �� ����� �������.
        Vector3 newPosition = RandomNavMeshLocation();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, newPosition - transform.position, out hit, Vector3.Distance(transform.position, newPosition)) &&
            hit.collider.CompareTag(wallTag))
        {
            // ���� ���� �����, ���������� ����� ����� ��������� �����.
            Invoke("SetNewRandomDestination", Random.Range(1f, 3f));
        }
        else
        {
            randomDestination = newPosition;
            agent.SetDestination(randomDestination);
        }
    }

    private Vector3 RandomNavMeshLocation()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);

        return hit.position;
    }
}
