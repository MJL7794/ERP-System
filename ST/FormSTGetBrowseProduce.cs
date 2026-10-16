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
    public partial class FormSTGetBrowseProduce : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();
        FormSTGetMaterial formSTGetMaterial = null;

        public FormSTGetBrowseProduce()
        {
            InitializeComponent();
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        /// <param name="strTable">数据表名称</param>
        /// <param name="dgv">DataGridView控件的实例的名称</param>
        private void BindDataGridView(string strWhere ,string strTable,DataGridView dgv)
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

        private void FormSTGetBrowseProduce_Load(object sender, EventArgs e)
        {
            formSTGetMaterial = (FormSTGetMaterial)this.Owner;

            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["OperatorCode"], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["DepartmentCode"], "DepartmentCode", "DepartmentName", "select DepartmentCode,DepartmentName from BSDepartment", "BSDepartment");
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["InvenCode"], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["EmployeeCode"], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["IsFlag"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["IsComplete"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
            commUse.BindComboBox(this.dgvPRProduceItemInfo.Columns["InvenCode_Item"], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");

            this.BindDataGridView(" WHERE IsFlag = '1' AND IsComplete = '0'", "PRProduce", dgvPRProduceInfo);

            if (dgvPRProduceInfo.RowCount > 0)
            {
                string strPRProduceCode = this.dgvPRProduceInfo["PRProduceCode", 0].Value.ToString();
                this.BindDataGridView(" WHERE PRProduceCode =  '" + strPRProduceCode + "'", "PRProduceItem", dgvPRProduceItemInfo);
            }
        }

        private void dgvPRProduceInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPRProduceInfo.RowCount > 0) //单击主表记录，检索子表记录信息
            {
                string strPRProduceCode = this.dgvPRProduceInfo["PRProduceCode", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value.ToString();
                this.BindDataGridView(" WHERE PRProduceCode =  '" + strPRProduceCode + "'", "PRProduceItem", dgvPRProduceItemInfo);
            }
        }

        private void dgvPRProduceItemInfo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPRProduceItemInfo.RowCount > 0) //双击子表记录
            {
                formSTGetMaterial.txtPRProduceCode.Text = dgvPRProduceItemInfo["PRProduceCode_Item", dgvPRProduceItemInfo.CurrentCell.RowIndex].Value.ToString();
                formSTGetMaterial.cbxInvenCode.SelectedValue = dgvPRProduceItemInfo["InvenCode_Item", dgvPRProduceItemInfo.CurrentCell.RowIndex].Value;
                formSTGetMaterial.txtQuantity.Text = dgvPRProduceItemInfo["Quantity_Item", dgvPRProduceItemInfo.CurrentCell.RowIndex].Value.ToString();
                this.Close();
            }
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
