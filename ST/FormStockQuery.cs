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

namespace ERP.ST
{
    public partial class FormStockQuery : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();
        
        public FormStockQuery()
        {
            InitializeComponent();
        }

        /// <summary>
        ///DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strOtherCondition">Where条件子句</param>
        private void BindDataGridView(string strOtherCondition)
        {
            string strSql = null;

            strSql = "Select BSStore.StoreName,BSInvenType.InvenTypeName,BSInven.InvenCode,BSInven.InvenName,BSInven.SpecsModel,BSInven.MeaUnit,STStock.Quantity,STStock.AvePrice,STStock.STMoney,STStock.LossQuantity,STStock.LossMoney ";
            strSql += "From STStock,BSStore,BSInven,BSInvenType ";
            strSql += "Where STStock.StoreCode = BSStore.StoreCode and STStock.InvenCode = BSInven.InvenCode and BSInven.InvenTypeCode = BSInvenType.InvenTypeCode " + strOtherCondition;

            try
            {
                this.dgvStockQueryInfo.DataSource = db.GetDataSet(strSql, "StockQuery").Tables["StockQuery"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw ex;
            }
        }

        private void FormStockQuery_Load(object sender, EventArgs e)
        {
            //权限
            commUse.CortrolButtonEnabled(toolQuery, this);
            //ComboBox绑定到数据源
            commUse.BindComboBox(cbxStoreCode, "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(cbxInvenCode, "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            //
            cbxStoreCode.SelectedIndex = -1;
            cbxInvenCode.SelectedIndex = -1;
            toolQuery_Click(sender, e);
        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolExport_Click(object sender, EventArgs e)
        {
            commUse.DataGridViewExportToExcel(dgvStockQueryInfo, this.Text);
        }

        private void toolQuery_Click(object sender, EventArgs e)
        {
            StringBuilder strSearchCondition = new StringBuilder("");

            //存货代码
            if (cbxInvenCode.SelectedValue != null)
            {
                strSearchCondition.Append(" and STStock.InvenCode = '" + cbxInvenCode.SelectedValue + "'");
            }

            //仓库代码
            if (cbxStoreCode.SelectedValue != null)
            {
                strSearchCondition.Append(" and STStock.StoreCode = '" + cbxStoreCode.SelectedValue + "'");
            }

            BindDataGridView(strSearchCondition.ToString());
        }

        private void toolCancel_Click(object sender, EventArgs e)
        {
            cbxStoreCode.SelectedIndex = -1;
            cbxInvenCode.SelectedIndex = -1;
        }

        private void dgvStockQueryInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
