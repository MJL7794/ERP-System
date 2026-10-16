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
using ERP.SE;
using Microsoft.Data.SqlClient;

namespace ERP.PR
{
    public partial class FormPRPlan : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormPRPlan()
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
            this.btnChoice.Enabled = !this.btnChoice.Enabled;
            this.cbxInvenCode.Enabled = !this.cbxInvenCode.Enabled;
            this.txtQuantity.ReadOnly = !this.txtQuantity.ReadOnly;
            this.dtpFinishDate.Enabled = ! this.dtpFinishDate.Enabled;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            this.txtPRPlanCode.Text = "";
            this.dtpPRPlanDate.Value = Convert.ToDateTime("1900-01-01");
            this.cbxOperatorCode.SelectedIndex = -1;
            this.txtSEOrderCode.Text = "";
            this.cbxInvenCode.SelectedIndex = -1;
            this.txtQuantity.Text = "";
            this.dtpFinishDate.Value = Convert.ToDateTime("1900-01-01");
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
            this.txtPRPlanCode.Text = this.dgvPRPlanInfo["PRPlanCode", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value.ToString();
            this.dtpPRPlanDate.Value = Convert.ToDateTime(this.dgvPRPlanInfo["PRPlanDate", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value);
            this.cbxOperatorCode.SelectedValue = this.dgvPRPlanInfo["OperatorCode", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value;
            this.txtSEOrderCode.Text = this.dgvPRPlanInfo["SEOrderCode", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxInvenCode.SelectedValue = this.dgvPRPlanInfo["InvenCode", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value;
            this.txtQuantity.Text = this.dgvPRPlanInfo["Quantity", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value.ToString();
            this.dtpFinishDate.Value = Convert.ToDateTime(this.dgvPRPlanInfo["FinishDate", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value);
            this.cbxIsFlag.SelectedValue = this.dgvPRPlanInfo["IsFlag", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value;
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM PRPlan " + strWhere; ;

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

        /// <summary>
        /// 设置参数值
        /// </summary>
        private SqlParameter[] ParametersAddValue()
        {
            List<SqlParameter> pars = new List<SqlParameter>();

            pars.Add(new SqlParameter("@PRPlanCode", txtPRPlanCode.Text.Trim()));
            pars.Add(new SqlParameter("@PRPlanDate", dtpPRPlanDate.Value));

            if (cbxOperatorCode.SelectedValue == null)
                pars.Add(new SqlParameter("@OperatorCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@OperatorCode", cbxOperatorCode.SelectedValue.ToString()));

            if (String.IsNullOrEmpty(txtSEOrderCode.Text.Trim()))
                pars.Add(new SqlParameter("@SEOrderCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@SEOrderCode", txtSEOrderCode.Text.Trim()));

            if (cbxInvenCode.SelectedValue == null)
                pars.Add(new SqlParameter("@InvenCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@InvenCode", cbxInvenCode.SelectedValue.ToString()));

            if (String.IsNullOrEmpty(txtQuantity.Text.Trim()))
                pars.Add(new SqlParameter("@Quantity", 0));
            else
                pars.Add(new SqlParameter("@Quantity", Convert.ToInt32(txtQuantity.Text.Trim())));

            pars.Add(new SqlParameter("@FinishDate", dtpFinishDate.Value));

            if (cbxIsFlag.SelectedValue == null)
                pars.Add(new SqlParameter("@IsFlag", DBNull.Value));
            else
                pars.Add(new SqlParameter("@IsFlag", cbxIsFlag.SelectedValue.ToString()));

            return pars.ToArray();
        }

        private void FormPRPlan_Load(object sender, EventArgs e)
        {
            //权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            commUse.CortrolButtonEnabled(toolCheck, this);
            commUse.CortrolButtonEnabled(toolUnCheck, this);

            //ComboBox绑定到数据源
            commUse.BindComboBox(cbxOperatorCode, "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(cbxInvenCode, "InvenCode", "InvenName","Select InvenCode,InvenName From BSInven", "BSInven");
            commUse.BindComboBox(cbxIsFlag, "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvPRPlanInfo.Columns["OperatorCode"], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvPRPlanInfo.Columns["InvenCode"], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvPRPlanInfo.Columns["IsFlag"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
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
            toolStrip1.Tag = "ADD"; //添加标识

            dtpPRPlanDate.Value = DateTime.Today;
            cbxOperatorCode.SelectedValue = PropertyClass.OperatorCode;
            txtQuantity.Text = "1";
            this.dtpFinishDate.Value = DateTime.Today;
            cbxIsFlag.SelectedValue = "0";

            txtPRPlanCode.Text = commUse.BuildBillCode("PRPlan", "PRPlanCode", "PRPlanDate", dtpPRPlanDate.Value);
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //修改标识
        }

        private void dgvPRPlanInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT" && dgvPRPlanInfo.RowCount > 0)
            {
                if (this.dgvPRPlanInfo["IsFlag", this.dgvPRPlanInfo.CurrentRow.Index].Value.ToString() == "1")
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

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputInteger(e);
        }

        private void btnChoice_Click(object sender, EventArgs e)
        {
            FormBrowseSEOrder formBrowseSEOrder = new FormBrowseSEOrder();
            formBrowseSEOrder.Owner = this;
            formBrowseSEOrder.ShowDialog();
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            string strCode = null;
            SqlDataReader sdr = null;

            if (String.IsNullOrEmpty(txtPRPlanCode.Text.Trim()))
            {
                MessageBox.Show("单据编号不许为空！", "软件提示");
                txtPRPlanCode.Focus();
                return;
            }

            if (cbxInvenCode.SelectedValue == null)
            {
                MessageBox.Show("产品名称不许为空！", "软件提示");
                cbxInvenCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                MessageBox.Show("计划产量不许为空！", "软件提示");
                txtQuantity.Focus();
                return;
            }
            else
            {
                if (Convert.ToInt32(txtQuantity.Text.Trim()) == 0)
                {
                    MessageBox.Show("计划产量不能等于零", "软件提示");
                    txtQuantity.Focus();
                    return;
                }
            }

            if (dtpFinishDate.Value < dtpPRPlanDate.Value)
            {
                MessageBox.Show("完工日期不许小于单据日期！","软件提示");
                dtpFinishDate.Focus();
                return;
            }

            //判断该存货是否具有物料清单
            strCode = "SELECT * FROM BSBom WHERE ProInvenCode = @ProInvenCode";
            SqlParameter[] bomPars = { new SqlParameter("@ProInvenCode", cbxInvenCode.SelectedValue) };

            try
            {
                sdr = db.GetDataReader(strCode, bomPars);
                sdr.Read();
                if (!sdr.HasRows)
                {
                    MessageBox.Show("该产品没有物料清单，无法制定主生产计划！", "软件提示");
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

            //添加
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                try
                {
                    strCode = "INSERT INTO PRPlan(PRPlanCode,PRPlanDate,OperatorCode,SEOrderCode,InvenCode,Quantity,FinishDate,IsFlag) ";
                    strCode += "VALUES(@PRPlanCode,@PRPlanDate,@OperatorCode,@SEOrderCode,@InvenCode,@Quantity,@FinishDate,@IsFlag)";

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
                string strPRPlanCode = txtPRPlanCode.Text.Trim();

                try
                {
                    strCode = "UPDATE PRPlan SET PRPlanCode = @PRPlanCode,PRPlanDate = @PRPlanDate,OperatorCode = @OperatorCode, SEOrderCode = @SEOrderCode,";
                    strCode += "InvenCode = @InvenCode,Quantity = @Quantity,";
                    strCode += "FinishDate = @FinishDate,IsFlag = @IsFlag";
                    strCode += " WHERE PRPlanCode = @OldPRPlanCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldPRPlanCode", strPRPlanCode));

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
            string strPRPlanCode = null; //主生产计划号
            string strSql = null;
            string strFlag = null;

            if (this.dgvPRPlanInfo.RowCount <= 0)
            {
                return;
            }

            strPRPlanCode = this.dgvPRPlanInfo["PRPlanCode", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value.ToString();
            strFlag = this.dgvPRPlanInfo["IsFlag", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value.ToString();

            if (strFlag == "1")
            {
                MessageBox.Show("该单据已审核，不许删除！", "软件提示");
                return;
            }

            strSql = "DELETE FROM PRPlan WHERE PRPlanCode = @PRPlanCode";
            SqlParameter[] pars = { new SqlParameter("@PRPlanCode", strPRPlanCode) };

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

                this.BindDataGridView("");
            }
        }

        private void toolCheck_Click(object sender, EventArgs e)
        {
            string strPRPlanCode = null;   // 主生产计划号
            string strSql = null;
            string strFlag = null;         // 审核标记

            if (dgvPRPlanInfo.RowCount == 0)
            {
                return;
            }

            strPRPlanCode = this.dgvPRPlanInfo["PRPlanCode", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value.ToString();
            strFlag = this.dgvPRPlanInfo["IsFlag", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value.ToString();

            if (strFlag == "1")
            {
                MessageBox.Show("该单据已审核过，不许再次审核！", "软件提示");
                return;
            }

            // ✅ 参数化
            strSql = "UPDATE PRPlan SET IsFlag = '1' WHERE PRPlanCode = @PRPlanCode";
            SqlParameter[] pars = { new SqlParameter("@PRPlanCode", strPRPlanCode) };

            try
            {
                if (db.ExecDataBySql(strSql, pars) > 0)
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
                throw;
            }

            this.BindDataGridView("");
        }

        private void toolUnCheck_Click(object sender, EventArgs e)
        {
            string strPRPlanCode = null;   // 主生产计划号
            string strSql = null;
            string strFlag = null;         // 审核标记
            SqlDataReader sdr = null;

            if (dgvPRPlanInfo.RowCount == 0)
            {
                return;
            }

            strPRPlanCode = this.dgvPRPlanInfo["PRPlanCode", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value.ToString();
            strFlag = this.dgvPRPlanInfo["IsFlag", this.dgvPRPlanInfo.CurrentCell.RowIndex].Value.ToString();

            // ✅ 检查是否已发生业务关系（参数化）
            strSql = "SELECT * FROM PRProduce WHERE PRPlanCode = @PRPlanCode";
            SqlParameter[] checkPars = { new SqlParameter("@PRPlanCode", strPRPlanCode) };

            try
            {
                sdr = db.GetDataReader(strSql, checkPars);
                sdr.Read();

                if (sdr.HasRows)
                {
                    MessageBox.Show("该单据已发生业务关系，无法弃审！", "软件提示");
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

            if (strFlag == "0")
            {
                MessageBox.Show("该单据未审核，无需弃审！", "软件提示");
                return;
            }

            // ✅ 更新审核标记（参数化）
            strSql = "UPDATE PRPlan SET IsFlag = '0' WHERE PRPlanCode = @PRPlanCode";
            SqlParameter[] pars = { new SqlParameter("@PRPlanCode", strPRPlanCode) };

            try
            {
                if (db.ExecDataBySql(strSql, pars) > 0)
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

                    strWhere = " WHERE PRPlanCode LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "单据日期":

                    strWhere = " WHERE SUBSTRING(CONVERT(VARCHAR(20),PRPlanDate,20),1,10) LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                default:
                    break;
            }
        }

        private void dgvPRPlanInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
