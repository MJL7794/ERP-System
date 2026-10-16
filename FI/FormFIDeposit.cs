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
    public partial class FormFIDeposit : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormFIDeposit()
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
            this.cbxOutAccCode.Enabled = !this.cbxOutAccCode.Enabled;
            this.cbxInAccCode.Enabled = !this.cbxInAccCode.Enabled;
            this.txtFIMoney.ReadOnly = !this.txtFIMoney.ReadOnly;
            this.cbxEmployeeCode.Enabled = !this.cbxEmployeeCode.Enabled;
            this.txtRemark.ReadOnly = !this.txtRemark.ReadOnly;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            this.txtFIDepositCode.Text = "";
            this.dtpFIDepositDate.Value = Convert.ToDateTime("1900-01-01");
            this.cbxOperatorCode.SelectedIndex = -1;
            this.cbxOutAccCode.SelectedIndex = -1;
            this.cbxInAccCode.SelectedIndex = -1;
            this.txtFIMoney.Text = "";
            this.cbxEmployeeCode.SelectedIndex = -1;
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
            this.txtFIDepositCode.Text = this.dgvFIDepositInfo["FIDepositCode", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value.ToString();
            this.dtpFIDepositDate.Value = Convert.ToDateTime(this.dgvFIDepositInfo["FIDepositDate", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value);
            this.cbxOperatorCode.SelectedValue = this.dgvFIDepositInfo["OperatorCode", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value;
            this.cbxOutAccCode.SelectedValue = this.dgvFIDepositInfo["OutAccCode", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value;
            this.cbxInAccCode.SelectedValue = this.dgvFIDepositInfo["InAccCode", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value;
            this.txtFIMoney.Text = this.dgvFIDepositInfo["FIMoney", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxEmployeeCode.SelectedValue = this.dgvFIDepositInfo["EmployeeCode", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value;
            this.txtRemark.Text = this.dgvFIDepositInfo["Remark", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxIsFlag.SelectedValue = this.dgvFIDepositInfo["IsFlag", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value;
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM FIDeposit " + strWhere; ;

            try
            {
                this.dgvFIDepositInfo.DataSource = db.GetDataSet(strSql, "FIDeposit").Tables["FIDeposit"];
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

            pars.Add(new SqlParameter("@FIDepositCode", txtFIDepositCode.Text.Trim()));
            pars.Add(new SqlParameter("@FIDepositDate", dtpFIDepositDate.Value));

            if (cbxOperatorCode.SelectedValue == null)
                pars.Add(new SqlParameter("@OperatorCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@OperatorCode", cbxOperatorCode.SelectedValue.ToString()));

            if (cbxOutAccCode.SelectedValue == null)
                pars.Add(new SqlParameter("@OutAccCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@OutAccCode", cbxOutAccCode.SelectedValue.ToString()));

            if (cbxInAccCode.SelectedValue == null)
                pars.Add(new SqlParameter("@InAccCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@InAccCode", cbxInAccCode.SelectedValue.ToString()));

            if (String.IsNullOrEmpty(txtFIMoney.Text.Trim()))
                pars.Add(new SqlParameter("@FIMoney", DBNull.Value));
            else
                pars.Add(new SqlParameter("@FIMoney", Convert.ToDecimal(txtFIMoney.Text.Trim())));

            if (cbxEmployeeCode.SelectedValue == null)
                pars.Add(new SqlParameter("@EmployeeCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@EmployeeCode", cbxEmployeeCode.SelectedValue.ToString()));

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

        private void FormFIDeposit_Load(object sender, EventArgs e)
        {
            //权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            commUse.CortrolButtonEnabled(toolCheck, this);
            commUse.CortrolButtonEnabled(toolUnCheck, this);

            //ComboBox绑定到数据源
            commUse.BindComboBox(cbxOperatorCode, "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(cbxOutAccCode, "AccountCode", "AccountName", "select AccountCode,AccountName from BSAccount", "BSAccount");
            commUse.BindComboBox(cbxInAccCode, "AccountCode", "AccountName", "select AccountCode,AccountName from BSAccount", "BSAccount");
            commUse.BindComboBox(cbxEmployeeCode, "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(cbxIsFlag, "Code", "Name", "select * from INCheckFlag", "INCheckFlag");

            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvFIDepositInfo.Columns["OperatorCode"], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvFIDepositInfo.Columns["OutAccCode"],"AccountCode", "AccountName", "select AccountCode,AccountName from BSAccount", "BSAccount");
            commUse.BindComboBox(this.dgvFIDepositInfo.Columns["InAccCode"],"AccountCode","AccountName", "select AccountCode,AccountName from BSAccount", "BSAccount");
            commUse.BindComboBox(this.dgvFIDepositInfo.Columns["EmployeeCode"],"EmployeeCode","EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvFIDepositInfo.Columns["IsFlag"],"Code", "Name", "select * from INCheckFlag", "INCheckFlag");
            //
            BindDataGridView("");
            this.BindToolStripComboBox();
            this.cbxCondition.SelectedIndex = 0;
            toolStrip1.Tag = "";
        }

        private void toolAdd_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "ADD"; //添加操作标识

            dtpFIDepositDate.Value = DateTime.Today;
            cbxOperatorCode.SelectedValue = PropertyClass.OperatorCode;
            cbxIsFlag.SelectedValue = "0";
            txtFIDepositCode.Text = commUse.BuildBillCode("FIDeposit", "FIDepositCode", "FIDepositDate", dtpFIDepositDate.Value);
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //修改操作标识
        }

        private void dgvFIDepositInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT" && dgvFIDepositInfo.RowCount > 0)
            {
                if (this.dgvFIDepositInfo["IsFlag", this.dgvFIDepositInfo.CurrentRow.Index].Value.ToString() == "1")
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
            commUse.InputNumeric(e,sender as Control);
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            string strCode = null;

            if (String.IsNullOrEmpty(txtFIDepositCode.Text.Trim()))
            {
                MessageBox.Show("单据编号不许为空！", "软件提示");
                txtFIDepositCode.Focus();
                return;
            }

            if (cbxOutAccCode.SelectedValue == null)
            {
                MessageBox.Show("转出帐户不许为空！", "软件提示");
                cbxOutAccCode.Focus();
                return;
            }

            if (cbxInAccCode.SelectedValue == null)
            {
                MessageBox.Show("转入帐户不许为空！", "软件提示");
                cbxInAccCode.Focus();
                return;
            }

            if (cbxOutAccCode.SelectedValue.ToString() == cbxInAccCode.SelectedValue.ToString())
            {
                MessageBox.Show("转出帐户不许等于转入帐户！", "软件提示");
                cbxOutAccCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtFIMoney.Text.Trim()))
            {
                MessageBox.Show("存取金额不许为空！", "软件提示");
                txtFIMoney.Focus();
                return;
            }
            else
            {
                if (Convert.ToDecimal(txtFIMoney.Text.Trim()) == 0)
                {
                    MessageBox.Show("存取金额不能等于零！", "软件提示");
                    txtFIMoney.Focus();
                    return;
                }
            }

            if (cbxEmployeeCode.SelectedValue == null)
            {
                MessageBox.Show("出纳员不许为空！", "软件提示");
                cbxEmployeeCode.Focus();
                return;
            }

            if (toolStrip1.Tag.ToString() == "ADD")
            {
                try
                {
                    strCode = "INSERT INTO FIDeposit(FIDepositCode,FIDepositDate,OperatorCode,OutAccCode,InAccCode,FIMoney,EmployeeCode,Remark,IsFlag) ";
                    strCode += "VALUES(@FIDepositCode,@FIDepositDate,@OperatorCode,@OutAccCode,@InAccCode,@FIMoney,@EmployeeCode,@Remark,@IsFlag)";

                    SqlParameter[] pars = ParametersAddValue();

                    if (db.ExecDataBySql(strCode, pars) > 0)
                    {
                        MessageBox.Show("保存成功！", "软件提示");
                        this.BindDataGridView("");
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
                string strFIDepositCode = txtFIDepositCode.Text.Trim();

                try
                {
                    strCode = "UPDATE FIDeposit SET FIDepositCode = @FIDepositCode,FIDepositDate = @FIDepositDate,";
                    strCode += "OperatorCode = @OperatorCode,OutAccCode = @OutAccCode,InAccCode = @InAccCode,";
                    strCode += "FIMoney = @FIMoney,EmployeeCode = @EmployeeCode,Remark = @Remark,IsFlag = @IsFlag ";
                    strCode += "WHERE FIDepositCode = @OldFIDepositCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldFIDepositCode", strFIDepositCode));

                    if (db.ExecDataBySql(strCode, parsList.ToArray()) > 0)
                    {
                        MessageBox.Show("保存成功！", "软件提示");
                        this.BindDataGridView("");
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
            string strFIDepositCode = null;
            string strSql = null;
            string strFlag = null;

            if (this.dgvFIDepositInfo.RowCount == 0)
            {
                return;
            }

            strFIDepositCode = this.dgvFIDepositInfo["FIDepositCode", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value.ToString();
            strFlag = this.dgvFIDepositInfo["IsFlag", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value.ToString();

            if (strFlag == "1")
            {
                MessageBox.Show("该单据已审核，不许删除！", "软件提示");
                return;
            }

            strSql = "DELETE FROM FIDeposit WHERE FIDepositCode = @FIDepositCode";
            SqlParameter[] pars = { new SqlParameter("@FIDepositCode", strFIDepositCode) };

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

                this.BindDataGridView("");
            }
        }

        private void toolCheck_Click(object sender, EventArgs e)
        {
            SqlDataReader sdr = null;
            string strCode = null;
            List<string> strSqls = new List<string>();

            string strFIDepositSql = null;
            string strOutAccSql = null;
            string strInAccSql = null;

            string strIsFlag = null;
            string strOutAccCode = null;
            string strInAccCode = null;
            string strFIDepositCode = null;
            decimal decFIMoney;

            if (dgvFIDepositInfo.RowCount == 0) return;

            strOutAccCode = this.dgvFIDepositInfo["OutAccCode", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value.ToString();
            strInAccCode = this.dgvFIDepositInfo["InAccCode", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value.ToString();
            strFIDepositCode = this.dgvFIDepositInfo["FIDepositCode", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value.ToString();
            decFIMoney = Convert.ToDecimal(this.dgvFIDepositInfo["FIMoney", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value);
            strIsFlag = this.dgvFIDepositInfo["IsFlag", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value.ToString();

            if (strIsFlag == "1")
            {
                MessageBox.Show("该单据已审核过，不许再次审核！", "软件提示");
                return;
            }

            // ✅ 检查转出账户余额（参数化）
            strCode = "SELECT AccMoney FROM BSAccount WHERE AccountCode = @OutAccCode";
            SqlParameter[] checkPars = { new SqlParameter("@OutAccCode", strOutAccCode) };

            try
            {
                sdr = db.GetDataReader(strCode, checkPars);
                sdr.Read();

                if (sdr.GetDecimal(0) < decFIMoney)
                {
                    MessageBox.Show("转出帐户金额不足，无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }
                sdr.Close();

                // ⚠️ 下面三条 SQL 是批量事务，但 db.ExecDataBySqls 只接受 List<string>
                // 没法传参数，所以这里暂时用字符串拼接。
                // 更优方案：把 ExecDataBySqls 改成接受 List<SqlCommand> 或 List<(string, SqlParameter[])>

                // 转出账户
                strOutAccSql = "UPDATE BSAccount SET AccMoney = AccMoney - " + decFIMoney + " WHERE AccountCode = '" + strOutAccCode + "'";
                strSqls.Add(strOutAccSql);

                // 转入账户
                strInAccSql = "UPDATE BSAccount SET AccMoney = AccMoney + " + decFIMoney + " WHERE AccountCode = '" + strInAccCode + "'";
                strSqls.Add(strInAccSql);

                // 打审核标记
                strFIDepositSql = "UPDATE FIDeposit SET IsFlag = '1' WHERE FIDepositCode = '" + strFIDepositCode + "'";
                strSqls.Add(strFIDepositSql);

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

            this.BindDataGridView("");
        }

        private void toolUnCheck_Click(object sender, EventArgs e)
        {
            SqlDataReader sdr = null;
            string strCode = null;
            List<string> strSqls = new List<string>();

            string strFIDepositSql = null;   //表示提交FIDeposit表的SQL语句
            string strOutAccSql = null;      //表示提交BSAccount表的SQL语句(转出帐户)
            string strInAccSql = null;       //表示提交BSAccount表的SQL语句(转入帐户)

            string strIsFlag = null;         //表示审核标记
            string strOutAccCode = null;     //表示转出帐户的代码
            string strInAccCode = null;      //表示转入帐户的代码
            string strFIDepositCode = null;  //表示FIDeposit数据表的主键
            decimal decFIMoney;              //存取金额

            if (dgvFIDepositInfo.RowCount == 0)
            {
                return;
            }

            strOutAccCode = this.dgvFIDepositInfo["OutAccCode", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value.ToString();
            strInAccCode = this.dgvFIDepositInfo["InAccCode", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value.ToString();
            strFIDepositCode = this.dgvFIDepositInfo["FIDepositCode", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value.ToString();
            decFIMoney = Convert.ToDecimal(this.dgvFIDepositInfo["FIMoney", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value);
            strIsFlag = this.dgvFIDepositInfo["IsFlag", this.dgvFIDepositInfo.CurrentCell.RowIndex].Value.ToString();

            if (strIsFlag == "0")
            {
                MessageBox.Show("该单据未审核，无需弃审！", "软件提示");
                return;
            }

            // ✅ 参数化检查转入账户余额
            strCode = "SELECT AccMoney FROM BSAccount WHERE AccountCode = @InAccCode";
            SqlParameter[] checkPars = { new SqlParameter("@InAccCode", strInAccCode) };

            try
            {
                sdr = db.GetDataReader(strCode, checkPars);
                sdr.Read(); //只有一条记录

                if (sdr.GetDecimal(0) < decFIMoney)
                {
                    MessageBox.Show("转入帐户已发生相关业务，无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }

                //关闭sdr对象
                sdr.Close();

                // ⚠️ 下面三条是事务 SQL，db.ExecDataBySqls 只接受 List<string>，暂时保留拼接
                // 弃审转入帐户
                strInAccSql = "UPDATE BSAccount SET AccMoney = AccMoney - " + decFIMoney + " WHERE AccountCode = '" + strInAccCode + "'";
                strSqls.Add(strInAccSql);

                // 弃审转出帐户
                strOutAccSql = "UPDATE BSAccount SET AccMoney = AccMoney + " + decFIMoney + " WHERE AccountCode = '" + strOutAccCode + "'";
                strSqls.Add(strOutAccSql);

                // 打弃审标记
                strFIDepositSql = "UPDATE FIDeposit SET IsFlag = '0' WHERE FIDepositCode = '" + strFIDepositCode + "'";
                strSqls.Add(strFIDepositSql);

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
                throw;
            }

            this.BindDataGridView("");
        }

        private void txtOK_Click(object sender, EventArgs e)
        {
            string strWhere = String.Empty;
            string strConditonName = String.Empty;

            strConditonName = this.cbxCondition.Items[this.cbxCondition.SelectedIndex].ToString();
            switch (strConditonName)
            {
                case "单据编号":

                    strWhere = " WHERE FIDepositCode LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "单据日期":

                    strWhere = " WHERE SUBSTRING(CONVERT(VARCHAR(20),FIDepositDate,20),1,10) LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                default:
                    break;
            }
        }
    }
}
