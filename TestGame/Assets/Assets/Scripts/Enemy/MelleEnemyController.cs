using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class test : MonoBehaviour
{
    [SerializeField] private float entityDamage = 2f;
    [SerializeField] private float damageInterval = 2f; // �������� ����� �������
    [SerializeField] float chaseRadius = 10f; 
    [SerializeField] float walkRadius = 5f;   
    [SerializeField] float moveSpeed = 3f;   
    [SerializeField] string wallTag = "Walls";

    private bool isCooldown = false;             
    private float fleeCooldownDuration = 2f;     
    private bool hasNewOppositePoint = false;
    private bool isFleeing = false;               // ���� ��� ������������ ��������� ��������
    private float lastDamageTime;                 // ����� ���������� �����
    private Transform player;                     // ������ �� ������
    private NavMeshAgent agent;                   // ��������� NavMeshAgent
    private Vector3 randomDestination;            // ��������� ����� ����������
    private bool isChasing = false;               // ���� ��� ������������ ��������� �������������
    private bool shouldFollowPlayer = false;      // ���� ��� �����������, ��������� �� �� ������� ����� ���������� ��������� �����

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
        StartCoroutine(RandomDestinationRoutine());
    }


    private void Update()
    {
        if (player != null)
        {
            // ���� �������� ����� ��������� ����� � �� ���������� ������, ������������ � �������������
            if (!agent.pathPending && agent.remainingDistance < 0.5f && !isChasing)
            {
                isChasing = true;
                shouldFollowPlayer = true;

                // ���� �� �������, ������������� ����� � ������
                if (!isFleeing)
                {
                    agent.SetDestination(player.position);
                }
            }

            // ���� ���������� �� ������� ���������, ������������� ����� � ������
            if (shouldFollowPlayer)
            {
                agent.SetDestination(player.position);
            }

            // ���� ������������� ������������ � ����� �� � �������, ������������ � ���������� ���������
            if (isChasing && Vector3.Distance(transform.position, player.position) > chaseRadius)
            {
                isChasing = false;
                shouldFollowPlayer = false;
                SetNewOppositePoint();
                agent.SetDestination(randomDestination);

                // ���������� ����, ����� ����� ���� ���������� ����� ����������� ��� ��������� ������������
                hasNewOppositePoint = false;
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

    private IEnumerator RandomDestinationRoutine()
    {
        while (true)
        {
            if (!isFleeing)
            {
                SetNewRandomDestination();
            }

            // ���� 2 �������
            yield return new WaitForSeconds(2f);
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

    private void OnCollisionStay2D(Collision2D other)
    {
        string entityTag = other.gameObject.tag;

        // Check if it's time to flee again
        if (!isCooldown && Time.time - lastDamageTime >= damageInterval)
        {
            Health health = other.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.Reduce((int)entityDamage, health.currentHealth);
                
                // Update the time of the last damage
                lastDamageTime = Time.time;

                // Set the flag to start fleeing
                isFleeing = true;

                // Set a cooldown period during which the enemy won't immediately switch back to chasing
                StartCoroutine(FleeCooldown());

                // Set a new destination away from the player
                SetNewOppositePoint();
                isChasing = false;
                shouldFollowPlayer = false;
                agent.SetDestination(randomDestination);
            }
        }
    }

    private IEnumerator FleeCooldown()
    {
        // Set the cooldown flag to true
        isCooldown = true;

        // Wait for the cooldown duration
        yield return new WaitForSeconds(fleeCooldownDuration);

        // Reset the cooldown flag
        isCooldown = false;
    }

    private void SetNewOppositePoint()
    {
        // Check if the player is nearby
        if (Vector3.Distance(transform.position, player.position) <= chaseRadius)
        {
            // Determine the vector from the player to the current enemy position
            Vector3 directionToPlayer = transform.position - player.position;

            // Normalize the vector and multiply by walkRadius to get a point in the opposite direction with the specified radius
            Vector3 oppositePoint = transform.position + directionToPlayer.normalized * walkRadius;

            // Check for walls in the chosen direction
            RaycastHit hit;
            if (Physics.Raycast(transform.position, oppositePoint - transform.position, out hit, walkRadius) &&
                hit.collider.CompareTag(wallTag))
            {
                // If there is a wall, try again after a random delay
                Invoke("SetNewOppositePoint", Random.Range(1f, 3f));
            }
            else
            {
                randomDestination = oppositePoint;
            }
        }
        else
        {
            // Player is not nearby, choose a random destination within the walkRadius
            randomDestination = RandomNavMeshLocation();
        }
    }

}