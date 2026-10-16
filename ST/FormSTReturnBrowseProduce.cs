using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using ERP.ComClass;
using ERP.DataClass;

namespace ERP.ST
{
    public partial class FormSTReturnBrowseProduce : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();
        FormSTReturnMaterial formSTReturnMaterial = null;

        public FormSTReturnBrowseProduce()
        {
            InitializeComponent();
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        /// <param name="strTable">数据表的名称</param>
        /// <param name="dgv">DataGridView控件的实例的名称</param>
        private void BindDataGridView(string strWhere, string strTable, DataGridView dgv)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM " + strTable + strWhere;

            try
            {
                dgv.DataSource = db.GetDataSet(strSql, strTable).Tables[strTable];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw ex;
            }
        }

        private void FormSTReturnBrowseProduce_Load(object sender, EventArgs e)
        {
            formSTReturnMaterial = (FormSTReturnMaterial)this.Owner;
            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["OperatorCode"], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["DepartmentCode"], "DepartmentCode", "DepartmentName", "select DepartmentCode,DepartmentName from BSDepartment", "BSDepartment");
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["InvenCode"], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["EmployeeCode"], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["IsFlag"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["IsComplete"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
            commUse.BindComboBox(this.dgvPRProduceItemInfo.Columns["InvenCode_Item"], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            //
            this.BindDataGridView(" WHERE IsFlag = '1'", "PRProduce", dgvPRProduceInfo);

            if (dgvPRProduceInfo.RowCount > 0)
            {
                string strPRProduceCode = this.dgvPRProduceInfo["PRProduceCode", 0].Value.ToString();
                this.BindDataGridView(" WHERE PRProduceCode =  '" + strPRProduceCode + "'", "PRProduceItem", dgvPRProduceItemInfo);
            }
        }

        private void dgvPRProduceInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPRProduceInfo.RowCount > 0)
            {
                string strPRProduceCode = this.dgvPRProduceInfo["PRProduceCode", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value.ToString();
                this.BindDataGridView(" WHERE PRProduceCode =  '" + strPRProduceCode + "'", "PRProduceItem", dgvPRProduceItemInfo);
            }
        }

        private void dgvPRProduceItemInfo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string strIsComplete = null;
            int intMaxReturnQuantity = 0; //最大可退料量
            Nullable<int> intGetQuantity = null; //领料量
            Nullable<int> intUseQuantity = null; //使用量

            if (dgvPRProduceItemInfo.RowCount == 0)
            {
                return;
            }

            //获取领料量
            if(!Convert.IsDBNull(this.dgvPRProduceItemInfo["GetQuantity",this.dgvPRProduceItemInfo.CurrentCell.RowIndex].Value ))
            {
                intGetQuantity = Convert.ToInt32(this.dgvPRProduceItemInfo["GetQuantity",this.dgvPRProduceItemInfo.CurrentCell.RowIndex].Value);
            }

            //获取使用量
            if (!Convert.IsDBNull(this.dgvPRProduceItemInfo["UseQuantity", this.dgvPRProduceItemInfo.CurrentCell.RowIndex].Value))
            {
                intUseQuantity = Convert.ToInt32(this.dgvPRProduceItemInfo["UseQuantity", this.dgvPRProduceItemInfo.CurrentCell.RowIndex].Value);
            }

            strIsComplete = this.dgvPRProduceInfo["IsComplete", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value.ToString();
            
            //未完工
            if (strIsComplete == "0")
            {
                if (!intGetQuantity.HasValue || intGetQuantity.Value == 0)
                {
                    MessageBox.Show("未领料，无法退料！", "软件提示");
                    return;
                }
                else
                {
                    intMaxReturnQuantity = intGetQuantity.Value ;
                }
            }

            //已完工
            if (strIsComplete == "1")
            {
                if (intUseQuantity == intGetQuantity)
                {
                    MessageBox.Show("领用的物料已经被完全使用掉，无法退料！", "软件提示");
                    return;
                }
                else
                {
                    intMaxReturnQuantity = intGetQuantity.Value - intUseQuantity.Value;
                }
            }

            formSTReturnMaterial.txtPRProduceCode.Text = dgvPRProduceItemInfo["PRProduceCode_Item", dgvPRProduceItemInfo.CurrentCell.RowIndex].Value.ToString();
            formSTReturnMaterial.cbxInvenCode.SelectedValue = dgvPRProduceItemInfo["InvenCode_Item", dgvPRProduceItemInfo.CurrentCell.RowIndex].Value;
            formSTReturnMaterial.txtQuantity.Text = intMaxReturnQuantity.ToString();
            formSTReturnMaterial.intMaxReturnQuantity = intMaxReturnQuantity;

            this.Close();
        }

        private void dgvPRProduceInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void dgvPRProduceItemInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
