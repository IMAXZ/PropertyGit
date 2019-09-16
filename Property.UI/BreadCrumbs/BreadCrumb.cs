using System;
using System.Linq;
using System.Text;

namespace MvcBreadCrumbs
{
    /// <summary>
    /// 面包屑操作对象
    /// </summary>
    public class BreadCrumb
    {
        /// <summary>
        /// session操作对象
        /// </summary>
        private static IProvideBreadCrumbsSession _SessionProvider { get; set; }

        /// <summary>
        /// 获取session操作对象
        /// </summary>
        private static IProvideBreadCrumbsSession SessionProvider
        {
            get
            {
                if (_SessionProvider != null)
                {
                    return _SessionProvider;
                }
                return new HttpSessionProvider();
            }
        }

        /// <summary>
        /// 清除面包屑
        /// </summary>
        public static void ClearState() {
            StateManager.RemoveState(SessionProvider.SessionId);
        }

        /// <summary>
        /// 设置当前画面名称
        /// </summary>
        /// <param name="label"></param>
        public static void SetLabel(string label)
        {
            var state = StateManager.GetState(SessionProvider.SessionId);
            state.Current.Label = label;
        }

        /// <summary>
        /// 获取面包屑导航
        /// </summary>
        /// <returns></returns>
        public static State GetBreadCrumbState() {
            var state = StateManager.GetState(SessionProvider.SessionId);
            return state;
        }

    }

}
