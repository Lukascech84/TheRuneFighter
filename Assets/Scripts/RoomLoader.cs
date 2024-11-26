using UnityEngine;

public class RoomLoader : MonoBehaviour
{
    public GameObject currentRoom; // Aktuální místnost
    public GameObject[] adjacentRooms; // Sousední místnosti

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Aktivace aktuální místnosti
            currentRoom.SetActive(true);

            // Aktivace sousedních místností
            foreach (GameObject room in adjacentRooms)
            {
                room.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Deaktivace aktuální místnosti, pokud hráè opustí
            currentRoom.SetActive(false);

            // Deaktivace sousedních místností
            foreach (GameObject room in adjacentRooms)
            {
                room.SetActive(false);
            }
        }
    }
}