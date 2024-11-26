using UnityEngine;

public class RoomLoader : MonoBehaviour
{
    public GameObject currentRoom; // Aktu�ln� m�stnost
    public GameObject[] adjacentRooms; // Sousedn� m�stnosti

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Aktivace aktu�ln� m�stnosti
            currentRoom.SetActive(true);

            // Aktivace sousedn�ch m�stnost�
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
            // Deaktivace aktu�ln� m�stnosti, pokud hr�� opust�
            currentRoom.SetActive(false);

            // Deaktivace sousedn�ch m�stnost�
            foreach (GameObject room in adjacentRooms)
            {
                room.SetActive(false);
            }
        }
    }
}