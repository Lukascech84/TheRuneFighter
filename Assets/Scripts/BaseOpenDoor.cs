using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseOpenDoor : MonoBehaviour
{
    [HideInInspector] public GameObject door;

    public virtual void Start()
    {
        door = gameObject;
    }

    public virtual void OpenDoor()
    {
        
    }
}
