using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CountDown : MonoBehaviour
{
    [SerializeField]
    TMP_Text CountDown_Text;

    // ��ʱѭ��
    public int CountDownCycle = 60;

    // ��ʱ��״̬
    int curTime = 0;
    float timer = 0;
    bool isTiming = false;

    public UnityAction OnTimerStart;
    public UnityAction OnTimerEnd;

    private void OnEnable()
    {
        SetTimerInit();
    }

    private void Update()
    {
        if (!isTiming)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer > 1)
        {
            timer = 0;
            curTime--;
            UpdateTimerDisplay();
        }
    }

    public void UpdateTimerDisplay()
    {
        CountDown_Text.text = curTime.ToString("00:00");
    }

    /// <summary>
    /// ��ʼ����ʱ��
    /// </summary>
    public void SetTimerInit()
    {
        curTime = CountDownCycle;
        timer = 0;
        isTiming = true;

        UpdateTimerDisplay();
    }


    /// <summary>
    /// ��ͣ��ʱ��
    /// </summary>
    public void SetTimerPause()
    {
        isTiming = false;
    }

}
