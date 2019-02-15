using System;
namespace ExtensionMethods
{
    public static class LipidCreatorExtensions
    {
        public static void InvokeIfRequired(this System.ComponentModel.ISynchronizeInvoke obj, System.Windows.Forms.MethodInvoker action)
        {
            if (obj.InvokeRequired)
            {
                obj.Invoke(action, new object[0]);
            }
            else
            {
                action.Invoke();
            }
        }
    }
}
