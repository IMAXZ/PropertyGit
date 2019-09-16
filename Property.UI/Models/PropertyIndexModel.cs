using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models
{
    /// <summary>
    /// 物业首页数据模型
    /// </summary>
    public class PropertyIndexModel
    {
        /// <summary>
        /// 累计App注册用户
        /// </summary>
        public int AppUserCount { get; set; }

        /// <summary>
        /// 累计物业人员用户
        /// </summary>
        public int PropertyUserCount { get; set; }

        /// <summary>
        /// 新闻公告个数
        /// </summary>
        public int NoticeCount { get; set; }

        /// <summary>
        /// 业主上报问题个数
        /// </summary>
        public int QuestionCount { get; set; }

        /// <summary>
        /// 本月未处理问题个数
        /// </summary>
        public int NotHandleQuestionCount { get; set; }

        /// <summary>
        /// 本月未处理巡检异常个数
        /// </summary>
        public int NotHandleExceptionCount { get; set; }


        /// <summary>
        /// 最近出现的问题列表
        /// </summary>
        public IEnumerable<T_Question> LatestQuestionList { get; set; }

        /// <summary>
        /// 最近总公司公告列表
        /// </summary>
        public IEnumerable<T_CompanyPost> LatestCompanyPostList { get; set; }
    }
}