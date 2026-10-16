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

namespace ERP.SE
{
    public partial class FormSEOrder : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormSEOrder()
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
            this.cbxCustomerCode.Enabled = !this.cbxCustomerCode.Enabled;
            this.cbxStoreCode.Enabled = !this.cbxStoreCode.Enabled;
            this.cbxInvenCode.Enabled = !this.cbxInvenCode.Enabled;
            this.txtSellPrice.ReadOnly = !this.txtSellPrice.ReadOnly;
            this.txtQuantity.ReadOnly = !this.txtQuantity.ReadOnly;
            this.dtpSenInvenDate.Enabled = !this.dtpSenInvenDate.Enabled;
            this.cbxEmployeeCode.Enabled = !this.cbxEmployeeCode.Enabled;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            this.txtSEOrderCode.Text = "";
            this.dtpSEOrderDate.Value = Convert.ToDateTime("1900-01-01");
            this.cbxOperatorCode.SelectedIndex = -1;
            this.cbxCustomerCode.SelectedIndex = -1;
            this.cbxStoreCode.SelectedIndex = -1;
            this.cbxInvenCode.SelectedIndex = -1;
            this.txtSellPrice.Text = "";
            this.txtQuantity.Text = "";
            this.txtSEMoney.Text = "";
            this.dtpSenInvenDate.Value = Convert.ToDateTime("1900-01-01");
            this.cbxEmployeeCode.SelectedIndex = -1;
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
            this.txtSEOrderCode.Text = this.dgvSEOrderInfo[0, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();
            this.dtpSEOrderDate.Value = Convert.ToDateTime(this.dgvSEOrderInfo[1, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value);
            this.cbxOperatorCode.SelectedValue = this.dgvSEOrderInfo[2, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value;
            this.cbxCustomerCode.SelectedValue = this.dgvSEOrderInfo[3, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value;
            this.cbxStoreCode.SelectedValue = this.dgvSEOrderInfo[4, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value;
            this.cbxInvenCode.SelectedValue = this.dgvSEOrderInfo[5, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value;
            this.txtSellPrice.Text = this.dgvSEOrderInfo[6, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtQuantity.Text = this.dgvSEOrderInfo[7, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtSEMoney.Text = this.dgvSEOrderInfo[8, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();
            this.dtpSenInvenDate.Value = Convert.ToDateTime(this.dgvSEOrderInfo[9, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value);
            this.cbxEmployeeCode.SelectedValue = this.dgvSEOrderInfo[10, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value;
            this.cbxIsFlag.SelectedValue = this.dgvSEOrderInfo[11, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value;
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM SEOrder " + strWhere; ;

            try
            {
                this.dgvSEOrderInfo.DataSource = db.GetDataSet(strSql, "SEOrder").Tables["SEOrder"];
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

            pars.Add(new SqlParameter("@SEOrderCode", txtSEOrderCode.Text.Trim()));
            pars.Add(new SqlParameter("@SEOrderDate", dtpSEOrderDate.Value));

            if (cbxOperatorCode.SelectedValue == null)
                pars.Add(new SqlParameter("@OperatorCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@OperatorCode", cbxOperatorCode.SelectedValue.ToString()));

            if (cbxCustomerCode.SelectedValue == null)
                pars.Add(new SqlParameter("@CustomerCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@CustomerCode", cbxCustomerCode.SelectedValue.ToString()));

            if (cbxStoreCode.SelectedValue == null)
                pars.Add(new SqlParameter("@StoreCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@StoreCode", cbxStoreCode.SelectedValue.ToString()));

            if (cbxInvenCode.SelectedValue == null)
                pars.Add(new SqlParameter("@InvenCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@InvenCode", cbxInvenCode.SelectedValue.ToString()));

            if (String.IsNullOrEmpty(txtSellPrice.Text.Trim()))
                pars.Add(new SqlParameter("@SellPrice", 0));
            else
                pars.Add(new SqlParameter("@SellPrice", Convert.ToDecimal(txtSellPrice.Text.Trim())));

            if (String.IsNullOrEmpty(txtQuantity.Text.Trim()))
                pars.Add(new SqlParameter("@Quantity", 0));
            else
                pars.Add(new SqlParameter("@Quantity", Convert.ToInt32(txtQuantity.Text.Trim())));

            if (String.IsNullOrEmpty(txtSEMoney.Text.Trim()))
                pars.Add(new SqlParameter("@SEMoney", 0));
            else
                pars.Add(new SqlParameter("@SEMoney", Convert.ToDecimal(txtSEMoney.Text.Trim())));

            pars.Add(new SqlParameter("@SenInvenDate", dtpSenInvenDate.Value));

            if (cbxEmployeeCode.SelectedValue == null)
                pars.Add(new SqlParameter("@EmployeeCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@EmployeeCode", cbxEmployeeCode.SelectedValue.ToString()));

            if (cbxIsFlag.SelectedValue == null)
                pars.Add(new SqlParameter("@IsFlag", DBNull.Value));
            else
                pars.Add(new SqlParameter("@IsFlag", cbxIsFlag.SelectedValue.ToString()));

            return pars.ToArray();
        }

        /// <summary>
        /// 计算销售金额
        /// </summary>
        private void ComputeMoney()
        {
            int int_Quantity;
            decimal dec_SellPrice;

            if (!String.IsNullOrEmpty(txtQuantity.Text.Trim()) && !String.IsNullOrEmpty(txtSellPrice.Text.Trim()))
            {
                int_Quantity = Convert.ToInt32(txtQuantity.Text.Trim());
                dec_SellPrice = Convert.ToDecimal(txtSellPrice.Text.Trim());
                txtSEMoney.Text = Decimal.Round(int_Quantity * dec_SellPrice, 2).ToString();
            }
        }

        private void FormSEOrder_Load(object sender, EventArgs e)
        {
            //权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            commUse.CortrolButtonEnabled(toolCheck, this);
            commUse.CortrolButtonEnabled(toolUnCheck, this);

            //ComboBox绑定到数据源
            commUse.BindComboBox(cbxOperatorCode, "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(cbxCustomerCode, "CustomerCode", "CustomerName", "select CustomerCode,CustomerName from BSCustomer", "BSCustomer");
            commUse.BindComboBox(cbxStoreCode, "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(cbxInvenCode, "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(cbxEmployeeCode, "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(cbxIsFlag, "Code", "Name", "select * from INCheckFlag", "INCheckFlag");

            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvSEOrderInfo.Columns[2], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvSEOrderInfo.Columns[3], "CustomerCode", "CustomerName", "select CustomerCode,CustomerName from BSCustomer", "BSCustomer");
            commUse.BindComboBox(this.dgvSEOrderInfo.Columns[4], "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(this.dgvSEOrderInfo.Columns[5], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvSEOrderInfo.Columns[10], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvSEOrderInfo.Columns[11], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");

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

            dtpSEOrderDate.Value = DateTime.Today;
            cbxOperatorCode.SelectedValue = PropertyClass.OperatorCode;
            txtQuantity.Text = "1";
            dtpSenInvenDate.Value = DateTime.Today;
            cbxIsFlag.SelectedValue = "0";
            txtSEOrderCode.Text = commUse.BuildBillCode("SEOrder", "SEOrderCode", "SEOrderDate", dtpSEOrderDate.Value);
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //修改标识
        }

        private void dgvSEOrderInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT" && dgvSEOrderInfo.RowCount > 0)
            {
                if (this.dgvSEOrderInfo[11, this.dgvSEOrderInfo.CurrentRow.Index].Value.ToString() == "1")
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

        private void txtSellPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputNumeric(e, sender as Control);
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputInteger(e);
        }

        private void txtSellPrice_TextChanged(object sender, EventArgs e)
        {
            ComputeMoney();
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            string strCode = null;

            // ===== 1. 数据校验 =====
            if (String.IsNullOrEmpty(txtSEOrderCode.Text.Trim()))
            {
                MessageBox.Show("单据编号不许为空！", "软件提示");
                txtSEOrderCode.Focus();
                return;
            }

            if (cbxCustomerCode.SelectedValue == null)
            {
                MessageBox.Show("客户不许为空！", "软件提示");
                cbxCustomerCode.Focus();
                return;
            }

            if (cbxInvenCode.SelectedValue == null)
            {
                MessageBox.Show("存货不许为空！", "软件提示");
                cbxInvenCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtSellPrice.Text.Trim()))
            {
                MessageBox.Show("单价不许为空！", "软件提示");
                txtSellPrice.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                MessageBox.Show("数量不许为空！", "软件提示");
                txtQuantity.Focus();
                return;
            }
            else if (Convert.ToInt32(txtQuantity.Text.Trim()) == 0)
            {
                MessageBox.Show("数量不能等于零", "软件提示");
                txtQuantity.Focus();
                return;
            }

            // ===== 2. 检查库存（参数化） =====
            string stockCheckSql = "SELECT (Quantity - LossQuantity) FROM STStock WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
            SqlParameter[] stockCheckPars = {
        new SqlParameter("@StoreCode", cbxStoreCode.SelectedValue.ToString()),
        new SqlParameter("@InvenCode", cbxInvenCode.SelectedValue.ToString())
    };

            DataSet ds = db.GetDataSet(stockCheckSql, "STStock", stockCheckPars);

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToInt32(txtQuantity.Text.Trim()) > Convert.ToInt32(ds.Tables[0].Rows[0][0]))
                {
                    MessageBox.Show("销售数量不能大于库存数量", "软件提示");
                    txtQuantity.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show("当前仓库中没有该商品，请重新选择", "软件提示");
                txtQuantity.Focus();
                return;
            }

            // ===== 3. 添加操作 =====
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                try
                {
                    List<(string, SqlParameter[])> commands = new List<(string, SqlParameter[])>();

                    // 3.1 插入订单
                    strCode = "INSERT INTO SEOrder(SEOrderCode,SEOrderDate,OperatorCode,CustomerCode,StoreCode,InvenCode,SellPrice,Quantity,SEMoney,SenInvenDate,EmployeeCode,IsFlag) ";
                    strCode += "VALUES(@SEOrderCode,@SEOrderDate,@OperatorCode,@CustomerCode,@StoreCode,@InvenCode,@SellPrice,@Quantity,@SEMoney,@SenInvenDate,@EmployeeCode,@IsFlag)";
                    commands.Add((strCode, ParametersAddValue()));

                    // 3.2 更新库存（扣减）
                    string updateStockSql = "UPDATE STStock SET Quantity = Quantity - @Quantity WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
                    SqlParameter[] updateStockPars = {
                new SqlParameter("@Quantity", Convert.ToInt32(txtQuantity.Text.Trim())),
                new SqlParameter("@StoreCode", cbxStoreCode.SelectedValue.ToString()),
                new SqlParameter("@InvenCode", cbxInvenCode.SelectedValue.ToString())
            };
                    commands.Add((updateStockSql, updateStockPars));

                    // 3.3 事务提交
                    if (db.ExecDataBySqls(commands))
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

            // ===== 4. 修改操作 =====
            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                string strSEOrderCode = txtSEOrderCode.Text.Trim();

                try
                {
                    List<(string, SqlParameter[])> commands = new List<(string, SqlParameter[])>();

                    // 4.1 更新订单
                    strCode = "UPDATE SEOrder SET SEOrderCode = @SEOrderCode,SEOrderDate = @SEOrderDate,OperatorCode = @OperatorCode, CustomerCode = @CustomerCode,StoreCode = @StoreCode,";
                    strCode += "InvenCode = @InvenCode,SellPrice = @SellPrice,Quantity = @Quantity,";
                    strCode += "SEMoney = @SEMoney,SenInvenDate = @SenInvenDate,EmployeeCode = @EmployeeCode,IsFlag = @IsFlag";
                    strCode += " WHERE SEOrderCode = @OldSEOrderCode";

                    List<SqlParameter> mainPars = new List<SqlParameter>(ParametersAddValue());
                    mainPars.Add(new SqlParameter("@OldSEOrderCode", strSEOrderCode));
                    commands.Add((strCode, mainPars.ToArray()));

                    // 4.2 更新库存（扣减）
                    string updateStockSql = "UPDATE STStock SET Quantity = Quantity - @Quantity WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
                    SqlParameter[] updateStockPars = {
                new SqlParameter("@Quantity", Convert.ToInt32(txtQuantity.Text.Trim())),
                new SqlParameter("@StoreCode", cbxStoreCode.SelectedValue.ToString()),
                new SqlParameter("@InvenCode", cbxInvenCode.SelectedValue.ToString())
            };
                    commands.Add((updateStockSql, updateStockPars));

                    // 4.3 事务提交
                    if (db.ExecDataBySqls(commands))
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
            string strSEOrderCode = null; //单据编号
            string strSql = null;
            string strFlag = null; //审核标记

            if (this.dgvSEOrderInfo.RowCount <= 0)
            {
                return;
            }

            strSEOrderCode = this.dgvSEOrderInfo[0, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();
            strFlag = this.dgvSEOrderInfo[11, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();

            if (strFlag == "1")
            {
                MessageBox.Show("该单据已审核，不许删除！", "软件提示");
                return;
            }

            strSql = "DELETE FROM SEOrder WHERE SEOrderCode = @SEOrderCode";
            SqlParameter[] pars = { new SqlParameter("@SEOrderCode", strSEOrderCode) };

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
            string strSEOrderCode = null; //单据编号
            string strSql = null;
            string strFlag = null; //审核标记

            if (dgvSEOrderInfo.RowCount == 0)
            {
                return;
            }

            strSEOrderCode = this.dgvSEOrderInfo[0, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();
            strFlag = this.dgvSEOrderInfo[11, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();

            if (strFlag == "1")
            {
                MessageBox.Show("该单据已审核过，不许再次审核！", "软件提示");
                return;
            }

            strSql = "UPDATE SEOrder SET IsFlag = '1' WHERE SEOrderCode = @SEOrderCode";
            SqlParameter[] pars = { new SqlParameter("@SEOrderCode", strSEOrderCode) };

            try
            {
                if (db.ExecDataBySql(strSql, pars) > 0)
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
            string strSEOrderCode = null;   // 单据编号
            string strSql = null;
            string strFlag = null;          // 审核标记
            SqlDataReader sdr = null;

            if (dgvSEOrderInfo.RowCount == 0)
            {
                return;
            }

            strSEOrderCode = this.dgvSEOrderInfo[0, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();
            strFlag = this.dgvSEOrderInfo[11, this.dgvSEOrderInfo.CurrentCell.RowIndex].Value.ToString();

            // ✅ 检查是否已发生业务关系（参数化）
            strSql = "SELECT * FROM SEOutStore WHERE SEOrderCode = @SEOrderCode";
            SqlParameter[] checkPars = { new SqlParameter("@SEOrderCode", strSEOrderCode) };

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
            strSql = "UPDATE SEOrder SET IsFlag = '0' WHERE SEOrderCode = @SEOrderCode";
            SqlParameter[] pars = { new SqlParameter("@SEOrderCode", strSEOrderCode) };

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

                    strWhere = " WHERE SEOrderCode LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "单据日期":

                    strWhere = " WHERE SUBSTRING(CONVERT(VARCHAR(20),SEOrderDate,20),1,10) LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                default:
                    break;
            }
        }

        private void dgvSEOrderInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
