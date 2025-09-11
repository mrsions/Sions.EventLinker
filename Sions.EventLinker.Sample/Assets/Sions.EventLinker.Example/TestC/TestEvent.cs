using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace TestC
{
    public class TempInfo
    {
        public string name = Guid.NewGuid().ToString();
    }

    public class TestEvent : MonoBehaviour
    {
        public UnityEvent<int, string, TempInfo> onEvent;
        public UnityEvent<int, string> onEvent2;

        public string this[int index] { get => ""; set { } }
        public TempInfo this[int index, string key] { get => null; set { } }

        public void Testing(int a, string b, TempInfo c)
        {

        }

        public void Testing2(int a, string b)
        {

        }

        public void Testing2(int a, string b, int c)
        {

        }
    }
}