using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TestB
{
    public class TempInfo
    {
        public string name = Guid.NewGuid().ToString();
    }

    public class Sender : MonoBehaviour
    {
        public void SendNone() => SendMessage("PublicMethod");
        public void SendTempInfo() => SendMessage("PublicMethod", new TempInfo());
        public void Sendint() => SendMessage("PublicMethod", 55);
        public void Sendstring() => SendMessage("PublicMethod", "abc");
        public void SendVector3() => SendMessage("PublicMethod", new Vector3(1, 2, 3));
        public void SendNullabl() => SendMessage("PublicMethod", new Nullable<int>());
        public void SendComp() => SendMessage("PublicMethod", (Component)this);
        public void SendBehaviour() => SendMessage("PublicMethod", (Behaviour)this);
        public void SendMono() => SendMessage("PublicMethod", this);

    }
}