using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_3_OpenDoors : BaseOpenDoor
{
    public override void OpenDoor()
    {
        Doors();
    }

    private void Doors()
    {
        Destroy(gameObject);
    }
}
