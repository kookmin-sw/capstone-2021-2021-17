using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkAnimator))] // lever animation
[RequireComponent(typeof(NetworkTransformChild))] // transform doorTransform
public class DoorNetBehaviour : NetworkBehaviour
{
    [SerializeField]
    private DoorController controller;

    [SyncVar]
    public bool IsOpen;

    [Command(ignoreAuthority = true)] // send to Server
    public void CmdOpenDoor()
    {
        IsOpen = true;
        controller.OpenDoor();
    }

    [Command(ignoreAuthority = true)] // send to Server
    public void CmdCloseDoor()
    {
        IsOpen = false;
        controller.CloseDoor();
    }
}
