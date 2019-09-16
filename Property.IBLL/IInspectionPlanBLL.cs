using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.IBLL
{
    /// <summary>
    /// 巡检计划业务层访问接口
    /// </summary>
    public interface IInspectionPlanBLL : IBaseBLL<T_InspectionPlan>
    {
        /// <summary>
        /// 更新巡检计划
        /// </summary>
        /// <param name="plan">新的巡检计划对象</param>
        /// <param name="PointIds">新的巡检点数组</param>
        /// <param name="StartHourNums">日巡检开始时间数组</param>
        /// <param name="EndHourNums">日巡检结束时间数组</param>
        /// <param name="StartWeekNums">周巡检开始时间数组</param>
        /// <param name="EndWeekNums">周巡检结束时间数组</param>
        /// <param name="StartDayNums">月巡检开始时间数组</param>
        /// <param name="EndDayNums">月巡检结束时间数组</param>
        /// <returns>是否更新成功</returns>
        bool UpdateInspectionPlan(T_InspectionPlan plan, int[] PointIds, int?[] StartHourNums, int?[] EndHourNums,
            int?[] StartWeekNums, int?[] EndWeekNums, int?[] StartDayNums, int?[] EndDayNums);

        /// <summary>
        /// 删除巡检计划
        /// </summary>
        /// <param name="plan">要删除的巡检计划对象</param>
        /// <returns></returns>
        bool DeleteInspectionPlan(T_InspectionPlan plan);
    }
}
