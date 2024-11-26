using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public RoomManager roomManager; // Odkaz na Room Manager
    public int roomIndex; // Index t�to m�stnosti

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            roomManager.ActivateRoom(roomIndex);
        }
    }
}