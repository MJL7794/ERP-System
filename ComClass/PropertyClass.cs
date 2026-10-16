using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.ComClass
{
    /// <summary>
    /// 表示若干属性的类
    /// </summary>
    public class PropertyClass
    {
        private static string m_OperatorCode;
        /// <summary>
        /// 操作员代码
        /// </summary>
        public static string OperatorCode
        {
            get
            {
                return m_OperatorCode;
            }
            set
            {
                m_OperatorCode = value;
            }
        }

        private static string m_OperatorName;
        /// <summary>
        /// 操作员名称
        /// </summary>
        public static string OperatorName
        {
            get
            {
                return m_OperatorName;
            }
            set
            {
                m_OperatorName = value;
            }
        }

        private static string m_PassWord;
        /// <summary>
        /// 操作员密码
        /// </summary>
        public static string PassWord
        {
            get
            {
                return m_PassWord;
            }
            set
            {
                m_PassWord = value;
            }
        }

        private static string m_IsAdmin;
        /// <summary>
        /// 是否系统管理员标记
        /// </summary>
        public static string IsAdmin
        {
            get
            {
                return m_IsAdmin;
            }
            set
            {
                m_IsAdmin = value;
            }
        }

        private string m_InvenCode;
        /// <summary>
        /// 存货代码
        /// </summary>
        public string InvenCode
        {
            get
            {
                return m_InvenCode;
            }
            set
            {
                m_InvenCode = value;
            }
        }

        private string m_InvenName;
        /// <summary>
        /// 存货名称
        /// </summary>
        public string InvenName
        {
            get
            {
                return m_InvenName;
            }
            set
            {
                m_InvenName = value;
            }
        }

        private string m_SpecsModel;
        /// <summary>
        /// 规格型号
        /// </summary>
        public string SpecsModel
        {
            get
            {
                return m_SpecsModel;
            }
            set
            {
                m_SpecsModel = value;
            }
        }

        private string m_ProInvenCode;
        /// <summary>
        /// Bom中母件的代码
        /// </summary>
        public string ProInvenCode
        {
            get
            {
                return m_ProInvenCode;
            }
            set
            {
                m_ProInvenCode = value;
            }
        }

        private string m_MatInvenCode;
        /// <summary>
        /// Bom中子件的代码
        /// </summary>
        public string MatInvenCode
        {
            get
            {
                return m_MatInvenCode;
            }
            set
            {
                m_MatInvenCode = value;
            }
        }
    }
}
