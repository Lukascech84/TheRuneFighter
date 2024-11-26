using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public List<GameObject> rooms; // Seznam v�ech m�stnost�
    public int activeRoomIndex = 0; // Index aktu�ln� m�stnosti

    public void ActivateRoom(int roomIndex)
    {
        // Aktivuj zvolenou m�stnost a sousedn� m�stnosti
        for (int i = 0; i < rooms.Count; i++)
        {
            if (i == roomIndex || i == roomIndex - 1 || i == roomIndex + 1 || i == roomIndex + 2 || i == roomIndex - 2)
            {
                rooms[i].SetActive(true);
            }
            else
            {
                rooms[i].SetActive(false);
            }
        }

        activeRoomIndex = roomIndex;
    }
}
