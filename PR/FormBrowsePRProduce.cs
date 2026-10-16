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

namespace ERP.PR
{
    public partial class FormBrowsePRProduce : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();
        FormProduceComplete formProduceComplete = null;
        FormPRInStore formPRInStore = null;

        public FormBrowsePRProduce()
        {
            InitializeComponent();
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strSql">Transact-SQL语句</param>
        /// <param name="strTable">数据表名称</param>
        /// <param name="dgv">DataGridView控件的实例的名称</param>
        private void BindDataGridView(string strSql,string strTable, DataGridView dgv)
        {  
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

        private void FormBrowsePRProduce_Load(object sender, EventArgs e)
        {
            string strSql = null;

            if (this.Owner.GetType() == typeof(FormProduceComplete))
            {
                formProduceComplete = (FormProduceComplete)this.Owner;
                strSql = "Select * From PRProduce Where IsFlag = '1'";
                this.Text = "浏览已审核生产单";
            }

            if (this.Owner.GetType() == typeof(FormPRInStore))
            {
                formPRInStore = (FormPRInStore)this.Owner;
                strSql = "Select * From PRProduce Where IsComplete = '1'"; //完工的，一定是审核的
                this.Text = "浏览完工生产单";
            }

            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["OperatorCode"], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["DepartmentCode"], "DepartmentCode", "DepartmentName", "select DepartmentCode,DepartmentName from BSDepartment", "BSDepartment");
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["InvenCode"], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["EmployeeCode"], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["IsFlag"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
            commUse.BindComboBox(this.dgvPRProduceInfo.Columns["IsComplete"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
            
            this.BindDataGridView(strSql, "PRProduce", dgvPRProduceInfo);
        }

        private void dgvPRProduceInfo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPRProduceInfo.RowCount > 0)
            {
                if (formProduceComplete != null)
                {
                    string strPRProduceCode = this.dgvPRProduceInfo["PRProduceCode", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value.ToString();
                    object objIsComplete = this.dgvPRProduceInfo["IsComplete", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value;

                    formProduceComplete.txtPRProduceCode.Text = strPRProduceCode;
                    formProduceComplete.dtpPRProduceDate.Value = Convert.ToDateTime(this.dgvPRProduceInfo["PRProduceDate", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value);
                    formProduceComplete.cbxOperatorCode.SelectedValue = this.dgvPRProduceInfo["OperatorCode", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value;
                    formProduceComplete.txtPRPlanCode.Text = this.dgvPRProduceInfo["PRPlanCode", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value.ToString();
                    formProduceComplete.cbxDepartmentCode.SelectedValue = this.dgvPRProduceInfo["DepartmentCode", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value;
                    formProduceComplete.cbxInvenCode.SelectedValue = this.dgvPRProduceInfo["InvenCode", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value;
                    formProduceComplete.txtQuantity.Text = this.dgvPRProduceInfo["Quantity", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value.ToString();
                    formProduceComplete.dtpStartDate.Value = Convert.ToDateTime(this.dgvPRProduceInfo["StartDate", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value);
                    formProduceComplete.dtpEndDate.Value = Convert.ToDateTime(this.dgvPRProduceInfo["EndDate", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value);
                    formProduceComplete.cbxIsComplete.SelectedValue = objIsComplete;

                    string strSql = "Select PRProduceItem.Id, PRProduceItem.PRProduceCode,PRProduceItem.InvenCode, BSInven.InvenName, PRProduceItem.Quantity, PRProduceItem.GetQuantity, PRProduceItem.UseQuantity ";
                    strSql += "From PRProduceItem,BSInven Where PRProduceItem.InvenCode = BSInven.InvenCode and PRProduceItem.PRProduceCode = '" + strPRProduceCode + "'";
                    BindDataGridView(strSql, "PRProduceItem", formProduceComplete.dgvPRProduceItemInfo);

                    if (objIsComplete.ToString() == "1")
                    {
                        formProduceComplete.dgvPRProduceItemInfo.Columns["UseQuantity"].ReadOnly = true;
                    }
                    else
                    {
                        formProduceComplete.dgvPRProduceItemInfo.Columns["UseQuantity"].ReadOnly = false;
                    }
                }

                if (formPRInStore != null)
                {
                    string strPRProduceCode = this.dgvPRProduceInfo["PRProduceCode", this.dgvPRProduceInfo.CurrentRow.Index].Value.ToString();
                    DataGridViewRowCollection dgvrc = formPRInStore.dgvPRInStoreInfo.Rows;

                    foreach (DataGridViewRow dgvr in dgvrc)
                    {
                        //判断该笔生产单是否已生成相应的产品入库单
                        if (strPRProduceCode == dgvr.Cells["PRProduceCode"].Value.ToString())
                        {
                            MessageBox.Show("该生产单已生成相应的生产入库单！", "软件提示");
                            return;
                        }
                    }

                    formPRInStore.txtPRProduceCode.Text = this.dgvPRProduceInfo["PRProduceCode", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value.ToString();
                    formPRInStore.cbxInvenCode.SelectedValue = this.dgvPRProduceInfo["InvenCode", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value;
                    formPRInStore.txtPRQuantity.Text = this.dgvPRProduceInfo["Quantity", this.dgvPRProduceInfo.CurrentCell.RowIndex].Value.ToString();
                    formPRInStore.txtInQuantity.Text = formPRInStore.txtPRQuantity.Text;
                }

                this.Close();
            }
        }

        private void dgvPRProduceInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
