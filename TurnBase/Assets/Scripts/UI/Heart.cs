using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    [SerializeField]
    Image HeartMask;

    //[SerializeField]
    //Sprite FullSprite;  // ��������
    //[SerializeField]
    //Sprite HalfSprite;  // �������

    /// <summary>
    /// ����Ѫ��״̬
    /// </summary>
    /// <param name="_value">1_��Ѫ��0_��Ѫ��-1_��Ѫ</param>
    public void SetDisplay(int _value)
    {
        if (_value == 1)
        {
            HeartMask.fillAmount = 1;
            // HeartMask.sprite = FullSprite;
        }
        else if (_value == 0)
        {
            HeartMask.fillAmount = 0;
            // HeartMask.sprite = null;
        }
        else if (_value == -1)
        {
            HeartMask.fillAmount = 0.5f;
            // HeartMask.sprite = HalfSprite;
        }
        else
        {
            Debug.LogError("Error HeartDisplay");
        }
    }
}
