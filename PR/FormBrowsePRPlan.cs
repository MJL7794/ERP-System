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

namespace ERP.PR
{
    public partial class FormBrowsePRPlan : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();
        FormPRProduce formPRProduce = null;

        public FormBrowsePRPlan()
        {
            InitializeComponent();
        }

        /// <summary>
        /// DataGridView绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM PRPlan " + strWhere;

            try
            {
                this.dgvPRPlanInfo.DataSource = db.GetDataSet(strSql, "PRPlan").Tables["PRPlan"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw ex;
            }
        }

        private void FormBrowsePRPlan_Load(object sender, EventArgs e)
        {
            formPRProduce = (FormPRProduce)this.Owner;
            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvPRPlanInfo.Columns["OperatorCode"], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvPRPlanInfo.Columns["InvenCode"], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvPRPlanInfo.Columns["IsFlag"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");

            this.BindDataGridView(" WHERE IsFlag = '1'");

            if (dgvPRPlanInfo.RowCount <= 0)
            {
                gbInfo.Text = "无已审核订单";
            }
        }

        private void dgvPRPlanInfo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPRPlanInfo.RowCount > 0)
            {
                string strPRPlanCode = this.dgvPRPlanInfo["PRPlanCode", this.dgvPRPlanInfo.CurrentRow.Index].Value.ToString();
                DataGridViewRowCollection dgvrc = formPRProduce.dgvPRProduceInfo.Rows;

                //判断该笔主生产计划单是否已经制定了生产单
                foreach (DataGridViewRow dgvr in dgvrc)
                {
                    if (strPRPlanCode == dgvr.Cells["PRPlanCode"].Value.ToString())
                    {
                        MessageBox.Show("该主生产计划已制定相应的生产单！", "软件提示");
                        return;
                    }
                }

                formPRProduce.txtPRPlanCode.Text = this.dgvPRPlanInfo["PRPlanCode", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value.ToString();
                formPRProduce.cbxInvenCode.SelectedValue = this.dgvPRPlanInfo["InvenCode", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value;
                formPRProduce.txtQuantity.Text = this.dgvPRPlanInfo["Quantity", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value.ToString();
                formPRProduce.dtpEndDate.Value = Convert.ToDateTime(this.dgvPRPlanInfo["FinishDate", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value);
                this.Close();
            }
        }

        private void dgvPRPlanInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
