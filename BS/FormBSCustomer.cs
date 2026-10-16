using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using ERP.ComClass;
using ERP.DataClass;

namespace ERP.BS
{
    public partial class FormBSCustomer : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormBSCustomer()
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
            this.txtCustomerCode.ReadOnly = !this.txtCustomerCode.ReadOnly;
            this.txtCustomerName.ReadOnly = !this.txtCustomerName.ReadOnly;
            this.cbxEmployeeCode.Enabled = !this.cbxEmployeeCode.Enabled;
            this.txtAtrMan.ReadOnly = !this.txtAtrMan.ReadOnly;
            this.txtTelephoneCode.ReadOnly = !this.txtTelephoneCode.ReadOnly;
            this.txtFaxCode.ReadOnly = !this.txtFaxCode.ReadOnly;
            this.txtPostCode.ReadOnly = !this.txtPostCode.ReadOnly;
            this.txtEmail.ReadOnly = !this.txtEmail.ReadOnly;
            this.txtLinkman.ReadOnly = !this.txtLinkman.ReadOnly;
            this.txtUrl.ReadOnly = !this.txtUrl.ReadOnly;
            this.txtAddress.ReadOnly = !this.txtAddress.ReadOnly;
            //
            this.cbxGradeCode.Enabled = !this.cbxGradeCode.Enabled;
            this.cbxStateCode.Enabled = !this.cbxStateCode.Enabled;
            this.cbxCreditCode.Enabled = !this.cbxCreditCode.Enabled;
            this.cbxTradeCode.Enabled = !this.cbxTradeCode.Enabled;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            //窗体控件状态切换
            this.txtCustomerCode.Text = "";
            this.txtCustomerName.Text = "";
            this.cbxEmployeeCode.SelectedIndex = -1;
            this.txtAtrMan.Text = "";
            this.txtTelephoneCode.Text = "";
            this.txtFaxCode.Text = "";
            this.txtPostCode.Text = "";
            this.txtEmail.Text = "";
            this.txtLinkman.Text = "";
            this.txtUrl.Text = "";
            this.txtAddress.Text = "";
            this.cbxGradeCode.SelectedIndex = -1;
            this.cbxStateCode.SelectedIndex = -1;
            this.cbxCreditCode.SelectedIndex = -1;
            this.cbxTradeCode.SelectedIndex = -1;
        }

        private void BindToolStripComboBox()
        {
            this.cbxCondition.Items.Add("客户名称");
            this.cbxCondition.Items.Add("联系人");
            this.cbxCondition.Items.Add("地址");
        }

        /// <summary>
        /// 设置控件的显示值
        /// </summary>
        private void FillControls()
        {
            this.txtCustomerCode.Text = this.dgvCustomerInfo[0, this.dgvCustomerInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtCustomerName.Text = this.dgvCustomerInfo[1, this.dgvCustomerInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxEmployeeCode.SelectedValue = this.dgvCustomerInfo[2, this.dgvCustomerInfo.CurrentCell.RowIndex].Value;
            this.txtAtrMan.Text = this.dgvCustomerInfo[3, this.dgvCustomerInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtTelephoneCode.Text = this.dgvCustomerInfo[4, this.dgvCustomerInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtFaxCode.Text = this.dgvCustomerInfo[5, this.dgvCustomerInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtPostCode.Text = this.dgvCustomerInfo[6, this.dgvCustomerInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtEmail.Text = this.dgvCustomerInfo[7, this.dgvCustomerInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtLinkman.Text = this.dgvCustomerInfo[8, this.dgvCustomerInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtUrl.Text = this.dgvCustomerInfo[9, this.dgvCustomerInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtAddress.Text = this.dgvCustomerInfo[10, this.dgvCustomerInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxGradeCode.SelectedValue = this.dgvCustomerInfo[11, this.dgvCustomerInfo.CurrentCell.RowIndex].Value;
            this.cbxStateCode.SelectedValue = this.dgvCustomerInfo[12, this.dgvCustomerInfo.CurrentCell.RowIndex].Value;
            this.cbxCreditCode.SelectedValue = this.dgvCustomerInfo[13, this.dgvCustomerInfo.CurrentCell.RowIndex].Value;
            this.cbxTradeCode.SelectedValue = this.dgvCustomerInfo[14, this.dgvCustomerInfo.CurrentCell.RowIndex].Value;
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += " FROM BSCustomer" + strWhere; ;

            try
            {
                this.dgvCustomerInfo.DataSource = db.GetDataSet(strSql, "BSCustomer").Tables["BSCustomer"];
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

            pars.Add(new SqlParameter("@CustomerCode", txtCustomerCode.Text.Trim()));
            pars.Add(new SqlParameter("@CustomerName", txtCustomerName.Text.Trim()));

            if (cbxEmployeeCode.SelectedValue == null)
                pars.Add(new SqlParameter("@EmployeeCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@EmployeeCode", cbxEmployeeCode.SelectedValue.ToString()));

            pars.Add(new SqlParameter("@AtrMan", txtAtrMan.Text.Trim()));
            pars.Add(new SqlParameter("@TelephoneCode", txtTelephoneCode.Text.Trim()));
            pars.Add(new SqlParameter("@FaxCode", txtFaxCode.Text.Trim()));
            pars.Add(new SqlParameter("@PostCode", txtPostCode.Text.Trim()));
            pars.Add(new SqlParameter("@Email", txtEmail.Text.Trim()));
            pars.Add(new SqlParameter("@Linkman", txtLinkman.Text.Trim()));
            pars.Add(new SqlParameter("@Url", txtUrl.Text.Trim()));
            pars.Add(new SqlParameter("@Address", txtAddress.Text.Trim()));

            if (cbxGradeCode.SelectedValue == null)
                pars.Add(new SqlParameter("@GradeCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@GradeCode", cbxGradeCode.SelectedValue.ToString()));

            if (cbxStateCode.SelectedValue == null)
                pars.Add(new SqlParameter("@StateCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@StateCode", cbxStateCode.SelectedValue.ToString()));

            if (cbxCreditCode.SelectedValue == null)
                pars.Add(new SqlParameter("@CreditCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@CreditCode", cbxCreditCode.SelectedValue.ToString()));

            if (cbxTradeCode.SelectedValue == null)
                pars.Add(new SqlParameter("@TradeCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@TradeCode", cbxTradeCode.SelectedValue.ToString()));

            return pars.ToArray();
        }

        private void FormCustomer_Load(object sender, EventArgs e)
        {

            //权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            //ComboBox绑定到数据源
            commUse.BindComboBox(cbxEmployeeCode, "EmployeeCode", "EmployeeName", "select EmployeeCode, EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(cbxGradeCode, "GradeCode", "GradeName", "select GradeCode, GradeName from CUGrade", "CUGrade");
            commUse.BindComboBox(cbxStateCode, "StateCode", "StateName", "select StateCode, StateName from CUState", "CUState");
            commUse.BindComboBox(cbxCreditCode, "CreditCode", "CreditName", "select CreditCode, CreditName from CUCredit", "CUCredit");
            commUse.BindComboBox(cbxTradeCode, "TradeCode", "TradeName", "select TradeCode, TradeName from CUTrade", "CUTrade");

            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvCustomerInfo.Columns[2], "EmployeeCode", "EmployeeName", "select EmployeeCode, EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvCustomerInfo.Columns[11], "GradeCode", "GradeName", "select GradeCode, GradeName from CUGrade", "CUGrade");
            commUse.BindComboBox(this.dgvCustomerInfo.Columns[12], "StateCode", "StateName", "select StateCode, StateName from CUState", "CUState");
            commUse.BindComboBox(this.dgvCustomerInfo.Columns[13], "CreditCode", "CreditName", "select CreditCode, CreditName from CUCredit", "CUCredit");
            commUse.BindComboBox(this.dgvCustomerInfo.Columns[14], "TradeCode", "TradeName", "select TradeCode, TradeName from CUTrade", "CUTrade");
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
            toolStrip1.Tag = "ADD"; //添加状态
            txtCustomerCode.Enabled = true;
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //编辑状态
            txtCustomerCode.Enabled = false;
        }

        private void toolreflush_Click(object sender, EventArgs e)
        {
            this.BindDataGridView("");
        }

        private void dgvCustomerInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                if (dgvCustomerInfo.RowCount > 0)
                {
                    //判断当前记录的主键值是否存在外键约束
                    if (commUse.IsExistConstraint("BSCustomer", dgvCustomerInfo[0, dgvCustomerInfo.CurrentCell.RowIndex].Value.ToString()))
                    {
                        txtCustomerCode.Enabled = false;
                    }
                    else
                    {
                        txtCustomerCode.Enabled = true;
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

        private void txtOK_Click(object sender, EventArgs e)
        {
            string strWhere = String.Empty;
            string strConditonName = String.Empty;

            strConditonName = this.cbxCondition.Items[this.cbxCondition.SelectedIndex].ToString();
            switch (strConditonName)
            {
                case "客户名称":

                    strWhere = " WHERE CustomerName LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "联系人":

                    strWhere = " WHERE Linkman LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "地址":

                    strWhere = " WHERE Address LIKE '%" + txtKeyWord.Text.Trim() + "%'";
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

        private void toolDelete_Click(object sender, EventArgs e)
        {
            string strCustomerCode = null;
            string strSql = null;

            if (this.dgvCustomerInfo.RowCount == 0)
            {
                return;
            }

            strCustomerCode = this.dgvCustomerInfo[0, this.dgvCustomerInfo.CurrentCell.RowIndex].Value.ToString();

            //判断当前记录的主键值是否存在外键约束
            if (commUse.IsExistConstraint("BSCustomer", strCustomerCode))
            {
                MessageBox.Show("已发生业务关系，无法删除", "软件提示");
                return;
            }

            strSql = "DELETE FROM BSCustomer WHERE CustomerCode = '" + strCustomerCode + "'";

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

        private void toolSave_Click(object sender, EventArgs e)
        {
            string strCode = null; ;
            SqlDataReader sdr = null;

            if (String.IsNullOrEmpty(txtCustomerCode.Text.Trim()))
            {
                MessageBox.Show("客户编号不许为空！","软件提示");
                txtCustomerCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtCustomerName.Text.Trim()))
            {
                MessageBox.Show("客户名称不许为空！", "软件提示");
                txtCustomerName.Focus();
                return;
            }

            //添加
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                strCode = "SELECT * FROM BSCustomer WHERE CustomerCode = @CustomerCode";
                SqlParameter[] checkPars = { new SqlParameter("@CustomerCode", txtCustomerCode.Text.Trim()) };

                try
                {
                    sdr = db.GetDataReader(strCode, checkPars);
                    sdr.Read();
                    if (!sdr.HasRows)
                    {
                        sdr.Close();

                        strCode = "INSERT INTO BSCustomer(CustomerCode,CustomerName,EmployeeCode,AtrMan,TelephoneCode,FaxCode,PostCode,Email,Linkman,Url,Address,GradeCode,StateCode,CreditCode,TradeCode) ";
                        strCode += "VALUES(@CustomerCode,@CustomerName,@EmployeeCode,@AtrMan,@TelephoneCode,@FaxCode,@PostCode,@Email,@Linkman,@Url,@Address,@GradeCode,@StateCode,@CreditCode,@TradeCode)";

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
                        this.txtCustomerCode.Focus();
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

            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                string strOldCustomerCode = null;

                strOldCustomerCode = this.dgvCustomerInfo[0, this.dgvCustomerInfo.CurrentCell.RowIndex].Value.ToString();

                //客户代码被修改过
                if (strOldCustomerCode != txtCustomerCode.Text.Trim())
                {
                    strCode = "SELECT * FROM BSCustomer WHERE CustomerCode = @CustomerCode";
                    SqlParameter[] checkPars = { new SqlParameter("@CustomerCode", txtCustomerCode.Text.Trim()) };

                    try
                    {
                        sdr = db.GetDataReader(strCode, checkPars);
                        sdr.Read();
                        if (sdr.HasRows)
                        {
                            MessageBox.Show("编码重复，请重新设置", "软件提示");
                            this.txtCustomerCode.Focus();
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
                    strCode = "UPDATE BSCustomer SET CustomerCode = @CustomerCode,CustomerName = @CustomerName,";
                    strCode += "EmployeeCode = @EmployeeCode,AtrMan = @AtrMan,TelephoneCode = @TelephoneCode,";
                    strCode += "FaxCode = @FaxCode,PostCode = @PostCode,Email = @Email,";
                    strCode += "Linkman = @Linkman,Url = @Url,Address = @Address,";
                    strCode += "GradeCode = @GradeCode,StateCode = @StateCode,CreditCode = @CreditCode,";
                    strCode += "TradeCode = @TradeCode ";
                    strCode += "WHERE CustomerCode = @OldCustomerCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldCustomerCode", strOldCustomerCode));

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

        private void dgvCustomerInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
