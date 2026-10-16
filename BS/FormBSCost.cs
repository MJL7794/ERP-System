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
    public partial class FormBSCost : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormBSCost()
        {
            InitializeComponent();
        }

        private void ControlStatus()
        {
            //按工具栏钮状态切换
            this.toolSave.Enabled = !this.toolSave.Enabled;
            this.toolCancel.Enabled = !this.toolCancel.Enabled;
            commUse.CortrolButtonEnabled(toolAdd,this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);

            //窗体控件状态切换
            this.txtCostCode.ReadOnly = !this.txtCostCode.ReadOnly;
            this.txtCostName.ReadOnly = !this.txtCostName.ReadOnly;
            this.cbxCostTypeCode.Enabled = !this.cbxCostTypeCode.Enabled;
            this.rtbRemark.ReadOnly = !this.rtbRemark.ReadOnly;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            //窗体控件状态切换
            this.txtCostCode.Text = "";
            this.txtCostName.Text = "";
            this.cbxCostTypeCode.SelectedIndex = -1;
            this.rtbRemark.Text = "";
        }

        private void BindToolStripComboBox()
        {
            this.cbxCondition.Items.Add("费用名称");
            this.cbxCondition.Items.Add("备注");
        }

        /// <summary>
        /// 设置控件的显示值
        /// </summary>
        private void FillControls()
        {
            this.txtCostCode.Text = this.dgvCostInfo[0, this.dgvCostInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtCostName.Text = this.dgvCostInfo[1, this.dgvCostInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxCostTypeCode.SelectedValue = this.dgvCostInfo[2, this.dgvCostInfo.CurrentCell.RowIndex].Value;
            this.rtbRemark.Text = this.dgvCostInfo[3, this.dgvCostInfo.CurrentCell.RowIndex].Value.ToString();
        }

        /// <summary>
        /// DataGridView绑定数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT CostCode,CostName,";
            strSql += "CostTypeCode,Remark ";
            strSql += " FROM BSCost" + strWhere; ;

            try
            {
                this.dgvCostInfo.DataSource = db.GetDataSet(strSql, "BSCost").Tables["BSCost"];
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

            pars.Add(new SqlParameter("@CostCode", txtCostCode.Text.Trim()));
            pars.Add(new SqlParameter("@CostName", txtCostName.Text.Trim()));

            if (cbxCostTypeCode.SelectedValue == null)
                pars.Add(new SqlParameter("@CostTypeCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@CostTypeCode", cbxCostTypeCode.SelectedValue.ToString()));

            pars.Add(new SqlParameter("@Remark", rtbRemark.Text.Trim()));

            return pars.ToArray();
        }

        private void FormCost_Load(object sender, EventArgs e)
        {
            //权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            //ComboBox绑定到数据源
            commUse.BindComboBox(cbxCostTypeCode, "CostTypeCode", "CostTypeName", "select * from BSCostType", "BSCostType");
            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvCostInfo.Columns[2], "CostTypeCode", "CostTypeName", "select * from BSCostType", "BSCostType");
            BindDataGridView("");
            this.BindToolStripComboBox();
            this.cbxCondition.SelectedIndex = 0;
            toolStrip1.Tag = "";
        }

        private void toolAdd_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "ADD"; //添加状态
            txtCostCode.Enabled = true;
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //修改状态
            txtCostCode.Enabled = false;
        }

        private void toolreflush_Click(object sender, EventArgs e)
        {
            this.BindDataGridView("");
        }

        private void dgvCostInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                if (dgvCostInfo.RowCount > 0)
                {
                    //判断当前记录的主键值是否存在外键约束
                    if (commUse.IsExistConstraint("BSCost", dgvCostInfo[0, dgvCostInfo.CurrentCell.RowIndex].Value.ToString()))
                    {
                        txtCostCode.Enabled = false;
                    }
                    else
                    {
                        txtCostCode.Enabled = true;
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
            string strCode = null; ;
            SqlDataReader sdr = null;

            if (String.IsNullOrEmpty(txtCostCode.Text.Trim()))
            {
                MessageBox.Show("费用编号不许为空！", "软件提示");
                txtCostCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtCostName.Text.Trim()))
            {
                MessageBox.Show("费用名称不许为空！", "软件提示");
                txtCostName.Focus();
                return;
            }

            if (cbxCostTypeCode.SelectedValue == null)
            {
                MessageBox.Show("费用类别不许为空！", "软件提示");
                cbxCostTypeCode.Focus();
                return;
            }

            //添加
            //添加
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                strCode = "SELECT * FROM BSCost WHERE CostCode = @CostCode";
                SqlParameter[] checkPars = { new SqlParameter("@CostCode", txtCostCode.Text.Trim()) };

                try
                {
                    sdr = db.GetDataReader(strCode, checkPars);
                    sdr.Read();
                    if (!sdr.HasRows)
                    {
                        sdr.Close();

                        strCode = "INSERT INTO BSCost(CostCode,CostName,CostTypeCode,Remark) ";
                        strCode += "VALUES(@CostCode,@CostName,@CostTypeCode,@Remark)";

                        SqlParameter[] pars = ParametersAddValue();

                        if (db.ExecDataBySql(strCode, pars) > 0)
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
                        this.txtCostCode.Focus();
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
                string strOldCostCode = null;

                //未修改的费用代码
                strOldCostCode = this.dgvCostInfo[0, this.dgvCostInfo.CurrentCell.RowIndex].Value.ToString();

                //费用代码被修改过
                if (strOldCostCode != txtCostCode.Text.Trim())
                {
                    strCode = "SELECT * FROM BSCost WHERE CostCode = @CostCode";
                    SqlParameter[] checkPars = { new SqlParameter("@CostCode", txtCostCode.Text.Trim()) };

                    try
                    {
                        sdr = db.GetDataReader(strCode, checkPars);
                        sdr.Read();
                        if (sdr.HasRows)
                        {
                            MessageBox.Show("编码重复，请重新设置", "软件提示");
                            this.txtCostCode.Focus();
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

                //更新数据库
                try
                {
                    strCode = "UPDATE BSCost SET CostCode = @CostCode,CostName = @CostName,";
                    strCode += "CostTypeCode = @CostTypeCode,Remark = @Remark ";
                    strCode += "WHERE CostCode = @OldCostCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldCostCode", strOldCostCode));

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
            string strCostCode = null;
            string strSql = null;

            if (this.dgvCostInfo.RowCount == 0)
            {
                return;
            }

            strCostCode = this.dgvCostInfo[0, this.dgvCostInfo.CurrentCell.RowIndex].Value.ToString();

            //判断当前记录的主键值是否存在外键约束
            if (commUse.IsExistConstraint("BSCost", strCostCode))
            {
                MessageBox.Show("已发生业务关系，无法删除", "软件提示");
                return;
            }

            strSql = "DELETE FROM BSCost WHERE CostCode = '" + strCostCode + "'";

            if (MessageBox.Show("确定要删除吗？", "软件提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                try
                {
                    if (db.ExecDataBySql(strSql) > 0)
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
                    throw ex;
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
                case "费用名称":

                    strWhere = " WHERE CostName LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "备注":

                    strWhere = " WHERE Remark LIKE '%" + txtKeyWord.Text.Trim() + "%'";
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
