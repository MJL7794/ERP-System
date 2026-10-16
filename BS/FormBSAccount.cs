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

namespace ERP.BS
{
    public partial class FormBSAccount : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormBSAccount()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 空间的状态切换
        /// </summary>
        private void ControlStatus()
        {
            //按钮切换状态及授权控制
            this.toolSave.Enabled = !this.toolSave.Enabled;
            this.toolCancel.Enabled = !this.toolCancel.Enabled;
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);

            //窗体控件状态切换
            this.txtAccountCode.ReadOnly = !this.txtAccountCode.ReadOnly;
            this.txtAccountName.ReadOnly = !this.txtAccountName.ReadOnly;
            this.txtBankAccount.ReadOnly = !this.txtBankAccount.ReadOnly;
            this.cbxAccSubject.Enabled = !this.cbxAccSubject.Enabled;
            this.txtAccMoney.ReadOnly = !this.txtAccMoney.ReadOnly;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            //窗体控件状态切换
            this.txtAccountCode.Text = "";
            this.txtAccountName.Text = "";
            this.txtBankAccount.Text = "";
            this.cbxAccSubject.SelectedIndex = -1;
            this.txtAccMoney.Text = "";
        }

        private void BindToolStripComboBox()
        {
            this.cbxCondition.Items.Add("帐户名称");
            this.cbxCondition.Items.Add("银行账号");
        }

        /// <summary>
        /// 设置控件的显示值
        /// </summary>
        private void FillControls()
        {
            this.txtAccountCode.Text = this.dgvAccountInfo[0, this.dgvAccountInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtAccountName.Text = this.dgvAccountInfo[1, this.dgvAccountInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtBankAccount.Text = this.dgvAccountInfo[2, this.dgvAccountInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxAccSubject.SelectedValue = this.dgvAccountInfo[3, this.dgvAccountInfo.CurrentCell.RowIndex].Value;
            this.txtAccMoney.Text = this.dgvAccountInfo[4, this.dgvAccountInfo.CurrentCell.RowIndex].Value.ToString();
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT AccountCode,AccountName,BankAccount,";
            strSql += "AccSubject,AccMoney ";
            strSql += " FROM BSAccount" + strWhere; ;

            try
            {
                this.dgvAccountInfo.DataSource = db.GetDataSet(strSql, "BSAccount").Tables["BSAccount"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw ex;
            }
        }

        private void txtAccMoney_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputNumeric(e, sender as Control);
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        private SqlParameter[] ParametersAddValue()
        {
            List<SqlParameter> pars = new List<SqlParameter>();

            pars.Add(new SqlParameter("@AccountCode", txtAccountCode.Text.Trim()));
            pars.Add(new SqlParameter("@AccountName", txtAccountName.Text.Trim()));
            pars.Add(new SqlParameter("@BankAccount", txtBankAccount.Text.Trim()));

            if (cbxAccSubject.SelectedValue == null)
                pars.Add(new SqlParameter("@AccSubject", DBNull.Value));
            else
                pars.Add(new SqlParameter("@AccSubject", cbxAccSubject.SelectedValue.ToString()));

            if (string.IsNullOrEmpty(txtAccMoney.Text.Trim()))
                pars.Add(new SqlParameter("@AccMoney", 0));
            else
                pars.Add(new SqlParameter("@AccMoney",
                    Decimal.Round(Convert.ToDecimal(txtAccMoney.Text.Trim()), 2)));

            return pars.ToArray();
        }

        private void FormAccount_Load(object sender, EventArgs e)
        {
            //权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);

            //ComboBox绑定到数据源
            commUse.BindComboBox(cbxAccSubject, "Code", "Name", "select * from INAccSubject", "INAccSubject");
            
            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvAccountInfo.Columns[3], "Code", "Name", "select * from INAccSubject", "INAccSubject");
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
            toolStrip1.Tag = "ADD"; //表示添加状态
            txtAccountCode.Enabled = true;
            txtAccountName.Enabled = true;
            cbxAccSubject.Enabled = true;
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //表示修改状态
            txtAccountCode.Enabled = false;
            txtAccountName.Enabled = true;
            cbxAccSubject.Enabled = true;
        }

        private void toolreflush_Click(object sender, EventArgs e)
        {
            this.BindDataGridView("");
        }

        private void dgvAccountInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                if (dgvAccountInfo.RowCount > 0)
                {
                    //对于基础类型帐户信息，不许修改帐户代码、帐户名称、会计科目
                    if (dgvAccountInfo[0, dgvAccountInfo.CurrentCell.RowIndex].Value.ToString() == "01"
                       || dgvAccountInfo[0, dgvAccountInfo.CurrentCell.RowIndex].Value.ToString() == "02")
                    {
                        txtAccountCode.Enabled = false;
                        txtAccountName.Enabled = false;
                        cbxAccSubject.Enabled = false;
                    }
                    else
                    {
                        txtAccountCode.Enabled = true;
                        txtAccountName.Enabled = true;
                        cbxAccSubject.Enabled = true;
                    }

                    //判断当前记录的主键值是否存在外键约束
                    if (commUse.IsExistConstraint("BSAccount", dgvAccountInfo[0,dgvAccountInfo.CurrentCell.RowIndex].Value.ToString()))
                    {
                        txtAccountCode.Enabled = false;
                    }
                    else
                    {
                        txtAccountCode.Enabled = true;
                    }

                    FillControls();
                }
            }
        }

        private void toolCancel_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "";
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            string strCode = null;
            SqlDataReader sdr = null;

            if (String.IsNullOrEmpty(txtAccountCode.Text.Trim()))
            {
                MessageBox.Show("帐户编号不许为空！", "软件提示");
                txtAccountCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtAccountName.Text.Trim()))
            {
                MessageBox.Show("帐户名称不许为空！", "软件提示");
                txtAccountName.Focus();
                return;
            }

            // 添加
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                strCode = "SELECT * FROM BSAccount WHERE AccountCode = @AccountCode";
                SqlParameter[] checkPars = { new SqlParameter("@AccountCode", txtAccountCode.Text.Trim()) };

                try
                {
                    sdr = db.GetDataReader(strCode, checkPars);  // ✅ 传参数
                    sdr.Read();
                    if (!sdr.HasRows)
                    {
                        sdr.Close();

                        strCode = "INSERT INTO BSAccount(AccountCode,AccountName,BankAccount,AccSubject,AccMoney) ";
                        strCode += "VALUES(@AccountCode,@AccountName,@BankAccount,@AccSubject,@AccMoney)";

                        SqlParameter[] pars = ParametersAddValue();  // ✅ 获取参数

                        if (db.ExecDataBySql(strCode, pars) > 0)      // ✅ 传参数
                        {
                            MessageBox.Show("保存成功！", "软件提示");
                            toolStrip1.Tag = "";
                            this.BindDataGridView("");
                            ControlStatus();
                        }
                        else
                        {
                            MessageBox.Show("保存失败！", "软件提示");
                        }
                    }
                    else
                    {
                        MessageBox.Show("编码重复，请重新设置", "软件提示");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "软件提示");
                    throw;
                }
                finally
                {
                    if (sdr != null) sdr.Close();
                }
            }

            // 修改
            else if (toolStrip1.Tag.ToString() == "EDIT")
            {
                string strOldAccountCode = this.dgvAccountInfo[0, this.dgvAccountInfo.CurrentCell.RowIndex].Value.ToString();

                // 若帐户代码被修改过
                if (strOldAccountCode != txtAccountCode.Text.Trim())
                {
                    strCode = "SELECT * FROM BSAccount WHERE AccountCode = @AccountCode";
                    SqlParameter[] checkPars = { new SqlParameter("@AccountCode", txtAccountCode.Text.Trim()) };

                    try
                    {
                        sdr = db.GetDataReader(strCode, checkPars);  // ✅ 传参数
                        sdr.Read();
                        if (sdr.HasRows)
                        {
                            MessageBox.Show("编码重复，请重新设置", "软件提示");
                            this.txtAccountCode.Focus();
                            sdr.Close();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "软件提示");
                        throw;
                    }
                    finally
                    {
                        if (sdr != null) sdr.Close();
                    }
                }

                // 更新数据库
                try
                {
                    strCode = "UPDATE BSAccount SET AccountCode = @AccountCode,AccountName = @AccountName,";
                    strCode += "BankAccount = @BankAccount,AccSubject = @AccSubject,AccMoney = @AccMoney ";
                    strCode += "WHERE AccountCode = @OldAccountCode";  // ✅ 参数化旧值

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldAccountCode", strOldAccountCode));  // ✅ 加旧值参数

                    if (db.ExecDataBySql(strCode, parsList.ToArray()) > 0)
                    {
                        MessageBox.Show("保存成功！", "软件提示");
                        toolStrip1.Tag = "";
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
        }

        private void toolDelete_Click(object sender, EventArgs e)
        {
            string strAccountCode = null;
            string strSql = null;

            if (this.dgvAccountInfo.RowCount == 0)
            {
                return;
            }

            strAccountCode = this.dgvAccountInfo[0, this.dgvAccountInfo.CurrentCell.RowIndex].Value.ToString();

            // 基础类型帐户，无法删除
            if (strAccountCode == "01" || strAccountCode == "02")
            {
                MessageBox.Show("基础类型帐户，无法删除", "软件提示");
                return;
            }

            // 判断是否存在外键约束
            if (commUse.IsExistConstraint("BSAccount", strAccountCode))
            {
                MessageBox.Show("已发生业务关系，无法删除", "软件提示");
                return;
            }

            // ✅ 参数化
            strSql = "DELETE FROM BSAccount WHERE AccountCode = @AccountCode";
            SqlParameter[] pars = { new SqlParameter("@AccountCode", strAccountCode) };

            if (MessageBox.Show("确定要删除吗？", "软件提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                try
                {
                    if (db.ExecDataBySql(strSql, pars) > 0)  // ✅ 传参数
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
                    throw;  // ✅ 保留堆栈
                }

                this.BindDataGridView("");
            }
        }

        private void txtOK_Click(object sender, EventArgs e)
        {
            string strWhere = String.Empty;
            string strConditonName = String.Empty;

            strConditonName = this.cbxCondition.Items[this.cbxCondition.SelectedIndex].ToString();
            switch (strConditonName)
            {
                case "帐户名称":

                    strWhere = " WHERE AccountName LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "银行账号":

                    strWhere = " WHERE BankAccount LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                default:
                    break;
            }
        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtBankAccount_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputNumeric(e, sender as Control);
        }
    }
}
