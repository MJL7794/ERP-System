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
using ERP.PR;
using Microsoft.Data.SqlClient;

namespace ERP.SE
{
    public partial class FormBrowseSEOrder : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();
        FormSEOutStore formSEOutStore = null;
        FormPRPlan formPRPlan = null;

        public FormBrowseSEOrder()
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
            strSql += "FROM SEOrder " + strWhere;

            try
            {
                this.dgvSEOrderInfo.DataSource = db.GetDataSet(strSql, "SEOrder").Tables["SEOrder"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw ex;
            }
        }

        private void FormBrowseSEOrder_Load(object sender, EventArgs e)
        {
            if (this.Owner.GetType() == typeof(FormSEOutStore))
            {
                formSEOutStore = (FormSEOutStore)this.Owner;
            }

            if (this.Owner.GetType() == typeof(FormPRPlan))
            {
                formPRPlan = (FormPRPlan)this.Owner;
            }
            
            commUse.BindComboBox(this.dgvSEOrderInfo.Columns["OperatorCode"], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvSEOrderInfo.Columns["CustomerCode"], "CustomerCode", "CustomerName", "select CustomerCode,CustomerName from BSCustomer", "BSCustomer");
            commUse.BindComboBox(this.dgvSEOrderInfo.Columns["StoreCode"], "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(this.dgvSEOrderInfo.Columns["InvenCode"], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvSEOrderInfo.Columns["EmployeeCode"], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvSEOrderInfo.Columns["IsFlag"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");

            this.BindDataGridView(" WHERE IsFlag = '1'");

            if (dgvSEOrderInfo.RowCount <= 0)
            {
                gbInfo.Text = "无已审核订单";
            }
        }

        private void dgvSEOrderInfo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSEOrderInfo.RowCount > 0)
            {
                if (formSEOutStore != null)
                {
                    formSEOutStore.cbxCustomerCode.SelectedValue = this.dgvSEOrderInfo["CustomerCode", this.dgvSEOrderInfo.CurrentCell.RowIndex].Value;
                    formSEOutStore.cbxStoreCode.SelectedValue = this.dgvSEOrderInfo["StoreCode", this.dgvSEOrderInfo.CurrentCell.RowIndex].Value;
                    formSEOutStore.cbxInvenCode.SelectedValue = this.dgvSEOrderInfo["InvenCode", this.dgvSEOrderInfo.CurrentCell.RowIndex].Value;
                    formSEOutStore.txtSellPrice.Text = this.dgvSEOrderInfo["SellPrice", this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();
                    formSEOutStore.txtQuantity.Text = this.dgvSEOrderInfo["Quantity", this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();
                    formSEOutStore.txtSEMoney.Text = this.dgvSEOrderInfo["SEMoney", this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();
                    formSEOutStore.txtSEOrderCode.Text = this.dgvSEOrderInfo["SEOrderCode", this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();
                    formSEOutStore.cbxEmployeeCode.SelectedValue = this.dgvSEOrderInfo["EmployeeCode", this.dgvSEOrderInfo.CurrentCell.RowIndex].Value;
                }

                if (formPRPlan != null)
                {
                    string strSEOrderCode = this.dgvSEOrderInfo["SEOrderCode", this.dgvSEOrderInfo.CurrentRow.Index].Value.ToString();
                    DataGridViewRowCollection dgvrc =  formPRPlan.dgvPRPlanInfo.Rows;

                    foreach (DataGridViewRow dgvr in dgvrc)
                    {
                        if (dgvr.Cells["SEOrderCode"] != null)
                        {
                            if (strSEOrderCode == dgvr.Cells["SEOrderCode"].Value.ToString())
                            {
                                MessageBox.Show("该销售订单已制定相应的主生产计划！", "软件提示");
                                return;
                            }
                        }
                    }

                    formPRPlan.txtSEOrderCode.Text = this.dgvSEOrderInfo["SEOrderCode", this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();
                    formPRPlan.cbxInvenCode.SelectedValue = this.dgvSEOrderInfo["InvenCode", this.dgvSEOrderInfo.CurrentCell.RowIndex].Value;
                    formPRPlan.txtQuantity.Text = this.dgvSEOrderInfo["Quantity", this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();      
                }

                this.Close();
            }
        }

        private void dgvSEOrderInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
