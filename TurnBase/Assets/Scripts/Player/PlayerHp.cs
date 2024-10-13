using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{
    [SerializeField]
    Transform HeartContent;
    [SerializeField]
    Heart HeartPref;

    // ���ڰ���ĵĴ��ڣ��洢ֵΪ����ֵ*2��
    int maxHp;
    int curHp;
    List<Heart> HeartsList;

    // ������
    // ֮��ɾ�˵��ã���GameManager�е���
    private void OnEnable()
    {
        Setup(4);
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <param name="_realMaxHp">δ���ŵĳ�ʼѪ��</param>
    public void Setup(int _realMaxHp)
    {
        curHp = maxHp = _realMaxHp * 2;
        HeartsList = new List<Heart>();

        for (int i = 0; i < _realMaxHp; i++)
        {
            GameObject hpPref = Instantiate(HeartPref.gameObject, HeartContent);
            HeartsList.Add(hpPref.GetComponent<Heart>());
        }
    }

    /// <summary>
    /// ����Ѫ��UI��ʾ
    /// ���Ѫ�߼�
    /// </summary>
    public void Update_HpDisplay()
    {
        for (int i = 0; i < maxHp/2; i++)
        {
            if (i < curHp / 2)
            {
                HeartsList[i].SetDisplay(1);
                return;
            }
            else if (i > curHp / 2)
            {
                HeartsList[i].SetDisplay(0);
            }
            else
            {
                HeartsList[i].SetDisplay(curHp % 2 == 1 ? -1 : 0);
            }
            
        }
        
    }

    /// <summary>
    /// �ܻ�
    /// </summary>
    /// <param name="_damageValue">�����Ѿ����ź���˺�ֵ��0.5->1��</param>
    public void OnTakeDamage(int _damageValue)
    {
        curHp -= _damageValue;
        Update_HpDisplay();

        if (curHp <= 0)
        {
            // todo:Boss�����߼�
            Debug.Log("Boss Die!");
            return;
        }
    }

    /// <summary>
    /// �ָ�
    /// </summary>
    /// <param name="_recoverValue">�����Ѿ����ź�Ļָ�ֵ��2->4��</param>
    public void OnGetRecovery(int _recoverValue)
    {
        curHp += _recoverValue;
        curHp = Mathf.Min(curHp,maxHp);

        Update_HpDisplay();
    }

}
