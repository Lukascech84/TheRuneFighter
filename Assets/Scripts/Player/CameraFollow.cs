using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // C�l, kter� kamera sleduje
    public float smoothTime = 0.3f; // �as hladk�ho p�echodu
    public Vector3 offset; // V�choz� offset kamery
    private Vector3 velocity = Vector3.zero; // Rychlost kamery pro SmoothDamp

    public float mouseInfluence = 2.0f; // Jak moc my� ovlivn� pozici kamery
    public Vector2 mouseOffsetLimit = new Vector2(5f, 5f); // Maxim�ln� vych�len� kamery od offsetu

    void Update()
    {
        if (target != null)
        {
            // Z�kladn� pozice kamery podle c�le a offsetu
            Vector3 targetPosition = target.position + offset;

            // Z�sk�n� pozice my�i relativn� ke st�edu obrazovky
            Vector2 mousePosition = Input.mousePosition;
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 mouseDelta = (mousePosition - screenCenter) / screenCenter; // Normalizov�no na hodnoty od -1 do 1

            // Upraven� vych�len� kamery na z�klad� pozice my�i
            Vector3 mouseOffset = new Vector3(
                Mathf.Clamp(mouseDelta.x * mouseInfluence, -mouseOffsetLimit.x, mouseOffsetLimit.x),
                0, // Neposouv�me kameru vertik�ln�
                Mathf.Clamp(mouseDelta.y * mouseInfluence, -mouseOffsetLimit.y, mouseOffsetLimit.y)
            );

            // P�id�n� vych�len� my�i k c�lov� pozici kamery
            targetPosition += mouseOffset;

            // Plynul� p�echod na novou pozici
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
