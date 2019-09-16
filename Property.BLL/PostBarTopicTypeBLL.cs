using Property.Entity;
using Property.IBLL;
using Property.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.BLL
{
    /// <summary>
    /// 小区沟通主题分类业务层访问类
    /// </summary>
    public class PostBarTopicTypeBLL : BaseBLL<T_PostBarTopicType>, IPostBarTopicTypeBLL
    {
        private const string _Type = "PostBarTopicTypeDAL";

        private IPostBarTopicTypeDAL _Dal;

        public PostBarTopicTypeBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPostBarTopicTypeDAL;
        }
    }
}
