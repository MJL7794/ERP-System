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

namespace ERP.SE
{
    public partial class FormBrowseSEOutStore : Form
    {

        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();
        FormSEGather formSEGather = null;

        public FormBrowseSEOutStore()
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
            strSql += "FROM SEOutStore " + strWhere;

            try
            {
                this.dgvSEOutStoreInfo.DataSource = db.GetDataSet(strSql, "SEOutStore").Tables["SEOutStore"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw ex;
            }
        }

        private void FormBrowseSEOutStore_Load(object sender, EventArgs e)
        {
            //获取拥有已审核的销售出库单窗体的窗体，即销售收款单窗体
            formSEGather = (FormSEGather)this.Owner;
            //DataGridView控件的相关列绑定到数据源（包括操作员、客户名称、物料名称、仓库、员工名称、审核标记）
            commUse.BindComboBox(this.dgvSEOutStoreInfo.Columns["OperatorCode"], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvSEOutStoreInfo.Columns["CustomerCode"], "CustomerCode", "CustomerName", "select CustomerCode,CustomerName from BSCustomer", "BSCustomer");
            commUse.BindComboBox(this.dgvSEOutStoreInfo.Columns["StoreCode"], "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(this.dgvSEOutStoreInfo.Columns["InvenCode"], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvSEOutStoreInfo.Columns["EmployeeCode"], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvSEOutStoreInfo.Columns["IsFlag"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");

            this.BindDataGridView(" WHERE IsFlag = '1'");//检索已审核的销售出库单

            if (dgvSEOutStoreInfo.RowCount <= 0)//若无数据行，则系统给与提示信息
            {
                gbInfo.Text = "无已审核销售出库单";
            }
        }

        private void dgvSEOutStoreInfo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSEOutStoreInfo.RowCount > 0)//若存在已审核的出库单
            {
                //为窗体FormSEGather上面的某些控件传值
                //设置出库单号
                formSEGather.txtSEOutCode.Text = this.dgvSEOutStoreInfo["SEOutCode", this.dgvSEOutStoreInfo.CurrentCell.RowIndex].Value.ToString();
                //设置出库日期
                formSEGather.dtpSEOutDate.Value = Convert.ToDateTime(this.dgvSEOutStoreInfo["SEOutDate", this.dgvSEOutStoreInfo.CurrentCell.RowIndex].Value);
                //设置客户
                formSEGather.cbxCustomerCode.SelectedValue = this.dgvSEOutStoreInfo["CustomerCode", this.dgvSEOutStoreInfo.CurrentCell.RowIndex].Value;
                //设置默认的收款金额
                formSEGather.txtSEMoney.Text = this.dgvSEOutStoreInfo["SEMoney", this.dgvSEOutStoreInfo.CurrentCell.RowIndex].Value.ToString();
                this.Close();//关闭当前窗口
            }
        }

        private void dgvSEOutStoreInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
