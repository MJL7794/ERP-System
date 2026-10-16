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

namespace ERP.FI
{
    public partial class FormFISelCost : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormFISelCost()
        {
            InitializeComponent();
        }

        private void ControlStatus()
        {
            //工具栏按钮状态切换
            this.toolSave.Enabled = !this.toolSave.Enabled;
            this.toolCancel.Enabled = !this.toolCancel.Enabled;
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            commUse.CortrolButtonEnabled(toolCheck, this);
            commUse.CortrolButtonEnabled(toolUnCheck, this);

            //窗体控件状态切换
            this.cbxCustomerCode.Enabled = !this.cbxCustomerCode.Enabled;
            this.cbxAccountCode.Enabled = !this.cbxAccountCode.Enabled;
            this.cbxEmployeeCode.Enabled = !this.cbxEmployeeCode.Enabled;
            this.cbxCostCode.Enabled = !this.cbxCostCode.Enabled;
            this.txtFIMoney.ReadOnly = !this.txtFIMoney.ReadOnly;
            this.txtRemark.ReadOnly = !this.txtRemark.ReadOnly;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            this.txtFISelCode.Text = "";
            this.dtpFISelDate.Value = Convert.ToDateTime("1900-01-01");
            this.cbxOperatorCode.SelectedIndex = -1;
            this.cbxCustomerCode.SelectedIndex = -1;
            this.cbxAccountCode.SelectedIndex = -1;
            this.cbxEmployeeCode.SelectedIndex = -1;
            this.cbxCostCode.SelectedIndex = -1;
            this.txtFIMoney.Text = "";
            this.txtRemark.Text = "";
            this.cbxIsFlag.SelectedIndex = -1;
        }

        private void BindToolStripComboBox()
        {
            this.cbxCondition.Items.Add("单据编号");
            this.cbxCondition.Items.Add("单据日期");
        }

