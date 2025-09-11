namespace SionsEventLink.Runtime
{
    /// DIRECTORY : file://E:/Sions/0_Portfolio/src/projects/Sions.EventLinker/Sions.EventLinker.Sample/Assets/Sions.EventLinker.Example/TestA/Scenes
    /// FILE      : file://E:/Sions/0_Portfolio/src/projects/Sions.EventLinker/Sions.EventLinker.Sample/Assets/Sions.EventLinker.Example/TestA/Scenes/SampleScene.unity
    [UnityEngine.Scripting.Preserve]
    public class Assets_Sions_EventLinker_Example_TestA_Scenes_SampleScene_unity
    {
        /// HIERARCHY : Canvas/SendMsg/clone[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_SendMsg_clone_3_m_OnClick_0(TestSend target)
            => target.__Clone();

        /// HIERARCHY : Canvas/SendMsg/enable[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_SendMsg_enable_3_m_OnClick_0(TestSend target)
            => target.__Enable();

        /// HIERARCHY : Canvas/SendMsg/internal[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_SendMsg_internal_3_m_OnClick_0(TestSend target)
            => target.__InternalMethod();

        /// HIERARCHY : Canvas/SendMsg/name[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_SendMsg_name_3_m_OnClick_0(TestSend target)
            => target.__Name();

        /// HIERARCHY : Canvas/SendMsg/none[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_SendMsg_none_3_m_OnClick_0(TestSend target)
            => target.__NoneMethod();

        /// HIERARCHY : Canvas/SendMsg/private[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_SendMsg_private_3_m_OnClick_0(TestSend target)
            => target.__PrivateMethod();

        /// HIERARCHY : Canvas/SendMsg/protected[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_SendMsg_protected_3_m_OnClick_0(TestSend target)
            => target.__ProtectedMethod();

        /// HIERARCHY : Canvas/SendMsg/public[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_SendMsg_public_3_m_OnClick_0(TestSend target)
            => target.__PublicMethod();

        /// HIERARCHY : Canvas/SendMsg/internal (2)[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_SendMsg_internal_2_3_m_OnClick_0(TestSend target)
            => target.__SetActive();

        /// HIERARCHY : Canvas/SendMsg/internal (1)[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_SendMsg_internal_1_3_m_OnClick_0(TestSend target)
            => target.__SetSibling();

        /// HIERARCHY : Canvas/SendMsg/internal (3)[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_SendMsg_internal_3_3_m_OnClick_0(TestSend target)
            => target.__SetSibling();

        /// HIERARCHY : Canvas/SendMsg/privateStart[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_SendMsg_privateStart_3_m_OnClick_0(TestSend target)
            => target.__Start();

        /// HIERARCHY : Canvas/Missing-Access/Btn[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        // !----- INACCESSIBLE METHOD -----!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //public void Canvas_Missing_Access_Btn_3_m_OnClick_0(MonoBehaviour1 target)
        //    => target.OnEnable();

        /// HIERARCHY : Canvas/Missing-Object/Btn[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        // !----- MISSING TARGET -----!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //public void Canvas_Missing_Object_Btn_3_m_OnClick_0(object target)
        //    => target.PublicMethod();

        /// HIERARCHY : Canvas/Ok/Btn[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_Ok_Btn_3_m_OnClick_0(MonoBehaviour1 target)
            => target.PublicMethod();

        /// HIERARCHY : Canvas/Missing-Method/Btn[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        // !----- MISSING METHOD -----!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //public void Canvas_Missing_Method_Btn_3_m_OnClick_0(MonoBehaviour1 target)
        //    => target.UnkownMethod();

    }
}
