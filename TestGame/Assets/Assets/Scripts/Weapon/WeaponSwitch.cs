using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    private Sword[] swords;

    public int selectedWeapon = 0;

    private void Start()
    {
        swords = GetComponentsInChildren<Sword>();
        SelectWeapon();
    }

    private void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            selectedWeapon = 1;
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    private void SelectWeapon()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == selectedWeapon)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(i).gameObject.SetActive(false);
                // �������� ����� ��� ���� �����, ������� ���� ���������
                if (swords != null && i < swords.Length)
                {
                    swords[i].CancelAttack();
                }
            }
        }
    }
}
