using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_2_OpenDoors : BaseOpenDoor
{
    public override void OpenDoor()
    {
        door.transform.position = new Vector3(door.transform.position.x, -10, door.transform.position.z);
    }
}
