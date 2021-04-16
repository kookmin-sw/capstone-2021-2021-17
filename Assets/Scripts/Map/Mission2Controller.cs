using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using keypadSystem;

public class Mission2Controller : MonoBehaviour
{
    //ĵ����
    [SerializeField] private GameObject missionCanvas;
    //������ ������Ʈ
    [SerializeField] private GameObject missionObject;
    //����� ������ ��
    public string correctCode;
    //�����ϰ� ���ΰ�
    public string randomCode;
    //�Է¹����� ����Ȯ�ο�
    public bool firstClick;
    //���� ���¸� ǥ���� �ؽ�Ʈ
    public Text[] state;
    //�̼� Ŭ���� ����
    private bool clearMission = false;


    public void CheckCode()
    { 
        if(state[1].text == correctCode[3].ToString() && state[2].text == correctCode[2].ToString() && state[3].text == correctCode[1].ToString() && state[4].text == correctCode[0].ToString())
        {
            clearMission = true;
        }
        if (clearMission == true)
        {
            state[0].text = "Clear";
            UnableMission();
            ValidStageClear();

            //1�ʵ� â ����
            Invoke("CloseMission", 1.0f);
        }
    }

    //�̼ǿ�����Ʈ ��ȣ�ۿ� ����
    public void UnableMission()
    {
        missionObject.tag = "Untagged";
        missionObject.layer = 0;
    }

    //�̼�â Ȱ��ȭ
    public void ShowMission()
    {
        KPDisableManager.instance.DisablePlayer(true);
        missionCanvas.SetActive(true);
    }

    //�̼�â ��Ȱ��ȭ
    public void CloseMission()
    {
        KPDisableManager.instance.DisablePlayer(false);
        missionCanvas.SetActive(false);
    }

    void ValidStageClear()
    {
        //Ż�ⱸ�� ������ �κ�
        GameMgr.MissionClear();
    }
    public void CodeChange(string num)
    {
        switch(num)
        {
            case "1":
                ChangeNum(1, 1);
                break;
            case "2":
                ChangeNum(1, 0);
                break;
            case "3":
                ChangeNum(2, 1);
                break;
            case "4":
                ChangeNum(2, 0);
                break;
            case "5":
                ChangeNum(3, 1);
                break;
            case "6":
                ChangeNum(3, 0);
                break;
            case "7":
                ChangeNum(4, 1);
                break;
            case "8":
                ChangeNum(4, 0);
                break;
        }
    }

    private void ChangeNum(int n, int calc)
    {
        int num = 0;
        num = int.Parse(state[n].text);
        if(calc == 0)
        {
            num--;
        }
        else 
        {
            num++;
        }
        if(0 < num && num < 10)
        {
            state[n].text = num.ToString();
        }
        else if(num == -1)
        {
            num = 9;
            state[n].text = num.ToString();
        }
        else
        {
            num = 0;
            state[n].text = num.ToString();
        }
    }

    //GameMgr ��ũ��Ʈ�� GenerateMissionCode�� ������ �̼ǿ� ����� �ڵ带 �迭�� ������
    public void SetStageCode()
    {
        int length = 4;
        correctCode = GameMgr.GeneratePassword(length);
        randomCode = GameMgr.GeneratePassword(length);
        state[0].text = correctCode;
        state[1].text = randomCode[0].ToString();
        state[2].text = randomCode[1].ToString();
        state[3].text = randomCode[2].ToString();
        state[4].text = randomCode[3].ToString();
        Debug.Log(correctCode);
        Debug.Log(randomCode);
    }
}