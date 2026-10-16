using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using ERP.DataClass;
using ERP.ComClass;

namespace ERP.BS
{
    public partial class FormBSEmployee : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormBSEmployee()
        {
            InitializeComponent();
        }

        private void ControlStatus()
        {
            //工具栏按钮状态切换
            this.toolSave.Enabled = !this.toolSave.Enabled;
            this.toolCancel.Enabled = !this.toolCancel.Enabled;
            commUse.CortrolButtonEnabled(toolAdd,this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            //窗体控件状态切换
            this.txtEmployeeCode.ReadOnly = !this.txtEmployeeCode.ReadOnly;
            this.txtEmployeeName.ReadOnly = !this.txtEmployeeName.ReadOnly;
            this.cbxDepartmentCode.Enabled = !this.cbxDepartmentCode.Enabled;
            this.txtAge.ReadOnly = !this.txtAge.ReadOnly;
            this.cbxSex.Enabled = !this.cbxSex.Enabled;
            this.cbxEduLevel.Enabled = !this.cbxEduLevel.Enabled;
            this.txtJob.ReadOnly = !this.txtJob.ReadOnly;
            this.dtpJoinDate.Enabled = !this.dtpJoinDate.Enabled;
            this.txtTelephoneCode.ReadOnly = !this.txtTelephoneCode.ReadOnly;
            this.rtbRemark.ReadOnly = !this.rtbRemark.ReadOnly;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {   
            this.txtEmployeeCode.Text = "";
            this.txtEmployeeName.Text = "";
            this.cbxDepartmentCode.SelectedIndex = -1;
            this.txtAge.Text = "";
            this.cbxSex.SelectedIndex = -1;
            this.cbxEduLevel.SelectedIndex = -1;
            this.txtJob.Text = "";
            this.dtpJoinDate.Value = Convert.ToDateTime("1900-01-01"); //DateTime.Today;
            this.txtTelephoneCode.Text = "";
            this.rtbRemark.Text = "";
        }

        private void BindToolStripComboBox()
        {
            this.cbxCondition.Items.Add("员工名称");
            this.cbxCondition.Items.Add("职位");
        }

        /// <summary>
        ///设置控件的显示值
        /// </summary>
        private void FillControls()
        {   
            this.txtEmployeeCode.Text = this.dgvEmployeeInfo[0, this.dgvEmployeeInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtEmployeeName.Text = this.dgvEmployeeInfo[1, this.dgvEmployeeInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxDepartmentCode.SelectedValue = this.dgvEmployeeInfo[2, this.dgvEmployeeInfo.CurrentCell.RowIndex].Value;
            this.txtAge.Text = this.dgvEmployeeInfo[3, this.dgvEmployeeInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxSex.SelectedValue = this.dgvEmployeeInfo[4, this.dgvEmployeeInfo.CurrentCell.RowIndex].Value;
            this.cbxEduLevel.SelectedValue = this.dgvEmployeeInfo[5, this.dgvEmployeeInfo.CurrentCell.RowIndex].Value;
            this.txtJob.Text = this.dgvEmployeeInfo[6, this.dgvEmployeeInfo.CurrentCell.RowIndex].Value.ToString();
            this.dtpJoinDate.Value = Convert.ToDateTime(this.dgvEmployeeInfo[7, this.dgvEmployeeInfo.CurrentCell.RowIndex].Value);
            this.txtTelephoneCode.Text = this.dgvEmployeeInfo[8, this.dgvEmployeeInfo.CurrentCell.RowIndex].Value.ToString();
            this.rtbRemark.Text = this.dgvEmployeeInfo[9, this.dgvEmployeeInfo.CurrentCell.RowIndex].Value.ToString();
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM BSEmployee" + strWhere; ;

            try
            {
                this.dgvEmployeeInfo.DataSource = db.GetDataSet(strSql, "BSEmployee").Tables["BSEmployee"];
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

            pars.Add(new SqlParameter("@EmployeeCode", txtEmployeeCode.Text.Trim()));
            pars.Add(new SqlParameter("@EmployeeName", txtEmployeeName.Text.Trim()));

            if (cbxDepartmentCode.SelectedValue == null)
                pars.Add(new SqlParameter("@DepartmentCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@DepartmentCode", cbxDepartmentCode.SelectedValue.ToString()));

            if (String.IsNullOrEmpty(txtAge.Text.Trim()))
                pars.Add(new SqlParameter("@Age", DBNull.Value));
            else
                pars.Add(new SqlParameter("@Age", Convert.ToInt32(txtAge.Text.Trim())));

            if (cbxSex.SelectedValue == null)
                pars.Add(new SqlParameter("@Sex", DBNull.Value));
            else
                pars.Add(new SqlParameter("@Sex", cbxSex.SelectedValue.ToString()));

            if (cbxEduLevel.SelectedValue == null)
                pars.Add(new SqlParameter("@EduLevel", DBNull.Value));
            else
                pars.Add(new SqlParameter("@EduLevel", cbxEduLevel.SelectedValue.ToString()));

            pars.Add(new SqlParameter("@Job", txtJob.Text.Trim()));
            pars.Add(new SqlParameter("@JoinDate", dtpJoinDate.Value));
            pars.Add(new SqlParameter("@TelephoneCode", txtTelephoneCode.Text.Trim()));
            pars.Add(new SqlParameter("@Remark", rtbRemark.Text.Trim()));

            return pars.ToArray();
        }

        private void FormEmployee_Load(object sender, EventArgs e)
        {
            //权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            //ComboBox绑定到数据源
            commUse.BindComboBox(cbxDepartmentCode, "DepartmentCode", "DepartmentName", "select DepartmentCode,DepartmentName from BSDepartment", "BSDepartment");
            commUse.BindComboBox(cbxSex, "Code", "Name", "select * from INSex", "INSex");
            commUse.BindComboBox(cbxEduLevel, "Code", "Name", "select * from INEduLevel", "INEduLevel");
            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvEmployeeInfo.Columns[2], "DepartmentCode", "DepartmentName", "select DepartmentCode,DepartmentName from BSDepartment", "BSDepartment");
            commUse.BindComboBox(this.dgvEmployeeInfo.Columns[4], "Code", "Name", "select * from INSex", "INSex");
            commUse.BindComboBox(this.dgvEmployeeInfo.Columns[5], "Code", "Name", "select * from INEduLevel", "INEduLevel");
            //DataGridView绑定到数据源
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
            txtEmployeeCode.Enabled = true;
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //表示是修改状态
            txtEmployeeCode.Enabled = false;
        }

        private void toolreflush_Click(object sender, EventArgs e)
        {
            this.BindDataGridView("");
        }

        private void dgvStoreInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                if (dgvEmployeeInfo.RowCount > 0)
                {
                    //判断当前记录的主键值是否存在外键约束
                    if (commUse.IsExistConstraint("BSEmployee", dgvEmployeeInfo[0, dgvEmployeeInfo.CurrentCell.RowIndex].Value.ToString()))
                    {
                        txtEmployeeCode.Enabled = false;
                    }
                    else
                    {
                        txtEmployeeCode.Enabled = true;
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

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtAge_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputInteger(e);
        }

        private void txtOK_Click(object sender, EventArgs e)
        {
            string strWhere = String.Empty;
            string strConditonName = String.Empty;

            strConditonName = this.cbxCondition.Items[this.cbxCondition.SelectedIndex].ToString();
            switch (strConditonName)
            {
                case "员工名称":

                    strWhere = " WHERE EmployeeName LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "职位":

                    strWhere = " WHERE Job LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                default:
                    break;
            }
        }

        private void toolDelete_Click(object sender, EventArgs e)
        {
            string strEmployeeCode = null;
            string strSql = null;

            if (this.dgvEmployeeInfo.RowCount == 0)
            {
                return;
            }

            strEmployeeCode = this.dgvEmployeeInfo[0, this.dgvEmployeeInfo.CurrentCell.RowIndex].Value.ToString();

            if (commUse.IsExistConstraint("BSEmployee", strEmployeeCode))
            {
                MessageBox.Show("已发生业务关系，无法删除", "软件提示");
                return;
            }

            strSql = "DELETE FROM BSEmployee WHERE EmployeeCode = @EmployeeCode";
            SqlParameter[] pars = { new SqlParameter("@EmployeeCode", strEmployeeCode) };

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

        private void toolSave_Click(object sender, EventArgs e)
        {
            string strCode = null; ;
            SqlDataReader sdr = null;

            if (String.IsNullOrEmpty(txtEmployeeCode.Text.Trim()))
            {
                MessageBox.Show("员工编号不许为空！", "软件提示");
                txtEmployeeCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtEmployeeName.Text.Trim()))
            {
                MessageBox.Show("员工名称不许为空！", "软件提示");
                txtEmployeeName.Focus();
                return;
            }

            if (cbxDepartmentCode.SelectedValue == null)
            {
                MessageBox.Show("所在部门不许为空！", "软件提示");
                cbxDepartmentCode.Focus();
                return;
            }

            //添加
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                strCode = "SELECT * FROM BSEmployee WHERE EmployeeCode = @EmployeeCode";
                SqlParameter[] checkPars = { new SqlParameter("@EmployeeCode", txtEmployeeCode.Text.Trim()) };

                try
                {
                    sdr = db.GetDataReader(strCode, checkPars);
                    sdr.Read();
                    if (!sdr.HasRows)
                    {
                        sdr.Close();

                        strCode = "INSERT INTO BSEmployee(EmployeeCode,EmployeeName,DepartmentCode,Age,Sex,EduLevel,Job,JoinDate,TelephoneCode,Remark) ";
                        strCode += "VALUES(@EmployeeCode,@EmployeeName,@DepartmentCode,@Age,@Sex,@EduLevel,@Job,@JoinDate,@TelephoneCode,@Remark)";

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
                        this.txtEmployeeCode.Focus();
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
                string strOldEmployeeCode = null;

                strOldEmployeeCode = this.dgvEmployeeInfo[0, this.dgvEmployeeInfo.CurrentCell.RowIndex].Value.ToString();

                //员工代码被修改过
                if (strOldEmployeeCode != txtEmployeeCode.Text.Trim())
                {
                    strCode = "SELECT * FROM BSEmployee WHERE EmployeeCode = @EmployeeCode";
                    SqlParameter[] checkPars = { new SqlParameter("@EmployeeCode", txtEmployeeCode.Text.Trim()) };

                    try
                    {
                        sdr = db.GetDataReader(strCode, checkPars);
                        sdr.Read();
                        if (sdr.HasRows)
                        {
                            MessageBox.Show("编码重复，请重新设置", "软件提示");
                            this.txtEmployeeCode.Focus();
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
                    strCode = "UPDATE BSEmployee SET EmployeeCode = @EmployeeCode,EmployeeName = @EmployeeName,";
                    strCode += "DepartmentCode = @DepartmentCode,Age = @Age,Sex = @Sex,";
                    strCode += "EduLevel = @EduLevel,Job = @Job,JoinDate = @JoinDate,TelephoneCode = @TelephoneCode,Remark = @Remark ";
                    strCode += "WHERE EmployeeCode = @OldEmployeeCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldEmployeeCode", strOldEmployeeCode));

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

        private void dgvEmployeeInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

    }
}
