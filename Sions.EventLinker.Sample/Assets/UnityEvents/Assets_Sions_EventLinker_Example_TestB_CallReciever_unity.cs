namespace SionsEventLink.Runtime
{
    /// DIRECTORY : file://E:/Sions/0_Portfolio/src/projects/Sions.EventLinker/Sions.EventLinker.Sample/Assets/Sions.EventLinker.Example/TestB
    /// FILE      : file://E:/Sions/0_Portfolio/src/projects/Sions.EventLinker/Sions.EventLinker.Sample/Assets/Sions.EventLinker.Example/TestB/CallReciever.unity
    [UnityEngine.Scripting.Preserve]
    public class Assets_Sions_EventLinker_Example_TestB_CallReciever_unity
    {
        /// HIERARCHY : Canvas/none[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_none_3_m_OnClick_0(TestB.Receiver target)
            => target.PublicMethod();

        /// HIERARCHY : Canvas/string[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        // !----- MISSING METHOD -----!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //public void Canvas_string_3_m_OnClick_0(TestB.Receiver target, string p0)
        //    => target.PublicMethod(p0);

        /// HIERARCHY : Canvas/int[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        // !----- MISSING METHOD -----!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //public void Canvas_int_3_m_OnClick_0(TestB.Receiver target, int p0)
        //    => target.PublicMethod(p0);

        /// HIERARCHY : Canvas/nullable bool[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        // !----- MISSING METHOD -----!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //public void Canvas_nullable_bool_3_m_OnClick_0(TestB.Receiver target, bool p0)
        //    => target.PublicMethod(p0);

        /// HIERARCHY : Canvas/comp[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_comp_3_m_OnClick_0(TestB.Receiver target, UnityEngine.Component p0)
            => target.PublicMethod(p0);

        /// HIERARCHY : Canvas/Behav[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_Behav_3_m_OnClick_0(TestB.Receiver target, UnityEngine.Behaviour p0)
            => target.PublicMethod(p0);

        /// HIERARCHY : Canvas/Mono[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_Mono_3_m_OnClick_0(TestB.Receiver target, UnityEngine.MonoBehaviour p0)
            => target.PublicMethod(p0);

        /// HIERARCHY : Canvas/obj[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_obj_3_m_OnClick_0(TestB.Receiver target, UnityEngine.Object p0)
            => target.PublicMethod(p0);

        /// HIERARCHY : Canvas/testObj comp[3]
        /// PROPERTY  : m_OnClick.m_PersistentCalls.m_Calls.Array.data[0]
        public void Canvas_testObj_comp_3_m_OnClick_0(TestB.Receiver target, UnityEngine.Component p0)
            => target.TestObj(p0);

    }
}
