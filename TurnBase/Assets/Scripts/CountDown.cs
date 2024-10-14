using UnityEngine;
using TMPro;
using System;

public class CountDown : BaseSingleton<CountDown>
{
    [SerializeField]
    TMP_Text CountDown_Text;

    // ��ʱѭ��
    [HideInInspector]
    public int CountDownCycle = 60;

    // ��ʱ��״̬
    int curTime = 0;
    float timer = 0;
    bool isTiming = false;

    public Action OnTimerEnd;   // ��ʱ��������¼�

    /// <summary>
    /// ��ʼ����ʱ��
    /// </summary>
    public void Setup(int cycleTime)
    {
        curTime = CountDownCycle = cycleTime;
        timer = 0;
        isTiming = true;

        UpdateTimerDisplay();
    }

    public void UpdateTimerDisplay()
    {
        CountDown_Text.text = curTime.ToString("00:00");

        if (curTime <= 0)
        {
            isTiming = false;
            // ��ʱ��������ִ�ж�ս��
            OnTimerEnd?.Invoke();
        }
    }


    /// <summary>
    /// ��ͣ��ʱ��
    /// </summary>
    public void SetTimerPause()
    {
        isTiming = false;
    }

    private void Update()
    {
        if (!isTiming)
        {
            return;
        }

        timer += Time.deltaTime;
        // todo:ʹ�üٵļ�ʱ���
        if (timer > 1)
        {
            timer = 0;
            curTime--;
            UpdateTimerDisplay();
        }
    }

}
