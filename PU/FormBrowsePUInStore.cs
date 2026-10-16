using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ERP.ComClass;
using ERP.DataClass;
using Microsoft.Data.SqlClient;

namespace ERP.PU
{
    public partial class FormBrowsePUInStore : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();
        FormPUPay formPUPay = null;

        public FormBrowsePUInStore()
        {
            InitializeComponent();
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM PUInStore " + strWhere;

            try
            {
                this.dgvPUInStoreInfo.DataSource = db.GetDataSet(strSql, "PUInStore").Tables["PUInStore"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw ex;
            }
        }

        private void FormBrowsePUInStore_Load(object sender, EventArgs e)
        {
            formPUPay = (FormPUPay)this.Owner;

            commUse.BindComboBox(this.dgvPUInStoreInfo.Columns[2], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvPUInStoreInfo.Columns[3], "SupplierCode", "SupplierName", "select SupplierCode,SupplierName from BSSupplier", "BSSupplier");
            commUse.BindComboBox(this.dgvPUInStoreInfo.Columns[4], "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(this.dgvPUInStoreInfo.Columns[5], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvPUInStoreInfo.Columns[10], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvPUInStoreInfo.Columns[11], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");

            this.BindDataGridView(" WHERE IsFlag = '1'");

            if (dgvPUInStoreInfo.RowCount <= 0)
            {
                gbInfo.Text = "无已审核采购入库单";
            }
        }

        private void dgvPUInStoreInfo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPUInStoreInfo.RowCount > 0)
            {
                formPUPay.txtPUInCode.Text = this.dgvPUInStoreInfo["PUInCode", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();
                formPUPay.dtpPUInDate.Value = Convert.ToDateTime(this.dgvPUInStoreInfo["PUInDate", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value);
                formPUPay.cbxSupplierCode.SelectedValue = this.dgvPUInStoreInfo["SupplierCode", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value;
                formPUPay.txtPUMoney.Text = this.dgvPUInStoreInfo["PUMoney", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();
                this.Close();
            }
        }

        private void dgvPUInStoreInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
