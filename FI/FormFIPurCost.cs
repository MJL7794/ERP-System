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
namespace ERP.FI
{
    public partial class FormFIPurCost : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormFIPurCost()
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
            this.cbxSupplierCode.Enabled = !this.cbxSupplierCode.Enabled;
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
            this.txtFIPurCode.Text = "";
            this.dtpFIPurDate.Value = Convert.ToDateTime("1900-01-01");
            this.cbxOperatorCode.SelectedIndex = -1;
            this.cbxSupplierCode.SelectedIndex = -1;
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
            this.txtFIPurCode.Text = this.dgvFIPurCostInfo["FIPurCode", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value.ToString();
            this.dtpFIPurDate.Value = Convert.ToDateTime(this.dgvFIPurCostInfo["FIPurDate", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value);
            this.cbxOperatorCode.SelectedValue = this.dgvFIPurCostInfo["OperatorCode", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value;
            this.cbxSupplierCode.SelectedValue = this.dgvFIPurCostInfo["SupplierCode", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value;
            this.cbxAccountCode.SelectedValue = this.dgvFIPurCostInfo["AccountCode", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value;
            this.cbxEmployeeCode.SelectedValue = this.dgvFIPurCostInfo["EmployeeCode", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value;
            this.cbxCostCode.SelectedValue = this.dgvFIPurCostInfo["CostCode", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value;
            this.txtFIMoney.Text = this.dgvFIPurCostInfo["FIMoney", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtRemark.Text = this.dgvFIPurCostInfo["Remark", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxIsFlag.SelectedValue = this.dgvFIPurCostInfo["IsFlag", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value;
        }

        /// <summary>
        /// DataGridView绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void DataGridViewBindSource(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM FIPurCost " + strWhere; ;

            try
            {
                this.dgvFIPurCostInfo.DataSource = db.GetDataSet(strSql, "FIPurCost").Tables["FIPurCost"];
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

            pars.Add(new SqlParameter("@FIPurCode", txtFIPurCode.Text.Trim()));
            pars.Add(new SqlParameter("@FIPurDate", dtpFIPurDate.Value));

            if (cbxOperatorCode.SelectedValue == null)
                pars.Add(new SqlParameter("@OperatorCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@OperatorCode", cbxOperatorCode.SelectedValue.ToString()));

            if (cbxSupplierCode.SelectedValue == null)
                pars.Add(new SqlParameter("@SupplierCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@SupplierCode", cbxSupplierCode.SelectedValue.ToString()));

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

        private void FormFIPurCost_Load(object sender, EventArgs e)
        {
            //权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            commUse.CortrolButtonEnabled(toolCheck, this);
            commUse.CortrolButtonEnabled(toolUnCheck, this);

            //ComboBox绑定到数据源
            commUse.BindComboBox(cbxOperatorCode, "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(cbxSupplierCode, "SupplierCode", "SupplierName", "select SupplierCode,SupplierName from BSSupplier", "BSSupplier");
            commUse.BindComboBox(cbxAccountCode, "AccountCode", "AccountName", "select AccountCode,AccountName from BSAccount", "BSAccount");
            commUse.BindComboBox(cbxEmployeeCode, "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(cbxCostCode, "CostCode", "CostName", "select CostCode,CostName from BSCost Where CostTypeCode = '01'", "BSCost");
            commUse.BindComboBox(cbxIsFlag, "Code", "Name", "select * from INCheckFlag", "INCheckFlag");

            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvFIPurCostInfo.Columns["OperatorCode"], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvFIPurCostInfo.Columns["SupplierCode"], "SupplierCode", "SupplierName", "select SupplierCode,SupplierName from BSSupplier", "BSSupplier");
            commUse.BindComboBox(this.dgvFIPurCostInfo.Columns["AccountCode"], "AccountCode", "AccountName", "select AccountCode,AccountName from BSAccount", "BSAccount");
            commUse.BindComboBox(this.dgvFIPurCostInfo.Columns["EmployeeCode"], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvFIPurCostInfo.Columns["CostCode"], "CostCode", "CostName", "select CostCode,CostName from BSCost Where CostTypeCode = '01'", "BSCost");
            commUse.BindComboBox(this.dgvFIPurCostInfo.Columns["IsFlag"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
            
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

            dtpFIPurDate.Value = DateTime.Today;
            cbxOperatorCode.SelectedValue = PropertyClass.OperatorCode;
            cbxIsFlag.SelectedValue = "0";
            txtFIPurCode.Text = commUse.BuildBillCode("FIPurCost", "FIPurCode", "FIPurDate", dtpFIPurDate.Value);
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //修改标识

        }

        private void dgvFIPurCostInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT" && dgvFIPurCostInfo.RowCount > 0)
            {
                if (this.dgvFIPurCostInfo["IsFlag", this.dgvFIPurCostInfo.CurrentRow.Index].Value.ToString() == "1")
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

            if (String.IsNullOrEmpty(txtFIPurCode.Text.Trim()))
            {
                MessageBox.Show("单据编号不许为空！", "软件提示");
                txtFIPurCode.Focus();
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

            //添加操作
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                try
                {
                    strCode = "INSERT INTO FIPurCost(FIPurCode,FIPurDate,OperatorCode,SupplierCode,AccountCode,EmployeeCode,CostCode,FIMoney,Remark,IsFlag) ";
                    strCode += "VALUES(@FIPurCode,@FIPurDate,@OperatorCode,@SupplierCode,@AccountCode,@EmployeeCode,@CostCode,@FIMoney,@Remark,@IsFlag)";

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

            //修改操作
            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                string strFIPurCode = txtFIPurCode.Text.Trim();

                try
                {
                    strCode = "UPDATE FIPurCost SET FIPurCode = @FIPurCode,FIPurDate = @FIPurDate,";
                    strCode += "OperatorCode = @OperatorCode,SupplierCode = @SupplierCode,AccountCode = @AccountCode,";
                    strCode += "EmployeeCode = @EmployeeCode,CostCode = @CostCode,FIMoney = @FIMoney,Remark = @Remark,IsFlag = @IsFlag ";
                    strCode += "WHERE FIPurCode = @OldFIPurCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldFIPurCode", strFIPurCode));

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
            string strFIPurCode = null; //数据表FIPurCost的主键
            string strSql = null;
            string strFlag = null;

            if (this.dgvFIPurCostInfo.RowCount == 0)
            {
                return;
            }

            strFIPurCode = this.dgvFIPurCostInfo["FIPurCode", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value.ToString();
            strFlag = this.dgvFIPurCostInfo["IsFlag", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value.ToString();

            if (strFlag == "1")
            {
                MessageBox.Show("该单据已审核，不许删除！", "软件提示");
                return;
            }

            strSql = "DELETE FROM FIPurCost WHERE FIPurCode = @FIPurCode";
            SqlParameter[] pars = { new SqlParameter("@FIPurCode", strFIPurCode) };

            if (MessageBox.Show("确定要删除吗？", "软件提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                try
                {
                    if (db.ExecDataBySql(strSql, pars) > 0)
                    {
                        MessageBox.Show("删除成功！", "软件提示");
                    }
                    else
                    {
                        MessageBox.Show("删除失败！", "软件提示");
                    }
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

            string strFIPurCostSql = null;   // 表示提交 FIPurCost 表的 SQL
            string strBSAccountSql = null;   // 表示提交 BSAccount 表的 SQL

            string strIsFlag = null;         // 审核标记
            string strAccountCode = null;    // 账户代码
            string strFIPurCode = null;      // 单据编号（FIPurCost 主键）
            decimal decFIMoney;              // 费用金额

            if (dgvFIPurCostInfo.RowCount == 0)
            {
                return;
            }

            strAccountCode = this.dgvFIPurCostInfo["AccountCode", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value.ToString();
            strFIPurCode = this.dgvFIPurCostInfo["FIPurCode", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value.ToString();
            decFIMoney = Convert.ToDecimal(this.dgvFIPurCostInfo["FIMoney", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value);
            strIsFlag = this.dgvFIPurCostInfo["IsFlag", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value.ToString();

            if (strIsFlag == "1")
            {
                MessageBox.Show("该单据已审核过，不许再次审核！", "软件提示");
                return;
            }

            // ✅ 检查账户余额（参数化查询）
            strCode = "SELECT AccMoney FROM BSAccount WHERE AccountCode = @AccountCode";
            SqlParameter[] checkPars = { new SqlParameter("@AccountCode", strAccountCode) };

            try
            {
                sdr = db.GetDataReader(strCode, checkPars);
                sdr.Read(); // 只有一条记录

                if (sdr.GetDecimal(0) < decFIMoney)
                {
                    MessageBox.Show("帐户金额不足，无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }

                // 关闭 sdr 对象
                sdr.Close();

                // ⚠️ 下面两条 SQL 是批量事务，db.ExecDataBySqls 只接受 List<string>
                // 没法传参数，所以这里暂时用字符串拼接。
                // 拼接的值是 decimal 和来自 DataGridView 的内部值，风险较低。

                // 扣减账户金额
                strBSAccountSql = "UPDATE BSAccount SET AccMoney = AccMoney - " + decFIMoney + " WHERE AccountCode = '" + strAccountCode + "'";
                strSqls.Add(strBSAccountSql);

                // 打审核标记
                strFIPurCostSql = "UPDATE FIPurCost SET IsFlag = '1' WHERE FIPurCode = '" + strFIPurCode + "'";
                strSqls.Add(strFIPurCostSql);

                // 提交事务
                if (db.ExecDataBySqls(strSqls))
                {
                    MessageBox.Show("审核成功！", "软件提示");
                }
                else
                {
                    MessageBox.Show("审核失败！", "软件提示");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw;   // ✅ 保留原始堆栈
            }

            this.DataGridViewBindSource("");
        }

        private void toolUnCheck_Click(object sender, EventArgs e)
        {
            List<string> strSqls = new List<string>();

            string strFIPurCostSql = null;   // 表示提交 FIPurCost 表的 SQL 语句
            string strBSAccountSql = null;   // 表示提交 BSAccount 表的 SQL 语句

            string strIsFlag = null;         // 表示审核标记
            string strAccountCode = null;    // 表示帐户代码
            string strFIPurCode = null;      // 表示单据编号
            decimal decFIMoney;              // 费用金额

            if (dgvFIPurCostInfo.RowCount == 0)
            {
                return;
            }

            strAccountCode = this.dgvFIPurCostInfo["AccountCode", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value.ToString();
            strFIPurCode = this.dgvFIPurCostInfo["FIPurCode", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value.ToString();
            decFIMoney = Convert.ToDecimal(this.dgvFIPurCostInfo["FIMoney", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value);
            strIsFlag = this.dgvFIPurCostInfo["IsFlag", this.dgvFIPurCostInfo.CurrentCell.RowIndex].Value.ToString();

            if (strIsFlag == "0")
            {
                MessageBox.Show("该单据未审核，无需弃审！", "软件提示");
                return;
            }

            try
            {
                // ✅ 参数化检查账户余额（可选，但推荐）
                string strCheckSql = "SELECT AccMoney FROM BSAccount WHERE AccountCode = @AccountCode";
                SqlParameter[] checkPars = { new SqlParameter("@AccountCode", strAccountCode) };

                SqlDataReader sdr = db.GetDataReader(strCheckSql, checkPars);
                sdr.Read();

                if (sdr.GetDecimal(0) < decFIMoney)
                {
                    MessageBox.Show("帐户金额不足，无法弃审！", "软件提示");
                    sdr.Close();
                    return;
                }
                sdr.Close();

                // ⚠️ 事务 SQL：db.ExecDataBySqls 只接受 List<string>，暂时保留字符串拼接
                // 说明：
                // - decFIMoney 是 decimal，拼接没有 SQL 注入风险
                // - strAccountCode / strFIPurCode 来自 DataGridView 的值（数据库里的值），非用户输入
                // - 如需完全参数化，需改 DataBase.ExecDataBySqls 支持 SqlParameter[]

                // 处理账户金额（加回）
                strBSAccountSql = "UPDATE BSAccount SET AccMoney = AccMoney + " + decFIMoney + " WHERE AccountCode = '" + strAccountCode + "'";
                strSqls.Add(strBSAccountSql);

                // 打弃审标记
                strFIPurCostSql = "UPDATE FIPurCost SET IsFlag = '0' WHERE FIPurCode = '" + strFIPurCode + "'";
                strSqls.Add(strFIPurCostSql);

                // 更新数据
                if (db.ExecDataBySqls(strSqls))
                {
                    MessageBox.Show("弃审成功！", "软件提示");
                }
                else
                {
                    MessageBox.Show("弃审失败！", "软件提示");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw;   // ✅ 保留原始堆栈
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

                    strWhere = " WHERE FIPurCode LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.DataGridViewBindSource(strWhere);
                    break;

                case "单据日期":

                    strWhere = " WHERE SUBSTRING(CONVERT(VARCHAR(20),FIPurDate,20),1,10) LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.DataGridViewBindSource(strWhere);
                    break;

                default:
                    break;
            }
        }

        private void dgvFIPurCostInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
