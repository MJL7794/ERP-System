using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ERP.DataClass;
using ERP.ComClass;
using Microsoft.Data.SqlClient;

namespace ERP.PU
{
    public partial class FormBrowsePUOrder : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();
        FormPUInStore formPUInStore = null;

        public FormBrowsePUOrder()
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
            strSql += "FROM PUOrder " + strWhere;

            try
            {
                this.dgvPUOrderInfo.DataSource = db.GetDataSet(strSql, "PUOrder").Tables["PUOrder"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw ex;
            }
        }

        private void FormBrowsePUOrder_Load(object sender, EventArgs e)
        {
            formPUInStore = (FormPUInStore)this.Owner;

            commUse.BindComboBox(this.dgvPUOrderInfo.Columns[2], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvPUOrderInfo.Columns[3], "SupplierCode", "SupplierName", "select SupplierCode,SupplierName from BSSupplier", "BSSupplier");
            commUse.BindComboBox(this.dgvPUOrderInfo.Columns[4], "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(this.dgvPUOrderInfo.Columns[5], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvPUOrderInfo.Columns[10], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvPUOrderInfo.Columns[11], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
            
            this.BindDataGridView(" WHERE IsFlag = '1'");

            if (dgvPUOrderInfo.RowCount <= 0)
            {
                gbInfo.Text = "无已审核订单";
            }
        }

        private void dgvPUOrderInfo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPUOrderInfo.RowCount > 0)
            {
                formPUInStore.cbxSupplierCode.SelectedValue = this.dgvPUOrderInfo[3, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value;
                formPUInStore.cbxStoreCode.SelectedValue = this.dgvPUOrderInfo[4, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value;
                formPUInStore.cbxInvenCode.SelectedValue = this.dgvPUOrderInfo[5, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value;
                formPUInStore.txtUnitPrice.Text = this.dgvPUOrderInfo[6, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value.ToString();
                formPUInStore.txtQuantity.Text = this.dgvPUOrderInfo[7, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value.ToString();
                formPUInStore.txtPUMoney.Text = this.dgvPUOrderInfo[8, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value.ToString();
                formPUInStore.txtPUOrderCode.Text = this.dgvPUOrderInfo[0, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value.ToString();
                this.Close();
            }
        }

        private void dgvPUOrderInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
