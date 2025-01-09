using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_2_OpenDoors : BaseOpenDoor
{
    public float duration = 3f;


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
            door.transform.position = Vector3.Lerp(new Vector3(door.transform.position.x, door.transform.position.y, door.transform.position.z), new Vector3(door.transform.position.x, -1.5f, door.transform.position.z), time / duration);
            yield return null; // Po�k�me na dal�� sn�mek
        }

        // Zajist�me p�esn� nastaven� c�lov� rotace
        door.transform.position = new Vector3(door.transform.position.x, -1.5f, door.transform.position.z);
    }
}
