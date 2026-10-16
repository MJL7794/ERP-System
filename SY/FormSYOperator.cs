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

namespace ERP.SY
{
    public partial class FormSYOperator : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormSYOperator()
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
            //窗体控件状态切换
            this.txtOperatorCode.ReadOnly = !this.txtOperatorCode.ReadOnly;
            this.txtOperatorName.ReadOnly = !this.txtOperatorName.ReadOnly;
            this.txtPassWord.ReadOnly = !this.txtPassWord.ReadOnly;
            this.txtAgainPassWord.ReadOnly = !this.txtAgainPassWord.ReadOnly;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            //窗体控件状态切换
            this.txtOperatorCode.Text = "";
            this.txtOperatorName.Text = "";
            this.txtPassWord.Text = "";
            this.txtAgainPassWord.Text = "";
        }

        private void BindToolStripComboBox()
        {
            this.cbxCondition.Items.Add("用户编码");
            this.cbxCondition.Items.Add("用户名称");
        }

        /// <summary>
        /// 设置控件的显示值
        /// </summary>
        private void FillControls()
        {
            this.txtOperatorCode.Text = this.dgvSYOperatorInfo["OperatorCode", this.dgvSYOperatorInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtOperatorName.Text = this.dgvSYOperatorInfo["OperatorName", this.dgvSYOperatorInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtPassWord.Text = this.dgvSYOperatorInfo["PassWord", this.dgvSYOperatorInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtAgainPassWord.Text = this.dgvSYOperatorInfo["PassWord", this.dgvSYOperatorInfo.CurrentCell.RowIndex].Value.ToString();
        }

        /// <summary>
        /// DataGridView绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += " FROM SYOperator" + strWhere; ;

            try
            {
                this.dgvSYOperatorInfo.DataSource = db.GetDataSet(strSql, "SYOperator").Tables["SYOperator"];
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

            pars.Add(new SqlParameter("@OperatorCode", txtOperatorCode.Text.Trim()));
            pars.Add(new SqlParameter("@OperatorName", txtOperatorName.Text.Trim()));
            pars.Add(new SqlParameter("@PassWord", txtPassWord.Text));
            pars.Add(new SqlParameter("@IsAdmin", "0"));

            return pars.ToArray();
        }

        private void FormSYOperator_Load(object sender, EventArgs e)
        {
            //设置权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            //绑定到数据源
            commUse.BindComboBox(this.dgvSYOperatorInfo.Columns["IsAdmin"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
            BindDataGridView("");
            this.BindToolStripComboBox();
            this.cbxCondition.SelectedIndex = 0;
            toolStrip1.Tag = "";
        }

        private void toolAdd_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "ADD";
            txtOperatorCode.Enabled = true;
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            txtPassWord.ReadOnly = true;
            txtAgainPassWord.ReadOnly = true;
            toolStrip1.Tag = "EDIT";
            txtOperatorCode.Enabled = false;
        }

        private void toolreflush_Click(object sender, EventArgs e)
        {
            this.BindDataGridView("");
        }

        private void dgvSYOperatorInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                if (dgvSYOperatorInfo.RowCount > 0)
                {
                    string strIsAdmin = this.dgvSYOperatorInfo["IsAdmin", this.dgvSYOperatorInfo.CurrentCell.RowIndex].Value.ToString();
                    
                    //系统管理员不许编辑
                    if (strIsAdmin == "1")
                    {
                        MessageBox.Show("系统管理员，不许编辑！", "软件提示");
                        return;
                    }

                    //判断当前记录的主键值是否存在外键约束,若存在不许修改该操作员的编码
                    if (commUse.IsExistConstraint("SYOperator", dgvSYOperatorInfo["OperatorCode", dgvSYOperatorInfo.CurrentCell.RowIndex].Value.ToString()))
                    {
                        this.txtOperatorCode.Enabled = false;
                    }
                    else
                    {
                        this.txtOperatorCode.Enabled = true;
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

            if (String.IsNullOrEmpty(txtOperatorCode.Text.Trim()))
            {
                MessageBox.Show("用户编码不许为空！", "软件提示");
                txtOperatorCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtOperatorName.Text.Trim()))
            {
                MessageBox.Show("用户名称不许为空！", "软件提示");
                txtOperatorName.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtPassWord.Text))
            {
                MessageBox.Show("密码不许为空！", "软件提示");
                txtPassWord.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtAgainPassWord.Text))
            {
                MessageBox.Show("确认密码不许为空！", "软件提示");
                txtAgainPassWord.Focus();
                return;
            }

            if(txtPassWord.Text != txtAgainPassWord.Text)
            {
                MessageBox.Show("两次输入的密码不相同！", "软件提示");
                txtPassWord.Focus();
                return;
            }

            //添加
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                strCode = "SELECT * FROM SYOperator WHERE OperatorCode = @OperatorCode";
                SqlParameter[] checkPars = { new SqlParameter("@OperatorCode", txtOperatorCode.Text.Trim()) };

                try
                {
                    sdr = db.GetDataReader(strCode, checkPars);
                    sdr.Read();
                    if (!sdr.HasRows)
                    {
                        sdr.Close();

                        strCode = "INSERT INTO SYOperator(OperatorCode,OperatorName,PassWord,IsAdmin) ";
                        strCode += "VALUES(@OperatorCode,@OperatorName,@PassWord,@IsAdmin)";

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
                    else
                    {
                        MessageBox.Show("编码重复，请重新设置", "软件提示");
                        this.txtOperatorCode.Focus();
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
            //修改
            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                string strOldOperatorCode = this.dgvSYOperatorInfo["OperatorCode", this.dgvSYOperatorInfo.CurrentCell.RowIndex].Value.ToString();

                // 用户编码被修改过
                if (strOldOperatorCode != txtOperatorCode.Text.Trim())
                {
                    strCode = "SELECT * FROM SYOperator WHERE OperatorCode = @OperatorCode";
                    SqlParameter[] checkPars = { new SqlParameter("@OperatorCode", txtOperatorCode.Text.Trim()) };

                    try
                    {
                        sdr = db.GetDataReader(strCode, checkPars);
                        sdr.Read();
                        if (sdr.HasRows)
                        {
                            MessageBox.Show("用户编码重复，请重新设置", "软件提示");
                            this.txtOperatorCode.Focus();
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
                    strCode = "UPDATE SYOperator SET OperatorCode = @OperatorCode,OperatorName = @OperatorName ";
                    strCode += "WHERE OperatorCode = @OldOperatorCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldOperatorCode", strOldOperatorCode));

                    if (db.ExecDataBySql(strCode, parsList.ToArray()) > 0)
                    {
                        MessageBox.Show("保存成功！", "软件提示");
                        this.BindDataGridView("");
                        ControlStatus();
                        txtAgainPassWord.ReadOnly = true;
                        txtPassWord.ReadOnly = true;
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
            string strIsAdmin = null;
            string strOperatorCode = null;

            if (this.dgvSYOperatorInfo.RowCount == 0) return;

            strIsAdmin = this.dgvSYOperatorInfo["IsAdmin", this.dgvSYOperatorInfo.CurrentCell.RowIndex].Value.ToString();

            if (strIsAdmin == "1")
            {
                MessageBox.Show("系统管理员，不许删除！", "软件提示");
                return;
            }

            strOperatorCode = this.dgvSYOperatorInfo["OperatorCode", this.dgvSYOperatorInfo.CurrentCell.RowIndex].Value.ToString();

            if (strOperatorCode == PropertyClass.OperatorCode)
            {
                MessageBox.Show("操作员不许删除自己！", "软件提示");
                return;
            }

            if (commUse.IsExistConstraint("SYOperator", strOperatorCode))
            {
                MessageBox.Show("已发生业务关系，无法删除", "软件提示");
                return;
            }

            // ✅ 事务参数化
            List<(string, SqlParameter[])> commands = new List<(string, SqlParameter[])>();

            string delOperatorSql = "DELETE FROM SYOperator WHERE OperatorCode = @OperatorCode";
            commands.Add((delOperatorSql, new SqlParameter[] { new SqlParameter("@OperatorCode", strOperatorCode) }));

            string delRightSql = "DELETE FROM SYAssignRight WHERE OperatorCode = @OperatorCode";
            commands.Add((delRightSql, new SqlParameter[] { new SqlParameter("@OperatorCode", strOperatorCode) }));

            if (MessageBox.Show("确定要删除吗？", "软件提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                try
                {
                    if (db.ExecDataBySqls(commands))
                        MessageBox.Show("删除成功！", "软件提示");
                    else
                        MessageBox.Show("删除失败！", "软件提示");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "软件提示");
                    throw;
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
                case "用户名称":

                    strWhere = " WHERE OperatorName LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "用户编码":

                    strWhere = " WHERE OperatorCode LIKE '%" + txtKeyWord.Text.Trim() + "%'";
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
    }
}
