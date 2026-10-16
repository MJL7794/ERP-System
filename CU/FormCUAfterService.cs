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
namespace ERP.CU
{
    public partial class FormCUAfterService : Form
    {
        FormCustomerCourse formCustomerCourse = null;
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormCUAfterService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        private SqlParameter[] ParametersAddValue()
        {
            List<SqlParameter> pars = new List<SqlParameter>();

            if (cbxCustomerCode.SelectedValue == null)
                pars.Add(new SqlParameter("@CustomerCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@CustomerCode", cbxCustomerCode.SelectedValue.ToString()));

            pars.Add(new SqlParameter("@SerDate", dtpSerDate.Value));

            if (cbxEmployeeCode.SelectedValue == null)
                pars.Add(new SqlParameter("@EmployeeCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@EmployeeCode", cbxEmployeeCode.SelectedValue.ToString()));

            if (String.IsNullOrEmpty(txtLinkman.Text.Trim()))
                pars.Add(new SqlParameter("@Linkman", DBNull.Value));
            else
                pars.Add(new SqlParameter("@Linkman", txtLinkman.Text.Trim()));

            if (String.IsNullOrEmpty(txtTelephoneCode.Text.Trim()))
                pars.Add(new SqlParameter("@TelephoneCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@TelephoneCode", txtTelephoneCode.Text.Trim()));

            if (String.IsNullOrEmpty(txtSerDays.Text.Trim()))
                pars.Add(new SqlParameter("@SerDays", DBNull.Value));
            else
                pars.Add(new SqlParameter("@SerDays", Convert.ToInt32(txtSerDays.Text.Trim())));

            if (String.IsNullOrEmpty(rtbSerContent.Text.Trim()))
                pars.Add(new SqlParameter("@SerContent", DBNull.Value));
            else
                pars.Add(new SqlParameter("@SerContent", rtbSerContent.Text.Trim()));

            if (String.IsNullOrEmpty(rtbResolvent.Text.Trim()))
                pars.Add(new SqlParameter("@Resolvent", DBNull.Value));
            else
                pars.Add(new SqlParameter("@Resolvent", rtbResolvent.Text.Trim()));

            return pars.ToArray();
        }

        private void FormCUAfterService_Load(object sender, EventArgs e)
        {
            formCustomerCourse = (FormCustomerCourse)this.Owner;
            commUse.BindComboBox(cbxCustomerCode, "CustomerCode", "CustomerName", "select CustomerCode,CustomerName from BSCustomer", "BSCustomer");
            commUse.BindComboBox(cbxEmployeeCode, "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            cbxCustomerCode.SelectedValue = formCustomerCourse.tvCustomer.SelectedNode.Tag;

            if (this.Tag.ToString() == "Add") //添加操作
            {
                cbxEmployeeCode.SelectedIndex = -1;
                dtpSerDate.Value = DateTime.Today;
            }

            if (this.Tag.ToString() == "Edit") //修改操作
            {
                dtpSerDate.Value = Convert.ToDateTime(formCustomerCourse.dgvAfter["SerDate", formCustomerCourse.dgvAfter.CurrentRow.Index].Value);
                cbxEmployeeCode.SelectedValue = formCustomerCourse.dgvAfter["EmployeeCode", formCustomerCourse.dgvAfter.CurrentRow.Index].Value;
                txtLinkman.Text = formCustomerCourse.dgvAfter["Linkman_After", formCustomerCourse.dgvAfter.CurrentRow.Index].Value.ToString();
                txtTelephoneCode.Text = formCustomerCourse.dgvAfter["TelephoneCode_After", formCustomerCourse.dgvAfter.CurrentRow.Index].Value.ToString();
                txtSerDays.Text = formCustomerCourse.dgvAfter["SerDays", formCustomerCourse.dgvAfter.CurrentRow.Index].Value.ToString();
                rtbSerContent.Text = formCustomerCourse.dgvAfter["SerContent", formCustomerCourse.dgvAfter.CurrentRow.Index].Value.ToString();
                rtbResolvent.Text = formCustomerCourse.dgvAfter["Resolvent", formCustomerCourse.dgvAfter.CurrentRow.Index].Value.ToString();
            }
        }

        private void txtSerDays_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputInteger(e);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int intAfterId;
            string strCode = null;

            if (cbxEmployeeCode.SelectedValue == null)
            {
                MessageBox.Show("服务人员不许为空！", "软件提示");
                cbxEmployeeCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtSerDays.Text.Trim()))
            {
                MessageBox.Show("服务天数不许为空！", "软件提示");
                txtSerDays.Focus();
                return;
            }
            else
            {
                if (Convert.ToInt32(txtSerDays.Text.Trim()) == 0)
                {
                    MessageBox.Show("服务天数不许为零！", "软件提示");
                    txtSerDays.Focus();
                    return;
                }
            }

            if (String.IsNullOrEmpty(rtbSerContent.Text.Trim()))
            {
                MessageBox.Show("服务内容不许为空！", "软件提示");
                rtbSerContent.Focus();
                return;
            }

            if (String.IsNullOrEmpty(rtbResolvent.Text.Trim()))
            {
                MessageBox.Show("解决方案不许为空！", "软件提示");
                rtbResolvent.Focus();
                return;
            }

            if (this.Tag.ToString() == "Add")
            {
                //添加参数
                SqlParameter[] pars = ParametersAddValue();

                strCode = "INSERT INTO CUAfterService(CustomerCode,SerDate,EmployeeCode,Linkman,TelephoneCode,SerDays,SerContent,Resolvent) ";
                strCode += "VALUES(@CustomerCode,@SerDate,@EmployeeCode,@Linkman,@TelephoneCode,@SerDays,@SerContent,@Resolvent)";

                if (db.ExecDataBySql(strCode, pars) > 0)
                {
                    MessageBox.Show("保存成功！", "软件提示");
                }
                else
                {
                    MessageBox.Show("保存失败！", "软件提示");
                }
            }

            if (this.Tag.ToString() == "Edit")
            {
                //取出主键
                intAfterId = Convert.ToInt32(formCustomerCourse.dgvAfter["AfterId", formCustomerCourse.dgvAfter.CurrentRow.Index].Value);

                //添加参数
                List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                parsList.Add(new SqlParameter("@AfterId", intAfterId));

                strCode = "UPDATE CUAfterService SET CustomerCode=@CustomerCode,SerDate=@SerDate,EmployeeCode = @EmployeeCode,Linkman = @Linkman,TelephoneCode = @TelephoneCode,SerDays = @SerDays,SerContent = @SerContent,Resolvent = @Resolvent ";
                strCode += "WHERE AfterId = @AfterId";

                if (db.ExecDataBySql(strCode, parsList.ToArray()) > 0)
                {
                    MessageBox.Show("保存成功！", "软件提示");
                }
                else
                {
                    MessageBox.Show("保存失败！", "软件提示");
                }
            }

            //重新绑定数据
            formCustomerCourse.BindDataGridView(formCustomerCourse.tvCustomer.SelectedNode.Tag.ToString(), "CUAfterService", formCustomerCourse.dgvAfter);
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
