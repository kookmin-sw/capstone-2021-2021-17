using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Object_Test : MonoBehaviour
{
    
    //check enemy has Path
    Enemy_Chase Check_HasP;
    Animator ani;
    void Start()
    {
        ani = GetComponent<Animator>();
        Check_HasP = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy_Chase>();
    }

    // ��ΰ� ������ �ִϸ��̼� ����.
    void Update()
    {
        if (Check_HasP.hasP)
        {
            ani.SetBool("Walk", true);
        }
        else
        {
            ani.SetBool("Walk", false);
        }
    }
}
