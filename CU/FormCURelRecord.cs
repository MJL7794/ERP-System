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
namespace ERP.CU
{
    public partial class FormCURelRecord : Form
    {
        FormCustomerCourse formCustomerCourse = null;
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormCURelRecord()
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

            pars.Add(new SqlParameter("@RelDate", dtpRelDate.Value));

            if (cbxRelManner.SelectedValue == null)
                pars.Add(new SqlParameter("@RelManner", DBNull.Value));
            else
                pars.Add(new SqlParameter("@RelManner", cbxRelManner.SelectedValue.ToString()));

            if (String.IsNullOrEmpty(txtLinkman.Text.Trim()))
                pars.Add(new SqlParameter("@Linkman", DBNull.Value));
            else
                pars.Add(new SqlParameter("@Linkman", txtLinkman.Text.Trim()));

            if (String.IsNullOrEmpty(txtTelephoneCode.Text.Trim()))
                pars.Add(new SqlParameter("@TelephoneCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@TelephoneCode", txtTelephoneCode.Text.Trim()));

            pars.Add(new SqlParameter("@NextDate", dtpNextDate.Value));

            if (String.IsNullOrEmpty(rtbRelContent.Text.Trim()))
                pars.Add(new SqlParameter("@RelContent", DBNull.Value));
            else
                pars.Add(new SqlParameter("@RelContent", rtbRelContent.Text.Trim()));

            if (String.IsNullOrEmpty(rtbFeeInfo.Text.Trim()))
                pars.Add(new SqlParameter("@FeeInfo", DBNull.Value));
            else
                pars.Add(new SqlParameter("@FeeInfo", rtbFeeInfo.Text.Trim()));

            return pars.ToArray();
        }

        private void FormCURelRecord_Load(object sender, EventArgs e)
        {
            formCustomerCourse = (FormCustomerCourse)this.Owner;
            commUse.BindComboBox(cbxCustomerCode, "CustomerCode", "CustomerName", "select CustomerCode,CustomerName from BSCustomer", "BSCustomer");
            commUse.BindComboBox(cbxRelManner, "Code", "Name", "select Code,Name from INRelManner", "INRelManner");
            cbxCustomerCode.SelectedValue = formCustomerCourse.tvCustomer.SelectedNode.Tag;

            //添加操作
            if (this.Tag.ToString() == "Add") 
            {
                cbxRelManner.SelectedIndex = -1;
                dtpRelDate.Value = DateTime.Today;
                dtpNextDate.Value = DateTime.Today;
            }

            //修改操作
            if (this.Tag.ToString() == "Edit")
            {   
                dtpRelDate.Value = Convert.ToDateTime(formCustomerCourse.dgvRel["RelDate", formCustomerCourse.dgvRel.CurrentRow.Index].Value);
                cbxRelManner.SelectedValue = formCustomerCourse.dgvRel["RelManner", formCustomerCourse.dgvRel.CurrentRow.Index].Value;
                txtLinkman.Text = formCustomerCourse.dgvRel["Linkman_Rel", formCustomerCourse.dgvRel.CurrentRow.Index].Value.ToString();
                txtTelephoneCode.Text = formCustomerCourse.dgvRel["TelephoneCode_Rel", formCustomerCourse.dgvRel.CurrentRow.Index].Value.ToString();
                dtpNextDate.Value = Convert.ToDateTime(formCustomerCourse.dgvRel["NextDate", formCustomerCourse.dgvRel.CurrentRow.Index].Value);
                rtbRelContent.Text = formCustomerCourse.dgvRel["RelContent", formCustomerCourse.dgvRel.CurrentRow.Index].Value.ToString();
                rtbFeeInfo.Text = formCustomerCourse.dgvRel["FeeInfo", formCustomerCourse.dgvRel.CurrentRow.Index].Value.ToString();    
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int intRelId;
            string strCode = null;

            if ( cbxRelManner.SelectedValue == null)
            {
                MessageBox.Show("联系方式不许为空！", "软件提示");
                cbxRelManner.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtLinkman.Text.Trim()))
            {
                MessageBox.Show("联系人不许为空！", "软件提示");
                txtLinkman.Focus();
                return;
            }

            if (String.IsNullOrEmpty(rtbRelContent.Text.Trim()))
            {
                MessageBox.Show("联系内容不许为空！", "软件提示");
                rtbRelContent.Focus();
                return;
            }

            if (this.Tag.ToString() == "Add")
            {
                SqlParameter[] pars = ParametersAddValue();

                strCode = "INSERT INTO CURelRecord(CustomerCode,RelDate,RelManner,Linkman,TelephoneCode,NextDate,RelContent,FeeInfo) ";
                strCode += "VALUES(@CustomerCode,@RelDate,@RelManner,@Linkman,@TelephoneCode,@NextDate,@RelContent,@FeeInfo)";

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
                intRelId = Convert.ToInt32(formCustomerCourse.dgvRel["RelId", formCustomerCourse.dgvRel.CurrentRow.Index].Value);

                SqlParameter[] pars = ParametersAddValue();

                strCode = "UPDATE CURelRecord SET CustomerCode=@CustomerCode,RelDate=@RelDate,RelManner = @RelManner,Linkman = @Linkman,TelephoneCode = @TelephoneCode,NextDate = @NextDate,RelContent = @RelContent,FeeInfo = @FeeInfo ";
                strCode += "WHERE RelId = @RelId";

                // 加上 RelId 参数
                List<SqlParameter> parsList = new List<SqlParameter>(pars);
                parsList.Add(new SqlParameter("@RelId", intRelId));

                if (db.ExecDataBySql(strCode, parsList.ToArray()) > 0)
                {
                    MessageBox.Show("保存成功！", "软件提示");
                }
                else
                {
                    MessageBox.Show("保存失败！", "软件提示");
                }
            }

            formCustomerCourse.BindDataGridView(formCustomerCourse.tvCustomer.SelectedNode.Tag.ToString(), "CURelRecord", formCustomerCourse.dgvRel);
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
