using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;
using UnityEngine.UI;

namespace TestB
{
    public class Receiver : MonoBehaviour
    {
        //public void PublicMethod(object temp)
        //{
        //    Debug.Log("PublicMethod obj " + temp.GetType().FullName + " / " + temp);
        //}
        //public void PublicMethod(TempInfo temp)
        //{
        //    Debug.Log("PublicMethod temp " + temp.name);
        //}
        //public void PublicMethod(float temp)
        //{
        //    Debug.Log("PublicMethod float " + temp);
        //}
        //public void PublicMethod(int temp)
        //{
        //    Debug.Log("PublicMethod int " + temp);
        //}
        //public void PublicMethod(string temp)
        //{
        //    Debug.Log("PublicMethod Str " + temp);
        //}
        //public void PublicMethod(Vector3 temp)
        //{
        //    Debug.Log("PublicMethod V3 " + temp);
        //}
        //public void PublicMethod(Nullable<int> temp)
        //{
        //    Debug.Log("PublicMethod Nullable " + temp);
        //}
        //public void PublicMethod(Nullable<bool> temp)
        //{
        //    Debug.Log("PublicMethod Nullable " + temp);
        //}
        public void PublicMethod(Component temp)
        {
            Debug.Log("PublicMethod Comp " + temp);
            printA();
        }
        public void PublicMethod(MonoBehaviour temp)
        {
            Debug.Log("PublicMethod Mono " + temp);
            printA();

        }
        public void PublicMethod(UnityEngine.Object temp)
        {
            Debug.Log("PublicMethod UOBJ " + temp);
            printA();
        }
        public void PublicMethod(Behaviour temp)
        {
            Debug.Log("PublicMethod Behaviour " + temp);
            printA();
        }
        public void TestObj(UnityEngine.Component temp)
        {
            Debug.Log("PublicMethod UOBJ " + temp);
            printA();
        }
        public void PublicMethod()
        {
            Debug.Log("PublicMethod");
        }

        private void printA()
        {
            //var f = GetType().GetMethod("PublicMethod", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            var f2 = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(v=>v.Name == "PublicMethod");
            Debug.Log("print " + f2.GetParameters()[0].ParameterType.Name + " / " + f2.GetParameters()[0].ParameterType.Name);
        }

        //public void Test(int temp) => print("PublicMethod int " + temp);
        public void Test(HideFlags temp) => print("PublicMethod hide " + temp);
        //public void Test(int temp)=>print("PublicMethod int " + temp);
        //public void Test(float temp)=>print("PublicMethod float " + temp);
        //public void Test(string temp) =>print("PublicMethod string " + temp);
        public void Test(GameObject temp) =>print("PublicMethod temp " + temp);
    }
}