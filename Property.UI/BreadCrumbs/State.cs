using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using Microsoft.SqlServer.Server;

namespace MvcBreadCrumbs
{
    /// <summary>
    /// 面包屑导航
    /// </summary>
    public class State
    {
        /// <summary>
        /// session Cookis
        /// </summary>
        public string SessionCookie { get; set; }

        /// <summary>
        /// 所有的面包屑
        /// </summary>
        public List<StateEntry> Crumbs { get; set; }

        /// <summary>
        /// 当前的面包屑
        /// </summary>
        public StateEntry Current { get; set; }
       
        /// <summary>
        /// 添加点击到导航中
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="label">当前画面名称</param>
        public void Push(ActionExecutingContext context, string label)
        {
            string controller = context.RouteData.Values["controller"].ToString();
            string action = context.RouteData.Values["action"].ToString();
            var key = ("/" + controller + "/" + action)
                .ToLower()
                .GetHashCode();

            if (Crumbs.Any(x => x.Key == key))
            {
                var newCrumbs = new List<StateEntry>();
                var remove = false;
                // We've seen this route before, maybe user clicked on a breadcrumb
                foreach (var crumb in Crumbs)
                {
                    if (crumb.Key == key)
                    {
                        remove = true;
                    }
                    if (!remove)
                    {
                        newCrumbs.Add(crumb);
                    }
                }
                Crumbs = newCrumbs;
            }

            Current = new StateEntry().WithKey(key)
                .SetContext(context)
                .WithUrl(context.HttpContext.Request.Url.ToString())
                .WithLabel(label);
                
            Crumbs.Add(Current);

        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cookie"></param>
        public State(string cookie)
        {
            SessionCookie = cookie;
            Crumbs = new List<StateEntry>();
        }

    }

    /// <summary>
    /// 每个面包屑实体
    /// </summary>
    public class StateEntry
    {
        /// <summary>
        /// 上下文
        /// </summary>
        public ActionExecutingContext Context { get; private set; }

        /// <summary>
        /// 画面名称
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 主键Key
        /// </summary>
        public int Key { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 对应的Controller名称
        /// </summary>
        public string Controller
        {
            get
            {
                return (string)Context.RouteData.Values["controller"];
            }
        }

        /// <summary>
        /// 对应的action名称
        /// </summary>
        public string Action
        {
            get
            {
                return (string)Context.RouteData.Values["action"];
            }
        }

        /// <summary>
        /// 设置Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public StateEntry WithKey(int key)
        {
            Key = key;
            return this;
        }

        public StateEntry WithUrl(string url)
        {
            Url = url;
            return this;
        }

        public StateEntry WithLabel(string label)
        {
            Label = label ?? Label;
            return this;
        }


        public StateEntry SetContext(ActionExecutingContext context)
        {
            Context = context;
            Label = Label ?? (string) context.RouteData.Values["action"];
            return this;
        }

      

    }
}