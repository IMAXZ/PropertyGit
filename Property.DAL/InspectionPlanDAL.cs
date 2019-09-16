using Property.Common;
using Property.Entity;
using Property.FactoryBLL;
using Property.IBLL;
using Property.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.DAL
{
    /// <summary>
    /// 巡检计划数据层访问类
    /// </summary>
    public class InspectionPlanDAL : BaseDAL<T_InspectionPlan>, IInspectionPlanDAL
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
        public bool UpdateInspectionPlan(T_InspectionPlan plan, int[] PointIds, int?[] StartHourNums, int?[] EndHourNums,
            int?[] StartWeekNums, int?[] EndWeekNums, int?[] StartDayNums, int?[] EndDayNums)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //删除该巡检计划对应的巡检计划巡检点关联对象
                    this.nContext.Database.ExecuteSqlCommand("delete from R_PlanPoint where PlanId=" + plan.Id);

                    //删除该巡检任务下的所有日巡检安排或周巡检安排或月巡检安排
                    this.nContext.Database.ExecuteSqlCommand("delete from T_InspectionTimePlan where PlanId=" + plan.Id);

                    //如果不随机，则记录巡检安排
                    if (plan.IsRandom == ConstantParam.DELIVERY_FLAG_FALSE)
                    {
                        int Numbers = 0;
                        //如果是日巡检
                        if (plan.Type == ConstantParam.INSPECTION_TYPE_DAY)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                if (StartHourNums[i] != null && EndHourNums[i] != null)
                                {
                                    plan.TimePlans.Add(new T_InspectionTimePlan()
                                    {
                                        BeginNum = StartHourNums[i].Value,
                                        EndNum = EndHourNums[i].Value,
                                        Number = i + 1
                                    });
                                    Numbers++;
                                }
                            }
                            plan.Number = Numbers;
                        }
                        //如果是周巡检
                        else if (plan.Type == ConstantParam.INSPECTION_TYPE_WEEK)
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                if (StartWeekNums[i] != null)
                                {
                                    plan.TimePlans.Add(new T_InspectionTimePlan()
                                    {
                                        BeginNum = StartWeekNums[i].Value,
                                        Number = i + 1
                                    });
                                    Numbers++;
                                }
                            }
                            plan.Number = Numbers;
                        }
                        else
                        {
                            //月巡检
                            for (int i = 0; i < 10; i++)
                            {
                                if (StartDayNums[i] != null)
                                {
                                    plan.TimePlans.Add(new T_InspectionTimePlan()
                                    {
                                        BeginNum = StartDayNums[i].Value,
                                        Number = i + 1
                                    });
                                    Numbers++;
                                }
                            }
                            plan.Number = Numbers;
                        }
                    }
                    else
                    {
                        //如果是随机巡检
                        for (int i = 0; i < plan.Number; i++)
                        {
                            plan.TimePlans.Add(new T_InspectionTimePlan()
                            {
                                Number = i + 1
                            });
                        }
                    }
                    //重新添加巡检点
                    foreach (var item in PointIds)
                    {
                        plan.PlanPoints.Add(new R_PlanPoint()
                        {
                            PointId = item
                        });
                    }
                    //更新
                    base.Update(plan);
                    //提交事务
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 删除巡检计划
        /// </summary>
        /// <param name="plan">要删除的巡检计划对象</param>
        /// <returns></returns>
        public bool DeleteInspectionPlan(T_InspectionPlan plan)
        {
            //使用事务进行数据库操作
            using (var tran = this.nContext.Database.BeginTransaction())
            {
                try
                {
                    //如果未发布
                    if (plan.PublishedFlag == ConstantParam.PUBLISHED_FALSE) 
                    {
                        //删除该巡检计划对应的巡检计划巡检点关联对象
                        this.nContext.Database.ExecuteSqlCommand("delete from R_PlanPoint where PlanId=" + plan.Id);

                        //删除该巡检任务下的所有时间安排
                        this.nContext.Database.ExecuteSqlCommand("delete from T_InspectionTimePlan where PlanId=" + plan.Id);

                        //真正删除
                        base.Delete(plan);
                    }
                    else 
                    {
                        //将巡检结果都更改为删除标识
                        IInspectionResultBLL resultBll = BLLFactory<IInspectionResultBLL>.GetBLL("InspectionResultBLL");
                        foreach (var TimePlan in plan.TimePlans)
                        {
                            foreach (var res in TimePlan.InspectionResults)
                            {
                                res.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                                resultBll.Update(res);
                            }
                        }
                        plan.DelFlag = ConstantParam.DEL_FLAG_DELETE;
                        //更新
                        base.Update(plan);   
                    }
                    //提交事务
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    return false;
                }
            }
            return true;
        }
    }
}
