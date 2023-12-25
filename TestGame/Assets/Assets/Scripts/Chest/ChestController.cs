using System.Collections;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public GameObject[] itemsToSpawn; // ������ ���������, ������� ����� ������� �� �������
    private bool isOpen = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpen)
        {
            SpawnItems(); // �������� ������� �������� ���������
            isOpen = true; // �������� ������ ��� ��������
            Destroy(gameObject); // ���������� ������, ��� ��� �� ������ �� �����
        }
    }

    private void SpawnItems()
    {
        // ������� ��������� ������� �� ������� � ��������� ��� ����� ��������
        if (itemsToSpawn.Length > 0)
        {
            int randomItemIndex = Random.Range(0, itemsToSpawn.Length);

            // Instantiate the prefab
            GameObject spawnedItem = Instantiate(itemsToSpawn[randomItemIndex], transform.position + Vector3.up, Quaternion.identity);

            // Optional: If your prefab has a parent object, you can set it here
            // spawnedItem.transform.parent = transform;

            // Optional: If your prefab has a script attached, you can access it like this
            // Example: spawnedItem.GetComponent<YourItemScript>().Initialize(); 

            // Optional: If you want to destroy the spawned item after a certain time
            // Destroy(spawnedItem, 10f); // Destroy after 10 seconds (adjust as needed)
        }
    }

}
