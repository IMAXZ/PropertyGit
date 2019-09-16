﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Property.Entity
{
    /// <summary>
    /// 物业小区账户表实体类
    /// </summary>
    public class T_PropertyAccount
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 所属物业小区ID
        /// </summary>
        public int PropertyPlaceId { get; set; }

        /// <summary>
        /// 所属物业小区关联对象
        /// </summary>
        [ForeignKey("PropertyPlaceId")]
        public virtual T_PropertyPlace PropertyPlace { get; set; }

        /// <summary>
        /// 账户类型 1:微信 2:支付宝
        /// </summary>
        public int AccountType { get; set; }

        /// <summary>
        /// 支付宝账号（微信AppID）
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Number { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string MerchantNo { get; set; }

        /// <summary>
        /// 账户密钥
        /// </summary>
        [Required]
        public string AccountKey { get; set; }

        /// <summary>
        /// 秘钥(私钥)文件路径
        /// </summary>
        [MaxLength(200)]
        public string PrivatePath { get; set; }

        /// <summary>
        /// 公钥文件路径
        /// </summary>
        [MaxLength(200)]
        public string PublicPath { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public Nullable<DateTime> CreatedDate { get; set; }

        /// <summary>
        /// 删除标识 0:未删除 1:删除
        /// </summary>
        public int DelFlag { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Memo { get; set; }
    }
}
