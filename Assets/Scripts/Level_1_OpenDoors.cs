using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_1_OpenDoors : BaseOpenDoor
{
    public float duration = 2f;


    public override void OpenDoor()
    {
        StartCoroutine(Doors());
    }

    private IEnumerator Doors()
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            // Interpolace mezi po��te�n� a c�lovou rotac�
            door.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 130, 0), time / duration);
            yield return null; // Po�k�me na dal�� sn�mek
        }

        // Zajist�me p�esn� nastaven� c�lov� rotace
        door.transform.rotation = Quaternion.Euler(0, 130, 0);
    }
}
