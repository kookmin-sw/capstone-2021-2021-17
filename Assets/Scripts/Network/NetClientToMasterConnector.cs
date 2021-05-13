﻿using MasterServerToolkit.MasterServer;
using UnityEditor;
using UnityEngine;

public class NetClientToMasterConnector : ClientToMasterConnector
{

    public bool isDisconnected = false;
    protected override void Awake()
    {
        base.Awake();
        OnConnectedEvent.AddListener(OnServerDisconnected);
    }

    void OnServerDisconnected()
    {
        isDisconnected = true;
    }

    
}