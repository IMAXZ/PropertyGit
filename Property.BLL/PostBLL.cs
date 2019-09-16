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
    /// 新闻公告业务逻辑类
    /// </summary>
    public class PostBLL : BaseBLL<T_Post>, IPostBLL
    {
        private const string _Type = "PostDAL";

        private IPostDAL _Dal;

        public PostBLL()
            : base(_Type)
        {
            this._Dal = base.CurrentDAL as IPostDAL;
        }
    }
}
