namespace SionsEventLink.Runtime
{
    /// DIRECTORY : file://E:/Sions/0_Portfolio/src/projects/Sions.EventLinker/Sions.EventLinker.Sample/Assets/Sions.EventLinker.Example/TestA
    /// FILE      : file://E:/Sions/0_Portfolio/src/projects/Sions.EventLinker/Sions.EventLinker.Sample/Assets/Sions.EventLinker.Example/TestA/testanim.anim
    [UnityEngine.Scripting.Preserve]
    public class Assets_Sions_EventLinker_Example_TestA_testanim_anim
    {
        /// CLIP : testanim(7400000)
        /// TIME : 0.2167s
        // !----- INACCESSIBLE METHOD -----!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //public void testanim_7400000_0_2167s(MonoBehaviour1 target)
        //    => target.InternalMethod();

        /// CLIP : testanim(7400000)
        /// TIME : 0.1333s
        // !----- INACCESSIBLE METHOD -----!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //public void testanim_7400000_0_1333s(MonoBehaviour1 target)
        //    => target.ProtectedMethod();

        /// CLIP : testanim(7400000)
        /// TIME : 0.0167s
        public void testanim_7400000_0_0167s(TestB.Receiver target, UnityEngine.Component p0)
            => target.PublicMethod(p0);

        /// CLIP : testanim(7400000)
        /// TIME : 0.0167s
        public void testanim_7400000_0_0167s_0(MonoBehaviour1 target)
            => target.PublicMethod();

        /// CLIP : testanim(7400000)
        /// TIME : 0.2667s
        public void testanim_7400000_0_2667s(TestB.Receiver target, UnityEngine.Component p0)
            => target.PublicMethod(p0);

        /// CLIP : testanim(7400000)
        /// TIME : 0.2667s
        public void testanim_7400000_0_2667s_0(MonoBehaviour1 target)
            => target.PublicMethod();

        /// CLIP : testanim(7400000)
        /// TIME : 0.3667s
        public void testanim_7400000_0_3667s(TestB.Receiver target, UnityEngine.Component p0)
            => target.PublicMethod(p0);

        /// CLIP : testanim(7400000)
        /// TIME : 0.3667s
        public void testanim_7400000_0_3667s_0(MonoBehaviour1 target)
            => target.PublicMethod();

    }
}
