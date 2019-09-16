using Property.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Property.UI.Models
{
    public class ExpenseSettingsModel
    {
        public int PropertyPlaceId { get; set; }
        public int BuildId { get; set; }
        public List<SelectListItem> BuildList { get; set; }
        public int UnitId { get; set; }
        public List<SelectListItem> UnitList { get; set; }
        public int ExpenseClassId { get; set; }
        public List<SelectListItem> ExpenseClassList { get; set; }
        public int ExpenseCircleId { get; set; }
        public List<SelectListItem> ExpenseCircleList { get; set; }
        public int ExpenseTypeId { get; set; }
        public int UpdateExpenseTypeId { get; set; }
        public List<SelectListItem> ExpenseTypeList { get; set; }
        public List<HouseUserExpenseTemplateModel> HouseUserExpenseTemplateList { get; set; }
        public DateTime? NotificationDate { get; set; }
        public string GetDoorExpenseVal { get; set; }
        public ExpenseSettingsModel()
        {
            BuildList = new List<SelectListItem>();
            UnitList = new List<SelectListItem>();
            HouseUserExpenseTemplateList = new List<HouseUserExpenseTemplateModel>();

            ExpenseCircleList = new List<SelectListItem>();
            ExpenseCircleList.Add(new SelectListItem() { Text = "每月", Value = "1" });
            ExpenseCircleList.Add(new SelectListItem() { Text = "每两个月", Value = "2" });
            ExpenseCircleList.Add(new SelectListItem() { Text = "每季度", Value = "3" });
            ExpenseCircleList.Add(new SelectListItem() { Text = "每半年", Value = "4" });
            ExpenseCircleList.Add(new SelectListItem() { Text = "每年", Value = "5" });

            ExpenseClassList = new List<SelectListItem>();
            ExpenseClassList.Add(new SelectListItem() { Text = "非固定缴费", Value = "0" });
            ExpenseClassList.Add(new SelectListItem() { Text = "固定缴费", Value = "1" });
        }
    }

    public class HouseUserExpenseTemplateModel
    {
        public int index { get; set; }
        public int DoorId { get; set; }
        public string DoorName { get; set; }
        //public int UnitId { get; set; }
        //public int BuildId { get; set; }
        //public int PropertyPlaceId { get; set; }
        public string Expense { get; set; }
        //public int ExpenseCircleId { get; set; }
        //public int ExpenseTypeId { get; set; }
        //public int ExpenseClassId { get; set; }
    }
}