using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadTest : MonoBehaviour
{
    bool flag;
    private void Start()
    {

    }

    private void Update()
    {
        if (!flag)
        {
            //Thread thread1 = new Thread(Log1);
            //Thread thread2 = new Thread(Log1);
            //Thread thread3 = new Thread(Log1);
            //thread1.Start();
            //thread2.Start();
            //thread3.Start();
            Log1();
            Log1();
            Log1();
            flag = true;
        }
    }
    private void Log1()
    {
        int index = 10000000;
        for (int i = 0; i < index; i++)
        {

        }

        string time = Time.time.ToString();
        Debug.LogFormat("Time:{0}", time);

    }




}
