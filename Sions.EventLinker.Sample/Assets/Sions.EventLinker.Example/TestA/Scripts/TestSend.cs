using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class TestSend : MonoBehaviour
{
    public void __Start()
    {
        SendMessage("Start");
    }

    public void __OnEnable()
    {
        SendMessage("Update");

    }

    public void __InternalMethod()
    {
        SendMessage("InternalMethod");
    }

    public void __PublicMethod()
    {
        SendMessage("PublicMethod");
    }

    public void __PrivateMethod()
    {
        SendMessage("PrivateMethod");
    }

    public void __ProtectedMethod()
    {
        SendMessage("ProtectedMethod");
    }

    public void __NoneMethod()
    {
        SendMessage("NoneMethod");
    }

    public void __SetActive()
    {
        SendMessage("SetActive", false);
    }

    public void __SetSibling()
    {
        SendMessage("SetAsLastSibling");
    }

    public void __RendererReceiveShadow()
    {
        SendMessage("set_receiveShadows", true);
    }

    public void __Enable()
    {
        SendMessage("set_enabled", false);
    }

    public void __SendOnSuccess()
    {
        SendMessage("OnSuccess");
    }

    public void __Name()
    {
        SendMessage("set_name", "test");
        MemberwiseClone();
    }
    public void __Clone()
    {
        SendMessage("MemberwiseClone");
    }

    public TestSend()
    {
        Debug.Log("Clone");
    }
}
