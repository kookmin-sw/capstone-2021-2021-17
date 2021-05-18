﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

/* 이 스크립트는 GamePlayer에 네트워크 기능을 다룹니다.
 * 
 */ 


public class NetGamePlayer : NetworkBehaviour
{
    [SyncVar(hook =nameof(OnHealthChanged))]
    public int Health;

    [SyncVar]
    public string Nickname;

    [SyncVar]
    public bool isLeader;

    [SyncVar]
    public ThirdPersonCharacter.State State;

    [SyncVar]
    public PlayerEndingState EndState;

    [SyncVar(hook = nameof(OnHandItemChanged))]
    public int handItemidx = -1;

    public ThirdCamera ThirdCamera;

    public Canvas Canvas;
    
    public MovePlayerTestForEnemy Character;
    
    public PlayerControlForEnemy ThirdControl;
    
    public PlayerHealth PlayerHealth;

    public PlayerInventory PlayerInventory;

    public TMP_Text NicknameUI;

    private InGame_MultiGameManager MultigameManager;

    private void Awake()
    {
        Health = PlayerHealth.health;
        State = (ThirdPersonCharacter.State)Character.state;
        EndState = PlayerEndingState.None;
        MultigameManager = InGame_MultiGameManager.instance;
    }

    public override void OnStartClient()
    {
        InGame_MultiGameManager.AddPlayer(this);

        if (isLocalPlayer)
        {
            if(MultigameManager.DebugIntroCam != null)
            {
                MultigameManager.DebugIntroCam.gameObject.SetActive(false);
            }
            else // == null
            {
                Debug.Log("Intro Cam is not detected - InGame_MultiGameManager  , heeun an");
            }

            if (ThirdCamera.gameObject != null)
            {
                ThirdCamera.gameObject.SetActive(true);               
            }

            if (Canvas.gameObject != null)
            {
                Canvas.gameObject.SetActive(true);
            }
            keypadSystem.KPDisableManager disableManager = keypadSystem.KPDisableManager.instance;
            disableManager.player = this.gameObject;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            SlotManager.instance.inventory = PlayerInventory;
            PlayerInventory.SlotManager = SlotManager.instance;

            NicknameUI.text = "";
        }
        else
        {
            NicknameUI.text = Nickname;
        }
        
    }

    public override void OnStopClient()
    {
        InGame_MultiGameManager.DisablePlayer(this);
    }
    
    public void ChangeHealth(int h)
    {
        if (isLocalPlayer) // Authority Check
        {
            CmdChangeHealth(h);
        } 
    }
    
    public void ChangeState(ThirdPersonCharacter.State state)
    {
        if (isLocalPlayer) // Authority Check
        {
            CmdChangeState(state);
        }
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
    [ClientCallback]
    public void MoveCharacter(Vector3 move, bool crouch, bool jump)
    {
        if (isLocalPlayer) // Authority Check
        {
            Character.Move(move, crouch, jump);
        }
    }

    [Command]
    private void CmdChangeHealth(int h)
    {
        Health = h;
    }


    [Command]
    private void CmdChangeState(ThirdPersonCharacter.State state)
    {
        State = state;
    }

    [Command]
    private void CmdPlaySound()
    {
        RpcPlaySound();
    }

    [ClientRpc]
    private void RpcPlaySound()
    {

    }

    [Command]
    private void CmdPlaySoundOneShot(int soundId)
    {
        RpcPlaySoundOneShot(soundId);
    }

    [ClientRpc]
    private void RpcPlaySoundOneShot(int soundId)
    {
        //사용 안해서 삭제 
    }

    [Command]
    private void CmdStopSound()
    {
        RpcStopSound();
    }

    [ClientRpc]
    private void RpcStopSound()
    {
        // Character.soundSources-> Character.soundSource
    }


    public void SpawnObject(Item item, Vector3 position, Quaternion rotation)
    {
        if(item is HealPack)
        {
            CmdSpawnObject(0, position, rotation);
        }
        else if(item is Gun)
        {
            CmdSpawnObject(1, position, rotation);
        }
        
    }

    [Command]
    private void CmdSpawnObject(int idx, Vector3 position, Quaternion rotation)
    {
        if (idx ==0) {
            GameObject createdObject = Instantiate( PlayerInventory.HealPackPrefab, position, rotation);
            NetworkServer.Spawn(createdObject);
        }
        else if (idx == 1)
        {
            GameObject createdObject = Instantiate(PlayerInventory.GunPrefab, position, rotation);
            NetworkServer.Spawn(createdObject);
        }
    }

    public bool IsItemUsing = false;

    public void SetActiveHandItem(Item item)
    {
        if (IsItemUsing) return;

        if(item == null)
        {
            CmdSetActiveHandItem(-1);
        }
        if (item is HealPack)
        {
            CmdSetActiveHandItem(0);
        }
        else if(item is Gun)
        {
            CmdSetActiveHandItem(1);
        }
        
    }

    [Command]
    private void CmdSetActiveHandItem( int item_idx)
    {
        handItemidx = item_idx;
    }

    void OnHandItemChanged(int oldItem, int newItem)
    {
        if(oldItem >= 0)
        PlayerInventory.HandItems[oldItem].SetActive(false);
        if(newItem >= 0 )
        PlayerInventory.HandItems[newItem].SetActive(true);

    }

    void OnHealthChanged(int oldVal, int newVal)
    {
        if(newVal == 0)
        {
            Die();
        }
    }

    
    public void Escape()
    {
        if (isLocalPlayer)
        {
            EndingPlayerMessage msg = new EndingPlayerMessage()
            {
                PlayerName = Nickname,
                endingState = PlayerEndingState.Live,
            };

            //ThirdCamera.gameObject.child(0).GetComponent<AudioListener>().enabled = false;
            CmdSetEndingState(PlayerEndingState.Live);

            foreach(var audio in GameObject.FindObjectsOfType<AudioSource>())
            {
                audio.volume = 0;
            }

            NetworkClient.Send(msg);

            

            gameObject.SetActive(false);
            

        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Die()
    {
        if (isLocalPlayer)
        {
            EndingPlayerMessage msg = new EndingPlayerMessage()
            {
                PlayerName = Nickname,
                endingState = PlayerEndingState.Dead
            };

            CmdSetEndingState(PlayerEndingState.Dead);

            NetworkClient.Send(msg);
            
            //UnityEngine.SceneManagement.SceneManager.LoadScene("Ending");
        }
    }

    [Command]
    public void CmdSetEndingState(PlayerEndingState endingState)
    {
        EndState = endingState;
    }
}
