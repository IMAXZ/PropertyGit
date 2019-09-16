using Property.Common;
using Property.FactoryBLL;
using Property.IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.ShopRejectOrderServer
{
    public class RejectOrderManage
    {
        public static bool RejectOrders()
        {
            try
            {
                //CommonUtils.WriteLog("进入到RejectOrders方法内部");
                //调用定单接口
                IOrderBLL templateBLL = BLLFactory<IOrderBLL>.GetBLL("OrderBLL");
                var twoHourAgoTime = DateTime.Now.AddHours(-2);

                //找出所有超过2小时商家没有接单的
                var orders = templateBLL.GetList(o => o.OrderStatus == ConstantParam.OrderStatus_CONFIRM && o.PayDate < twoHourAgoTime).ToList();
                //CommonUtils.WriteLog("成功连接数据库成功");
                //修改订单状态为退单状态

                foreach (var order in orders)
                {
                    order.OrderStatus = ConstantParam.OrderStatus_EXIT;
                    order.RecedeType = 1;
                    order.RecedeTime = DateTime.Now;
                    order.Reason = "商家2小时未接单，系统自动退单";
                    var alert = "您在" + order.Shop.ShopName + "提交的订单因商家2小时未接单已自动退单";

                    if (templateBLL.CancelOrder(order))
                    {
                        IUserPushBLL userPushBLL = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                        var userPush = userPushBLL.GetEntity(p => p.UserId == order.AppUserId);

                        if (userPush != null)
                        {
                            string registrationId = userPush.RegistrationId;
                            //通知信息
                            PropertyUtils.SendPush("订单最新状态", alert, ConstantParam.MOBILE_TYPE_OWNER, registrationId);
                        }
                    }
                }

                var twoDayAgoTime = DateTime.Now.AddDays(-2);

                var end = twoDayAgoTime.AddHours(2);
                var start = end.AddMinutes(-2);
                //获取所有提交订单超过46小时，且没有付款没有提醒的订单
                var noPayAndRemindOrders = templateBLL.GetList(o => o.OrderStatus == ConstantParam.OrderStatus_NOPAY && o.OrderDate < end && o.OrderDate >= start).ToList();
                foreach (var order in noPayAndRemindOrders)
                {
                    var alert = "您在" + order.Shop.ShopName + "提交的订单还未付款，2小时后订单会自动关闭";
                    IUserPushBLL userPushBLL = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                    var userPush = userPushBLL.GetEntity(p => p.UserId == order.AppUserId);

                    if (userPush != null)
                    {
                        string registrationId = userPush.RegistrationId;
                        //通知信息
                        PropertyUtils.SendPush("订单最新状态", alert, ConstantParam.MOBILE_TYPE_OWNER, registrationId);
                    }
                }


                //获取所有提交订单超过2天，且没有付款的订单
                var noPayOrders = templateBLL.GetList(o => o.OrderStatus == ConstantParam.OrderStatus_NOPAY && o.OrderDate < twoDayAgoTime).ToList();
                foreach (var order in noPayOrders)
                {
                    order.OrderStatus = ConstantParam.OrderStatus_CLOSE;
                    var alert = "您在" + order.Shop.ShopName + "提交的订单因长时间不付款已自动关闭";

                    if (templateBLL.CancelOrder(order))
                    {
                        IUserPushBLL userPushBLL = BLLFactory<IUserPushBLL>.GetBLL("UserPushBLL");
                        var userPush = userPushBLL.GetEntity(p => p.UserId == order.AppUserId);

                        if (userPush != null)
                        {
                            string registrationId = userPush.RegistrationId;
                            //通知信息
                            PropertyUtils.SendPush("订单最新状态", alert, ConstantParam.MOBILE_TYPE_OWNER, registrationId);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                CommonUtils.WriteLogError(ex);
                return false;
            }
        }
    }
}
