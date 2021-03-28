﻿using UnityEngine;
using System.Collections;
using keypadSystem;
using Mirror;

public class RayCastCam : MonoBehaviour
{
    private GameObject raycasted_obj;

    [SerializeField]
    private GameObject player_obj;

    [SerializeField]
    private Camera cam;

    private int rayLength = 2;
    public LayerMask layerMaskInteract;

    public GameObject cHNormal;
    public GameObject cHHighlight;

    private bool canPull = true;
    private KeypadItemController rayCastedKeypad;

    void Awake()
    {
        if (player_obj == null)
        {
            player_obj = transform.parent.gameObject;

        }
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void Update()
    {
        RaycastHit hit;

        Vector3 camPos = cam.transform.position;
        Vector3 camDir = cam.transform.forward;
        //player_obj = transform.parent.gameObject;

        Debug.DrawRay(camPos, camDir * 4, Color.red);
        if (Physics.Raycast(camPos, camDir, out hit, rayLength*2, layerMaskInteract.value))
        {
            if (hit.collider.CompareTag("Lever"))
            {
                raycasted_obj = hit.collider.gameObject;
                CrosshairActive();

                if (Input.GetKeyDown("e") && canPull)
                {

                    LeverController leverController = raycasted_obj.GetComponent<LeverScript>().leverController;
                    leverController.doorNet.CmdPullDownLever(raycasted_obj);
                    StartCoroutine(Timer(1.0f));
                }
            }
            else if (hit.collider.CompareTag("ItemBox"))
            {
                raycasted_obj = hit.collider.gameObject;
                CrosshairActive();

                if (Input.GetKeyDown("e"))
                {
                    ItemBoxNetBehaviour itemBoxNet = raycasted_obj.GetComponent<ItemBoxNetBehaviour>();
                    rayCastedKeypad = raycasted_obj.GetComponent<KeypadItemController>();
                    rayCastedKeypad.ShowKeypad();
                }
            }

            else if (hit.collider.CompareTag("ItemHeal"))
            {
                raycasted_obj = hit.collider.gameObject;
                CrosshairActive();

                if (Input.GetKeyDown("e"))
                {
                    HealPack healPack = raycasted_obj.GetComponent<HealPack>();
                    healPack.SetHealObject(raycasted_obj);
                    healPack.SetPlayerObject(player_obj);
                    healPack.UseHealPack();

                    //raycasted_obj.SetActive(false);
                }
            }
        }

        else
        {
            CrosshairNormal();
        }
    }

    void CrosshairActive()
    {
        cHNormal.SetActive(false);
        cHHighlight.SetActive(true);
    }

    void CrosshairNormal()
    {
        cHNormal.SetActive(true);
        cHHighlight.SetActive(false);
    }

    IEnumerator Timer(float waitTime)
    {
        canPull = false;
        yield return new WaitForSeconds(waitTime);
        canPull = true;
    }
}