        /// <summary>
        /// 设置控件的显示值
        /// </summary>
        private void FillControls()
        {
            this.txtFISelCode.Text = this.dgvFISelCostInfo["FISelCode", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value.ToString();
            this.dtpFISelDate.Value = Convert.ToDateTime(this.dgvFISelCostInfo["FISelDate", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value);
            this.cbxOperatorCode.SelectedValue = this.dgvFISelCostInfo["OperatorCode", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value;
            this.cbxCustomerCode.SelectedValue = this.dgvFISelCostInfo["CustomerCode", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value;
            this.cbxAccountCode.SelectedValue = this.dgvFISelCostInfo["AccountCode", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value;
            this.cbxEmployeeCode.SelectedValue = this.dgvFISelCostInfo["EmployeeCode", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value;
            this.cbxCostCode.SelectedValue = this.dgvFISelCostInfo["CostCode", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value;
            this.txtFIMoney.Text = this.dgvFISelCostInfo["FIMoney", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtRemark.Text = this.dgvFISelCostInfo["Remark", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxIsFlag.SelectedValue = this.dgvFISelCostInfo["IsFlag", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value;
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void DataGridViewBindSource(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM FISelCost " + strWhere; ;

            try
            {
                this.dgvFISelCostInfo.DataSource = db.GetDataSet(strSql, "FISelCost").Tables["FISelCost"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw ex;
            }
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        private SqlParameter[] ParametersAddValue()
        {
            List<SqlParameter> pars = new List<SqlParameter>();

            pars.Add(new SqlParameter("@FISelCode", txtFISelCode.Text.Trim()));
            pars.Add(new SqlParameter("@FISelDate", dtpFISelDate.Value));

            if (cbxOperatorCode.SelectedValue == null)
                pars.Add(new SqlParameter("@OperatorCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@OperatorCode", cbxOperatorCode.SelectedValue.ToString()));

            if (cbxCustomerCode.SelectedValue == null)
                pars.Add(new SqlParameter("@CustomerCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@CustomerCode", cbxCustomerCode.SelectedValue.ToString()));

            if (cbxAccountCode.SelectedValue == null)
                pars.Add(new SqlParameter("@AccountCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@AccountCode", cbxAccountCode.SelectedValue.ToString()));

            if (cbxEmployeeCode.SelectedValue == null)
                pars.Add(new SqlParameter("@EmployeeCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@EmployeeCode", cbxEmployeeCode.SelectedValue.ToString()));

            if (cbxCostCode.SelectedValue == null)
                pars.Add(new SqlParameter("@CostCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@CostCode", cbxCostCode.SelectedValue.ToString()));

            if (String.IsNullOrEmpty(txtFIMoney.Text.Trim()))
                pars.Add(new SqlParameter("@FIMoney", DBNull.Value));
            else
                pars.Add(new SqlParameter("@FIMoney", Convert.ToDecimal(txtFIMoney.Text.Trim())));

            if (String.IsNullOrEmpty(txtRemark.Text.Trim()))
                pars.Add(new SqlParameter("@Remark", DBNull.Value));
            else
                pars.Add(new SqlParameter("@Remark", txtRemark.Text.Trim()));

            if (cbxIsFlag.SelectedValue == null)
                pars.Add(new SqlParameter("@IsFlag", DBNull.Value));
            else
                pars.Add(new SqlParameter("@IsFlag", cbxIsFlag.SelectedValue.ToString()));

            return pars.ToArray();
        }


        private void FormFISelCost_Load(object sender, EventArgs e)
        {
            //权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            commUse.CortrolButtonEnabled(toolCheck, this);
            commUse.CortrolButtonEnabled(toolUnCheck, this);

            //ComboBox绑定到数据源
            commUse.BindComboBox(cbxOperatorCode, "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(cbxCustomerCode, "CustomerCode", "CustomerName", "select CustomerCode,CustomerName from BSCustomer", "BSCustomer");
            commUse.BindComboBox(cbxAccountCode, "AccountCode", "AccountName", "select AccountCode,AccountName from BSAccount", "BSAccount");
            commUse.BindComboBox(cbxEmployeeCode, "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(cbxCostCode, "CostCode", "CostName", "select CostCode,CostName from BSCost Where CostTypeCode = '02'", "BSCost");
            commUse.BindComboBox(cbxIsFlag, "Code", "Name", "select * from INCheckFlag", "INCheckFlag");

            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvFISelCostInfo.Columns["OperatorCode"], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvFISelCostInfo.Columns["CustomerCode"], "CustomerCode", "CustomerName", "select CustomerCode,CustomerName from BSCustomer", "BSCustomer");
            commUse.BindComboBox(this.dgvFISelCostInfo.Columns["AccountCode"], "AccountCode", "AccountName", "select AccountCode,AccountName from BSAccount", "BSAccount");
            commUse.BindComboBox(this.dgvFISelCostInfo.Columns["EmployeeCode"], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvFISelCostInfo.Columns["CostCode"], "CostCode", "CostName", "select CostCode,CostName from BSCost Where CostTypeCode = '02'", "BSCost");
            commUse.BindComboBox(this.dgvFISelCostInfo.Columns["IsFlag"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
            
            //DataGridView绑定到数据源
            DataGridViewBindSource("");
            this.BindToolStripComboBox();
            this.cbxCondition.SelectedIndex = 0;
            toolStrip1.Tag = "";
        }

        private void toolAdd_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "ADD"; //添加标识

            dtpFISelDate.Value = DateTime.Today;
            cbxOperatorCode.SelectedValue = PropertyClass.OperatorCode;
            cbxIsFlag.SelectedValue = "0";
            txtFISelCode.Text = commUse.BuildBillCode("FISelCost", "FISelCode", "FISelDate", dtpFISelDate.Value);
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //修改标识

        }

        private void dgvFISelCostInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT" && dgvFISelCostInfo.RowCount > 0)
            {
                if (this.dgvFISelCostInfo["IsFlag", this.dgvFISelCostInfo.CurrentRow.Index].Value.ToString() == "1")
                {
                    MessageBox.Show("该记录已审核，不允许编辑！", "软件提示");
                    return;
                }

                FillControls();
            }
        }

        private void toolCancel_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "";
        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtFIMoney_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputNumeric(e, sender as Control);
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            string strCode = null;

            if (String.IsNullOrEmpty(txtFISelCode.Text.Trim()))
            {
                MessageBox.Show("单据编号不许为空！", "软件提示");
                txtFISelCode.Focus();
                return;
            }

            if (cbxAccountCode.SelectedValue == null)
            {
                MessageBox.Show("结算帐户不许为空！", "软件提示");
                cbxAccountCode.Focus();
                return;
            }

            if (cbxEmployeeCode.SelectedValue == null)
            {
                MessageBox.Show("报销人不许为空！", "软件提示");
                cbxEmployeeCode.Focus();
                return;
            }

            if (cbxCostCode.SelectedValue == null)
            {
                MessageBox.Show("费用名称不许为空！", "软件提示");
                cbxCostCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtFIMoney.Text.Trim()))
            {
                MessageBox.Show("费用金额不许为空！", "软件提示");
                txtFIMoney.Focus();
                return;
            }
            else
            {
                if (Convert.ToDecimal(txtFIMoney.Text.Trim()) == 0)
                {
                    MessageBox.Show("费用金额不能等于零！", "软件提示");
                    txtFIMoney.Focus();
                    return;
                }
            }

            if (toolStrip1.Tag.ToString() == "ADD")
            {
                try
                {
                    strCode = "INSERT INTO FISelCost(FISelCode,FISelDate,OperatorCode,CustomerCode,AccountCode,EmployeeCode,CostCode,FIMoney,Remark,IsFlag) ";
                    strCode += "VALUES(@FISelCode,@FISelDate,@OperatorCode,@CustomerCode,@AccountCode,@EmployeeCode,@CostCode,@FIMoney,@Remark,@IsFlag)";

                    SqlParameter[] pars = ParametersAddValue();

                    if (db.ExecDataBySql(strCode, pars) > 0)
                    {
                        MessageBox.Show("保存成功！", "软件提示");
                        this.DataGridViewBindSource("");
                        ControlStatus();
                    }
                    else
                    {
                        MessageBox.Show("保存失败！", "软件提示");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "软件提示");
                    throw;
                }
            }

            //修改
            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                string strFISelCode = txtFISelCode.Text.Trim();

                try
                {
                    strCode = "UPDATE FISelCost SET FISelCode = @FISelCode,FISelDate = @FISelDate,";
                    strCode += "OperatorCode = @OperatorCode,CustomerCode = @CustomerCode,AccountCode = @AccountCode,";
                    strCode += "EmployeeCode = @EmployeeCode,CostCode = @CostCode,FIMoney = @FIMoney,Remark = @Remark,IsFlag = @IsFlag ";
                    strCode += "WHERE FISelCode = @OldFISelCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldFISelCode", strFISelCode));

                    if (db.ExecDataBySql(strCode, parsList.ToArray()) > 0)
                    {
                        MessageBox.Show("保存成功！", "软件提示");
                        this.DataGridViewBindSource("");
                        ControlStatus();
                    }
                    else
                    {
                        MessageBox.Show("保存失败！", "软件提示");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "软件提示");
                    throw;
                }
            }

            toolStrip1.Tag = "";
        }

        private void toolDelete_Click(object sender, EventArgs e)
        {
            string strFISelCode = null; //单据编号
            string strSql = null;
            string strFlag = null;

            if (this.dgvFISelCostInfo.RowCount == 0)
            {
                return;
            }

            strFISelCode = this.dgvFISelCostInfo["FISelCode", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value.ToString();
            strFlag = this.dgvFISelCostInfo["IsFlag", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value.ToString();

            if (strFlag == "1")
            {
                MessageBox.Show("该单据已审核，不许删除！", "软件提示");
                return;
            }

            strSql = "DELETE FROM FISelCost WHERE FISelCode = @FISelCode";
            SqlParameter[] pars = { new SqlParameter("@FISelCode", strFISelCode) };

            if (MessageBox.Show("确定要删除吗？", "软件提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                try
                {
                    if (db.ExecDataBySql(strSql, pars) > 0)
                        MessageBox.Show("删除成功！", "软件提示");
                    else
                        MessageBox.Show("删除失败！", "软件提示");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "软件提示");
                    throw;
                }

                this.DataGridViewBindSource("");
            }
        }

        private void toolCheck_Click(object sender, EventArgs e)
        {
            SqlDataReader sdr = null;
            string strCode = null;
            List<string> strSqls = new List<string>();

            string strFISelCostSql = null; //表示提交FISelCost表的SQL语句
            string strBSAccountSql = null; //表示提交BSAccount表的SQL语句

            string strIsFlag = null; //审核标记
            string strAccountCode = null; //帐户代码
            string strFISelCode = null; //单据编码
            decimal decFIMoney; //费用金额

            if (dgvFISelCostInfo.RowCount == 0)
            {
                return;
            }

            strAccountCode = this.dgvFISelCostInfo["AccountCode", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value.ToString();
            strFISelCode = this.dgvFISelCostInfo["FISelCode", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value.ToString();
            decFIMoney = Convert.ToDecimal(this.dgvFISelCostInfo["FIMoney", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value);
            strIsFlag = this.dgvFISelCostInfo["IsFlag", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value.ToString();

            if (strIsFlag == "1")
            {
                MessageBox.Show("该单据已审核过，不许再次审核！", "软件提示");
                return;
            }

            // ✅ 检查账户余额（参数化）
            strCode = "SELECT AccMoney FROM BSAccount WHERE AccountCode = @AccountCode";
            SqlParameter[] checkPars = { new SqlParameter("@AccountCode", strAccountCode) };

            try
            {
                sdr = db.GetDataReader(strCode, checkPars);
                sdr.Read();

                if (sdr.GetDecimal(0) < decFIMoney)
                {
                    MessageBox.Show("帐户金额不足，无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }
                sdr.Close();

                // 事务 SQL（没法参数化，暂用拼接）
                strBSAccountSql = "UPDATE BSAccount SET AccMoney = AccMoney - " + decFIMoney + " WHERE AccountCode = '" + strAccountCode + "'";
                strSqls.Add(strBSAccountSql);

                strFISelCostSql = "UPDATE FISelCost SET IsFlag = '1' WHERE FISelCode = '" + strFISelCode + "'";
                strSqls.Add(strFISelCostSql);

                if (db.ExecDataBySqls(strSqls))
                    MessageBox.Show("审核成功！", "软件提示");
                else
                    MessageBox.Show("审核失败！", "软件提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw;
            }

            this.DataGridViewBindSource("");
        }

        private void toolUnCheck_Click(object sender, EventArgs e)
        {   
            List<string> strSqls = new List<string>();

            string strFISelCostSql = null; //表示提交FISelCost表的SQL语句
            string strBSAccountSql = null; //表示提交BSAccount表的SQL语句
            
            string strIsFlag = null; //表示审核标记
            string strAccountCode = null; //表示帐户代码
            string strFISelCode = null; //表示单据编码
            decimal decFIMoney; //表示费用金额

            if (dgvFISelCostInfo.RowCount == 0)
            {
                return;
            }

            strAccountCode = this.dgvFISelCostInfo["AccountCode", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value.ToString();
            strFISelCode = this.dgvFISelCostInfo["FISelCode", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value.ToString();
            decFIMoney = Convert.ToDecimal(this.dgvFISelCostInfo["FIMoney", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value);
            strIsFlag = this.dgvFISelCostInfo["IsFlag", this.dgvFISelCostInfo.CurrentCell.RowIndex].Value.ToString();

            if (strIsFlag == "0")
            {
                MessageBox.Show("该单据未审核，无需弃审！", "软件提示");
                return;
            }

            try
            {
                strBSAccountSql = "UPDATE BSAccount SET AccMoney = AccMoney + " + decFIMoney + " WHERE AccountCode = '" + strAccountCode + "'";
                strSqls.Add(strBSAccountSql);

                strFISelCostSql = "UPDATE FISelCost SET IsFlag = '0' WHERE FISelCode = '" + strFISelCode + "'";
                strSqls.Add(strFISelCostSql);

                if (db.ExecDataBySqls(strSqls))
                    MessageBox.Show("弃审成功！", "软件提示");
                else
                    MessageBox.Show("弃审失败！", "软件提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw;
            }

            this.DataGridViewBindSource("");
        }

        private void txtOK_Click(object sender, EventArgs e)
        {
            string strWhere = String.Empty;
            string strConditonName = String.Empty;

            strConditonName = this.cbxCondition.Items[this.cbxCondition.SelectedIndex].ToString();
            switch (strConditonName)
            {
                case "单据编号":

                    strWhere = " WHERE FISelCode LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.DataGridViewBindSource(strWhere);
                    break;

                case "单据日期":

                    strWhere = " WHERE SUBSTRING(CONVERT(VARCHAR(20),FISelDate,20),1,10) LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.DataGridViewBindSource(strWhere);
                    break;

                default:
                    break;
            }
        }

        private void dgvFISelCostInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
