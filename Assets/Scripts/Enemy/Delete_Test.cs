using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�÷��̾�� �浹 ���� �� �÷��̾ ���ְ� �ٸ� Ÿ���� ã�� ���� bool�� ���¸� �Ѱ��ش�
public class Delete_Test : MonoBehaviour
{
    
    public Enemy_Chase Check_Catch;

    public void OnTriggerEnter(Collider other)   
    {
        //�±װ� �÷��̾��
        if(other.CompareTag("Player")){
            //��Ҵٴ� �� �Ѱ��ش�.
            Check_Catch.Check_Catched = true;
            Destroy(gameObject);
        }
    }
}
