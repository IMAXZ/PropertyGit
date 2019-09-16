using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Property.UI.Models
{
    public class ImportUsersModel
    {
        public bool HasImportTemplate { get; set; }
        public HttpPostedFileBase file { get; set; }
    }

    public class ImportHouseUserModel
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string BuildName { get; set; }
        public string UnitName { get; set; }
        public string DoorName { get; set; }
        public string Desc { get; set; }
    }
}