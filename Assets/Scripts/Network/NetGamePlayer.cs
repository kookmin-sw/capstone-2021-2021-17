﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/* 이 스크립트는 GamePlayer에 네트워크 기능을 다룹니다.
 * 
 */ 


public class NetGamePlayer : NetworkBehaviour
{
    [SyncVar]
    public int Health;

    [SyncVar]
    public string Nickname;

    [SyncVar]
    public bool isLeader;

    [SyncVar]
    public ThirdPersonCharacter.State State;

    
    public ThirdCamera ThirdCamera;
    
    public ThirdPersonCharacter Character;
    
    public ThirdPersonUserControl ThirdControl;
    
    public PlayerHealth PlayerHealth;

    private InGame_MultiGameManager MultigameManager;

    private void Awake()
    {
        Health = PlayerHealth.Health;
        State = Character.state;
        MultigameManager = InGame_MultiGameManager.instance;
    }

    public override void OnStartClient()
    {
        InGame_MultiGameManager.AddPlayer(this);

        if (isLocalPlayer)
        {
            if(MultigameManager.DebugIntroCam != null)
            {
                MultigameManager.DebugIntroCam.enabled = false;
            }
            else // == null
            {
                Debug.Log("Intro Cam is not detected - InGame_MultiGameManager  , heeun an");
            }

            if (ThirdCamera.gameObject != null)
            {
                ThirdCamera.gameObject.SetActive(true);
                
            }

            keypadSystem.KPDisableManager disableManager = keypadSystem.KPDisableManager.instance;
            disableManager.player = this.gameObject;

        }
    }

    public override void OnStopClient()
    {
        InGame_MultiGameManager.DisablePlayer(this);
    }


    [ClientCallback]
    public void MoveCharacter(Vector3 move, bool crouch, bool jump)
    {
        if (isLocalPlayer) // Authority Check
        {
            Character.Move(move, crouch, jump);
        }
    }

    
    public void ChangeHealth(int h)
    {
        if (isLocalPlayer) // Authority Check
        {
            CmdChangeHealth(h);
        } 
    }

    [Command]
    private void CmdChangeHealth(int h)
    {
        Health = h;
    }

    
    public void ChangeState(ThirdPersonCharacter.State state)
    {
        if (isLocalPlayer) // Authority Check
        {
            CmdChangeState(state);
        }
    }

    [Command]
    private void CmdChangeState(ThirdPersonCharacter.State state)
    {
        State = state;
    }

    public void PlaySound()
    {
        CmdPlaySound();
    }

    public void StopSound()
    {
        CmdStopSound();
    }

    public void PlaySoundOneShot(int soundId)
    {
        CmdPlaySoundOneShot(soundId);
    }


    [Command]
    private void CmdPlaySound()
    {
        RpcPlaySound();
    }

    [ClientRpc]
    private void RpcPlaySound()
    {
        Character.soundSource.Play();        
    }

    [Command]
    private void CmdPlaySoundOneShot(int soundId)
    {
        RpcPlaySoundOneShot(soundId);
    }

    [ClientRpc]
    private void RpcPlaySoundOneShot(int soundId)
    {
        Character.soundSource.PlayOneShot(Character.sound[soundId]);
    }

    [Command]
    private void CmdStopSound()
    {
        RpcStopSound();
    }

    [ClientRpc]
    private void RpcStopSound()
    {
        Character.soundSources.Stop();
    }




    

    
}
