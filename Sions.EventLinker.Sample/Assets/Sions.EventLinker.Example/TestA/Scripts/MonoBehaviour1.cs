using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MonoBehaviour1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
    }

    void OnEnable()
    {
        Debug.Log("Update");

    }

    internal void InternalMethod()
    {
        Debug.Log("InternalMethod");
    }

    public void PublicMethod()
    {
        Debug.Log("PublicMethod");
    }

    private void PrivateMethod()
    {
        Debug.Log("PrivateMethod");
    }

    protected void ProtectedMethod()
    {
        Debug.Log("ProtectedMethod");
    }

    void NoneMethod()
    {
        Debug.Log("NoneMethod");
    }

    public void OnSuccess()
    {
        Debug.Log("OnSuccess");
    }

}
