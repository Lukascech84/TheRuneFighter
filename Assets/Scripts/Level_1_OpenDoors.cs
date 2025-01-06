using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_1_OpenDoors : BaseOpenDoor
{
    public override void OpenDoor()
    {
        door.transform.Rotate(0, 130, 0);
    }
}
