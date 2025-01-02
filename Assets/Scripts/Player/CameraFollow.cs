using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Cíl, který kamera sleduje
    public float smoothTime = 0.3f; // Èas hladkého pøechodu
    public Vector3 offset; // Výchozí offset kamery
    private Vector3 velocity = Vector3.zero; // Rychlost kamery pro SmoothDamp

    public float mouseInfluence = 2.0f; // Jak moc myš ovlivní pozici kamery
    public Vector2 mouseOffsetLimit = new Vector2(5f, 5f); // Maximální vychýlení kamery od offsetu

    void Update()
    {
        if (target != null)
        {
            // Základní pozice kamery podle cíle a offsetu
            Vector3 targetPosition = target.position + offset;

            // Získání pozice myši relativnì ke støedu obrazovky
            Vector2 mousePosition = Input.mousePosition;
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 mouseDelta = (mousePosition - screenCenter) / screenCenter; // Normalizováno na hodnoty od -1 do 1

            // Upravení vychýlení kamery na základì pozice myši
            Vector3 mouseOffset = new Vector3(
                Mathf.Clamp(mouseDelta.x * mouseInfluence, -mouseOffsetLimit.x, mouseOffsetLimit.x),
                0, // Neposouváme kameru vertikálnì
                Mathf.Clamp(mouseDelta.y * mouseInfluence, -mouseOffsetLimit.y, mouseOffsetLimit.y)
            );

            // Pøidání vychýlení myši k cílové pozici kamery
            targetPosition += mouseOffset;

            // Plynulý pøechod na novou pozici
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
