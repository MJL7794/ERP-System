using ERP.ComClass;
using ERP.DataClass;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERP.CU
{
    public partial class FormCUSellChance : Form
    {
        FormCustomerCourse formCustomerCourse = null;
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormCUSellChance()
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

            if (String.IsNullOrEmpty(txtTheme.Text.Trim()))
                pars.Add(new SqlParameter("@Theme", DBNull.Value));
            else
                pars.Add(new SqlParameter("@Theme", txtTheme.Text.Trim()));

            pars.Add(new SqlParameter("@RegDate", dtpRegDate.Value));

            if (cbxChanceCode.SelectedValue == null)
                pars.Add(new SqlParameter("@ChanceCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@ChanceCode", cbxChanceCode.SelectedValue.ToString()));

            pars.Add(new SqlParameter("@ForeDate", dtpForeDate.Value));

            if (cbxInvenCode.SelectedValue == null)
                pars.Add(new SqlParameter("@InvenCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@InvenCode", cbxInvenCode.SelectedValue.ToString()));

            if (String.IsNullOrEmpty(txtUnitPrice.Text.Trim()))
                pars.Add(new SqlParameter("@UnitPrice", DBNull.Value));
            else
                pars.Add(new SqlParameter("@UnitPrice", Convert.ToDecimal(txtUnitPrice.Text.Trim())));

            if (String.IsNullOrEmpty(txtQuantity.Text.Trim()))
                pars.Add(new SqlParameter("@Quantity", DBNull.Value));
            else
                pars.Add(new SqlParameter("@Quantity", Convert.ToInt32(txtQuantity.Text.Trim())));

            if (String.IsNullOrEmpty(txtCUMoney.Text.Trim()))
                pars.Add(new SqlParameter("@CUMoney", DBNull.Value));
            else
                pars.Add(new SqlParameter("@CUMoney", Convert.ToDecimal(txtCUMoney.Text.Trim())));

            if (String.IsNullOrEmpty(rtbRemark.Text.Trim()))
                pars.Add(new SqlParameter("@Remark", DBNull.Value));
            else
                pars.Add(new SqlParameter("@Remark", rtbRemark.Text.Trim()));

            return pars.ToArray();
        }

        /// <summary>
        /// 计算预计金额
        /// </summary>
        private void ComputeMoney()
        {
            int int_Quantity;
            decimal dec_UnitPrice;

            if (!String.IsNullOrEmpty(txtQuantity.Text.Trim()) && !String.IsNullOrEmpty(txtUnitPrice.Text.Trim()))
            {
                int_Quantity = Convert.ToInt32(txtQuantity.Text.Trim());
                dec_UnitPrice = Convert.ToDecimal(txtUnitPrice.Text.Trim());
                txtCUMoney.Text = Decimal.Round(int_Quantity * dec_UnitPrice, 2).ToString();
            }
        }

        private void FormCUSellChance_Load(object sender, EventArgs e)
        {
            formCustomerCourse = (FormCustomerCourse)this.Owner;
            commUse.BindComboBox(cbxCustomerCode, "CustomerCode", "CustomerName", "select CustomerCode,CustomerName from BSCustomer", "BSCustomer");
            commUse.BindComboBox(cbxChanceCode, "ChanceCode", "ChanceName", "select ChanceCode,ChanceName from CUChance", "CUChance");
            commUse.BindComboBox(cbxInvenCode, "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            cbxCustomerCode.SelectedValue = formCustomerCourse.tvCustomer.SelectedNode.Tag;

            if (this.Tag.ToString() == "Add") //添加操作
            {
                cbxChanceCode.SelectedIndex = -1;
                cbxInvenCode.SelectedIndex = -1;
                dtpRegDate.Value = DateTime.Today;
                dtpForeDate.Value = DateTime.Today;
            }

            if (this.Tag.ToString() == "Edit") //编辑操作
            {
                txtTheme.Text = formCustomerCourse.dgvSell["Theme", formCustomerCourse.dgvSell.CurrentRow.Index].Value.ToString();
                dtpRegDate.Value = Convert.ToDateTime(formCustomerCourse.dgvSell["RegDate", formCustomerCourse.dgvSell.CurrentRow.Index].Value);
                cbxChanceCode.SelectedValue = formCustomerCourse.dgvSell["ChanceCode", formCustomerCourse.dgvSell.CurrentRow.Index].Value;
                dtpForeDate.Value = Convert.ToDateTime(formCustomerCourse.dgvSell["ForeDate", formCustomerCourse.dgvSell.CurrentRow.Index].Value);
                cbxInvenCode.SelectedValue = formCustomerCourse.dgvSell["InvenCode", formCustomerCourse.dgvSell.CurrentRow.Index].Value;
                txtUnitPrice.Text = formCustomerCourse.dgvSell["UnitPrice", formCustomerCourse.dgvSell.CurrentRow.Index].Value.ToString();
                txtQuantity.Text = formCustomerCourse.dgvSell["Quantity", formCustomerCourse.dgvSell.CurrentRow.Index].Value.ToString();
                txtCUMoney.Text = formCustomerCourse.dgvSell["CUMoney", formCustomerCourse.dgvSell.CurrentRow.Index].Value.ToString();
                rtbRemark.Text = formCustomerCourse.dgvSell["Remark", formCustomerCourse.dgvSell.CurrentRow.Index].Value.ToString();
            }
        }

        private void txtUnitPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputNumeric(e, sender as Control);
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputInteger(e);
        }

        private void txtUnitPrice_TextChanged(object sender, EventArgs e)
        {
            ComputeMoney();
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            ComputeMoney();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int intSellId;
            string strCode = null;

            if (String.IsNullOrEmpty(txtTheme.Text.Trim()))
            {
                MessageBox.Show("标题不许为空！","软件提示");
                txtTheme.Focus();
                return;
            }

            if (cbxChanceCode.SelectedValue == null)
            {
                MessageBox.Show("机会等级不许为空！", "软件提示");
                cbxChanceCode.Focus();
                return;
            }

            if (cbxInvenCode.SelectedIndex == -1)
            {
                MessageBox.Show("预售产品不许为空！", "软件提示");
                cbxInvenCode.Focus();
                return;
            }

            if (this.Tag.ToString() == "Add")
            {
                SqlParameter[] pars = ParametersAddValue();

                strCode = "INSERT INTO CUSellChance(CustomerCode,Theme,RegDate,ChanceCode,ForeDate,InvenCode,UnitPrice,Quantity,CUMoney,Remark) ";
                strCode += "VALUES(@CustomerCode,@Theme,@RegDate,@ChanceCode,@ForeDate,@InvenCode,@UnitPrice,@Quantity,@CUMoney,@Remark)";

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
                intSellId = Convert.ToInt32(formCustomerCourse.dgvSell["SellId", formCustomerCourse.dgvSell.CurrentRow.Index].Value);

                SqlParameter[] pars = ParametersAddValue();

                strCode = "UPDATE CUSellChance SET CustomerCode=@CustomerCode,Theme=@Theme,RegDate = @RegDate,ChanceCode = @ChanceCode,ForeDate = @ForeDate,InvenCode = @InvenCode,UnitPrice = @UnitPrice,Quantity = @Quantity,CUMoney = @CUMoney,Remark= @Remark ";
                strCode += "WHERE SellId = @SellId";

                // 加上 SellId 参数
                List<SqlParameter> parsList = new List<SqlParameter>(pars);
                parsList.Add(new SqlParameter("@SellId", intSellId));

                if (db.ExecDataBySql(strCode, parsList.ToArray()) > 0)
                {
                    MessageBox.Show("保存成功！", "软件提示");
                }
                else
                {
                    MessageBox.Show("保存失败！", "软件提示");
                }
            }

            formCustomerCourse.BindDataGridView(formCustomerCourse.tvCustomer.SelectedNode.Tag.ToString(), "CUSellChance", formCustomerCourse.dgvSell);
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
