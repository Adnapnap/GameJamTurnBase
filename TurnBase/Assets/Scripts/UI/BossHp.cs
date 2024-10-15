using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHp : MonoBehaviour
{
    [SerializeField]
    Scrollbar HpScroll;

    int maxHp;
    int curHp;

    // ������
    // ֮��ɾ�˵��ã���GameManager�е���
    private void OnEnable()
    {
        Setup(40);
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <param name="_maxHp"></param>
    public void Setup(int _maxHp)
    {
        curHp = maxHp = _maxHp;
        Update_HpDisplay();
    }

    /// <summary>
    /// ����Ѫ��UI��ʾ
    /// </summary>
    public void Update_HpDisplay()
    {
        HpScroll.size = Mathf.Clamp01((float)curHp / maxHp);
    }

    /// <summary>
    /// �ܻ�
    /// </summary>
    /// <param name="_damageValue">�ܵ����˺���Ϊ��ֵ</param>
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

}
