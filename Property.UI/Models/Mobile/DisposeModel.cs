
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models.Mobile
{

    /// <summary>
    /// 指派处理人模型
    /// </summary>
    public class DisposerModel : DetailSearchModel
    {
        /// <summary>
        /// 处理描述
        /// </summary>
        public string DisposeDesc { get; set; }

        /// <summary>
        /// 是否公示 默认0：不公示 1：公示
        /// </summary>
        public int IsPublish { get; set; }
    }

    /// <summary>
    /// 处理
    /// </summary>
    public class DisposeListModel : PagedSearchModel
    {
        /// <summary>
        /// 0:未指派处理人  1：处理人为自己
        /// </summary>
        public int Type { get; set; }
    }

    /// <summary>
    /// 指派处理人模型
    /// </summary>
    public class SetDisposerModel : DetailSearchModel
    {
        /// <summary>
        /// 处理人ID
        /// </summary>
        public int DisposerId { get; set; }
    }
}