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

namespace ERP.PU
{
    public partial class FormPUInStore : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormPUInStore()
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
            //this.txtPUOrderCode.ReadOnly = !this.txtPUOrderCode.ReadOnly;
            this.cbxSupplierCode.Enabled = !this.cbxSupplierCode.Enabled;
            this.cbxStoreCode.Enabled = !this.cbxStoreCode.Enabled;
            this.cbxInvenCode.Enabled = !this.cbxInvenCode.Enabled;
            this.txtUnitPrice.ReadOnly = !this.txtUnitPrice.ReadOnly;
            this.txtQuantity.ReadOnly = !this.txtQuantity.ReadOnly;
            this.btnChoice.Enabled = !this.btnChoice.Enabled;
            this.cbxEmployeeCode.Enabled = !this.cbxEmployeeCode.Enabled;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            this.txtPUInCode.Text = "";
            this.dtpPUInDate.Value = Convert.ToDateTime("1900-01-01");
            this.cbxOperatorCode.SelectedIndex = -1;
            this.cbxSupplierCode.SelectedIndex = -1;
            this.cbxStoreCode.SelectedIndex = -1;
            this.cbxInvenCode.SelectedIndex = -1;
            this.txtUnitPrice.Text = "";
            this.txtQuantity.Text = "";
            this.txtPUMoney.Text = "";
            this.txtPUOrderCode.Text = "";
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
            this.txtPUInCode.Text = this.dgvPUInStoreInfo[0, this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            this.dtpPUInDate.Value = Convert.ToDateTime(this.dgvPUInStoreInfo[1, this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value);
            this.cbxOperatorCode.SelectedValue = this.dgvPUInStoreInfo[2, this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value;
            this.cbxSupplierCode.SelectedValue = this.dgvPUInStoreInfo[3, this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value;
            this.cbxStoreCode.SelectedValue = this.dgvPUInStoreInfo[4, this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value;
            this.cbxInvenCode.SelectedValue = this.dgvPUInStoreInfo[5, this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value;
            this.txtUnitPrice.Text = this.dgvPUInStoreInfo[6, this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtQuantity.Text = this.dgvPUInStoreInfo[7, this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtPUMoney.Text = this.dgvPUInStoreInfo[8, this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtPUOrderCode.Text = this.dgvPUInStoreInfo[9, this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxEmployeeCode.SelectedValue = this.dgvPUInStoreInfo[10, this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value;
            this.cbxIsFlag.SelectedValue = this.dgvPUInStoreInfo[11, this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value;
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM PUInStore " + strWhere; ;

            try
            {
                this.dgvPUInStoreInfo.DataSource = db.GetDataSet(strSql, "PUInStore").Tables["PUInStore"];
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

            pars.Add(new SqlParameter("@PUInCode", txtPUInCode.Text.Trim()));
            pars.Add(new SqlParameter("@PUInDate", dtpPUInDate.Value));

            if (cbxOperatorCode.SelectedValue == null)
                pars.Add(new SqlParameter("@OperatorCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@OperatorCode", cbxOperatorCode.SelectedValue.ToString()));

            if (cbxSupplierCode.SelectedValue == null)
                pars.Add(new SqlParameter("@SupplierCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@SupplierCode", cbxSupplierCode.SelectedValue.ToString()));

            if (cbxStoreCode.SelectedValue == null)
                pars.Add(new SqlParameter("@StoreCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@StoreCode", cbxStoreCode.SelectedValue.ToString()));

            if (cbxInvenCode.SelectedValue == null)
                pars.Add(new SqlParameter("@InvenCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@InvenCode", cbxInvenCode.SelectedValue.ToString()));

            if (String.IsNullOrEmpty(txtUnitPrice.Text.Trim()))
                pars.Add(new SqlParameter("@UnitPrice", 0));
            else
                pars.Add(new SqlParameter("@UnitPrice", Convert.ToDecimal(txtUnitPrice.Text.Trim())));

            if (String.IsNullOrEmpty(txtQuantity.Text.Trim()))
                pars.Add(new SqlParameter("@Quantity", 0));
            else
                pars.Add(new SqlParameter("@Quantity", Convert.ToInt32(txtQuantity.Text.Trim())));

            if (String.IsNullOrEmpty(txtPUMoney.Text.Trim()))
                pars.Add(new SqlParameter("@PUMoney", 0));
            else
                pars.Add(new SqlParameter("@PUMoney", Convert.ToDecimal(txtPUMoney.Text.Trim())));

            pars.Add(new SqlParameter("@PUOrderCode", txtPUOrderCode.Text.Trim()));

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
        /// 计算采购金额
        /// </summary>
        private void ComputeMoney()
        {
            int int_Quantity;
            decimal dec_UnitPrice;

            if (!String.IsNullOrEmpty(txtQuantity.Text.Trim()) && !String.IsNullOrEmpty(txtUnitPrice.Text.Trim()))
            {
                int_Quantity = Convert.ToInt32(txtQuantity.Text.Trim());
                dec_UnitPrice = Convert.ToDecimal(txtUnitPrice.Text.Trim());
                txtPUMoney.Text = Decimal.Round(int_Quantity * dec_UnitPrice, 2).ToString();
            }
        }

        private void FormPUInStore_Load(object sender, EventArgs e)
        {
            //权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            commUse.CortrolButtonEnabled(toolCheck, this);
            commUse.CortrolButtonEnabled(toolUnCheck, this);

            //ComboBox绑定到数据源
            commUse.BindComboBox(cbxOperatorCode, "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(cbxSupplierCode, "SupplierCode", "SupplierName", "select SupplierCode,SupplierName from BSSupplier", "BSSupplier");
            commUse.BindComboBox(cbxStoreCode, "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(cbxInvenCode, "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(cbxEmployeeCode, "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(cbxIsFlag, "Code", "Name", "select * from INCheckFlag", "INCheckFlag");

            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvPUInStoreInfo.Columns[2], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvPUInStoreInfo.Columns[3], "SupplierCode", "SupplierName", "select SupplierCode,SupplierName from BSSupplier", "BSSupplier");
            commUse.BindComboBox(this.dgvPUInStoreInfo.Columns[4], "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(this.dgvPUInStoreInfo.Columns[5], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvPUInStoreInfo.Columns[10], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvPUInStoreInfo.Columns[11], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
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

            dtpPUInDate.Value = DateTime.Today;
            cbxOperatorCode.SelectedValue = PropertyClass.OperatorCode;
            txtQuantity.Text = "1";
            cbxIsFlag.SelectedValue = "0";

            txtPUInCode.Text = commUse.BuildBillCode("PUInStore", "PUInCode", "PUInDate", dtpPUInDate.Value);
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //修改标识

        }

        private void dgvPUInStoreInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT" && dgvPUInStoreInfo.RowCount > 0)
            {
                if (this.dgvPUInStoreInfo[11, this.dgvPUInStoreInfo.CurrentRow.Index].Value.ToString() == "1")
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

        private void btnChoice_Click(object sender, EventArgs e)
        {
            FormBrowsePUOrder formBrowsePUOrder = new FormBrowsePUOrder();
            formBrowsePUOrder.Owner = this;
            formBrowsePUOrder.ShowDialog();
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            string strCode = null;

            if (String.IsNullOrEmpty(txtPUInCode.Text.Trim()))
            {
                MessageBox.Show("单据编号不许为空！", "软件提示");
                txtPUInCode.Focus();
                return;
            }

            if (cbxSupplierCode.SelectedValue == null)
            {
                MessageBox.Show("供应商不许为空！", "软件提示");
                cbxSupplierCode.Focus();
                return;
            }

            if (cbxStoreCode.SelectedValue == null)
            {
                MessageBox.Show("仓库不许为空！","软件提示");
                cbxStoreCode.Focus();
                return;
            }

            if (cbxInvenCode.SelectedValue == null)
            {
                MessageBox.Show("存货不许为空！", "软件提示");
                cbxInvenCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtUnitPrice.Text.Trim()))
            {
                MessageBox.Show("单价不许为空！", "软件提示");
                txtUnitPrice.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                MessageBox.Show("数量不许为空！", "软件提示");
                txtQuantity.Focus();
                return;
            }
            else
            {
                if (Convert.ToInt32(txtQuantity.Text.Trim()) == 0)
                {
                    MessageBox.Show("数量不能等于零", "软件提示");
                    txtQuantity.Focus();
                    return;
                }
            }

            //添加
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                try
                {
                    strCode = "INSERT INTO PUInStore(PUInCode,PUInDate,OperatorCode,SupplierCode,StoreCode,InvenCode,UnitPrice,Quantity,PUMoney,PUOrderCode,EmployeeCode,IsFlag) ";
                    strCode += "VALUES(@PUInCode,@PUInDate,@OperatorCode,@SupplierCode,@StoreCode,@InvenCode,@UnitPrice,@Quantity,@PUMoney,@PUOrderCode,@EmployeeCode,@IsFlag)";

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
                string strPUInCode = txtPUInCode.Text.Trim();

                try
                {
                    strCode = "UPDATE PUInStore SET PUInCode = @PUInCode,PUInDate = @PUInDate,OperatorCode = @OperatorCode, SupplierCode = @SupplierCode,StoreCode = @StoreCode,";
                    strCode += "InvenCode = @InvenCode,UnitPrice = @UnitPrice,Quantity = @Quantity,";
                    strCode += "PUMoney = @PUMoney,PUOrderCode = @PUOrderCode,EmployeeCode = @EmployeeCode,IsFlag = @IsFlag";
                    strCode += " WHERE PUInCode = @OldPUInCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldPUInCode", strPUInCode));

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
            string strPUInCode = null; //单据编号
            string strSql = null;
            string strFlag = null; //审核标记

            if (this.dgvPUInStoreInfo.RowCount <= 0)
            {
                return;
            }

            strPUInCode = this.dgvPUInStoreInfo[0, this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            strFlag = this.dgvPUInStoreInfo[11, this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();

            if (strFlag == "1")
            {
                MessageBox.Show("该单据已审核，不许删除！", "软件提示");
                return;
            }

            strSql = "DELETE FROM PUInStore WHERE PUInCode = @PUInCode";
            SqlParameter[] pars = { new SqlParameter("@PUInCode", strPUInCode) };

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
            SqlDataReader sdr = null;
            string strCode = null;
            string strPUInCode = null;
            string strIsFlag = null;

            string strStoreCode = null;
            string strInvenCode = null;

            int intQuantity;
            decimal decPrice;
            decimal decMoney;

            if (dgvPUInStoreInfo.RowCount == 0) return;

            strStoreCode = this.dgvPUInStoreInfo["StoreCode", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            strInvenCode = this.dgvPUInStoreInfo["InvenCode", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            strPUInCode = this.dgvPUInStoreInfo["PUInCode", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            strIsFlag = this.dgvPUInStoreInfo["IsFlag", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();

            intQuantity = Convert.ToInt32(this.dgvPUInStoreInfo["Quantity", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value);
            decPrice = Convert.ToDecimal(this.dgvPUInStoreInfo["UnitPrice", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value);
            decMoney = Convert.ToDecimal(this.dgvPUInStoreInfo["PUMoney", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value);

            if (strIsFlag == "1")
            {
                MessageBox.Show("该单据已审核过，不许再次审核！", "软件提示");
                return;
            }

            // ✅ 查询库存（参数化）
            strCode = "SELECT Quantity,AvePrice,STMoney FROM STStock WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
            SqlParameter[] checkPars = {
        new SqlParameter("@StoreCode", strStoreCode),
        new SqlParameter("@InvenCode", strInvenCode)
    };

            try
            {
                sdr = db.GetDataReader(strCode, checkPars);
                sdr.Read();

                List<(string, SqlParameter[])> commands = new List<(string, SqlParameter[])>();

                if (sdr.HasRows)
                {
                    // 更新库存
                    intQuantity = intQuantity + sdr.GetInt32(0);
                    decMoney = decMoney + sdr.GetDecimal(2);
                    decPrice = Decimal.Round(decMoney / intQuantity, 2);

                    sdr.Close();

                    string stockSql = "UPDATE STStock SET Quantity = @Quantity, AvePrice = @AvePrice, STMoney = @STMoney " +
                                      "WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
                    SqlParameter[] stockPars = {
                new SqlParameter("@Quantity", intQuantity),
                new SqlParameter("@AvePrice", decPrice),
                new SqlParameter("@STMoney", decMoney),
                new SqlParameter("@StoreCode", strStoreCode),
                new SqlParameter("@InvenCode", strInvenCode)
            };
                    commands.Add((stockSql, stockPars));
                }
                else
                {
                    sdr.Close();

                    string stockSql = "INSERT INTO STStock(StoreCode,InvenCode,Quantity,AvePrice,STMoney) " +
                                      "VALUES(@StoreCode, @InvenCode, @Quantity, @AvePrice, @STMoney)";
                    SqlParameter[] stockPars = {
                new SqlParameter("@StoreCode", strStoreCode),
                new SqlParameter("@InvenCode", strInvenCode),
                new SqlParameter("@Quantity", intQuantity),
                new SqlParameter("@AvePrice", decPrice),
                new SqlParameter("@STMoney", decMoney)
            };
                    commands.Add((stockSql, stockPars));
                }

                // 更新审核标记
                string puSql = "UPDATE PUInStore SET IsFlag = '1' WHERE PUInCode = @PUInCode";
                commands.Add((puSql, new SqlParameter[] { new SqlParameter("@PUInCode", strPUInCode) }));

                if (db.ExecDataBySqls(commands))
                    MessageBox.Show("审核成功！", "软件提示");
                else
                    MessageBox.Show("审核失败！", "软件提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                if (sdr != null) sdr.Close();
                throw;
            }

            this.BindDataGridView("");
        }

        private void toolUnCheck_Click(object sender, EventArgs e)
        {
            SqlDataReader sdr = null;
            string strCode = null;
            string strPUInCode = null;
            string strIsFlag = null;

            string strStoreCode = null;
            string strInvenCode = null;

            int intQuantity;
            decimal decPrice;
            decimal decMoney;

            if (dgvPUInStoreInfo.RowCount == 0) return;

            strStoreCode = this.dgvPUInStoreInfo["StoreCode", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            strInvenCode = this.dgvPUInStoreInfo["InvenCode", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            strPUInCode = this.dgvPUInStoreInfo["PUInCode", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            strIsFlag = this.dgvPUInStoreInfo["IsFlag", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value.ToString();

            intQuantity = Convert.ToInt32(this.dgvPUInStoreInfo["Quantity", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value);
            decPrice = Convert.ToDecimal(this.dgvPUInStoreInfo["UnitPrice", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value);
            decMoney = Convert.ToDecimal(this.dgvPUInStoreInfo["PUMoney", this.dgvPUInStoreInfo.CurrentCell.RowIndex].Value);

            // ✅ 检查是否已付款（参数化）
            strCode = "SELECT * FROM PUPay WHERE PUInCode = @PUInCode";
            SqlParameter[] payPars = { new SqlParameter("@PUInCode", strPUInCode) };

            try
            {
                sdr = db.GetDataReader(strCode, payPars);
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
                if (sdr != null) sdr.Close();
                throw;
            }
            finally
            {
                if (sdr != null) sdr.Close();
            }

            if (strIsFlag == "0")
            {
                MessageBox.Show("该单据未审核，无需弃审！", "软件提示");
                return;
            }

            // ✅ 查询库存（参数化）
            strCode = "SELECT Quantity,AvePrice,STMoney FROM STStock WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
            SqlParameter[] checkPars = {
        new SqlParameter("@StoreCode", strStoreCode),
        new SqlParameter("@InvenCode", strInvenCode)
    };

            try
            {
                sdr = db.GetDataReader(strCode, checkPars);
                sdr.Read();

                if (!sdr.HasRows)
                {
                    MessageBox.Show("库存数据异常，无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }

                if (sdr.GetInt32(0) < intQuantity)
                {
                    MessageBox.Show("该笔入库且经审核的存货，已经发生了相关业务，无法弃审！", "软件提示");
                    sdr.Close();
                    return;
                }

                intQuantity = sdr.GetInt32(0) - intQuantity;
                decMoney = sdr.GetDecimal(2) - decMoney;

                if (intQuantity == 0)
                {
                    decPrice = 0;
                    decMoney = 0;
                }
                else
                {
                    decPrice = Decimal.Round(decMoney / intQuantity, 2);
                }
                sdr.Close();

                List<(string, SqlParameter[])> commands = new List<(string, SqlParameter[])>();

                string stockSql = "UPDATE STStock SET Quantity = @Quantity, AvePrice = @AvePrice, STMoney = @STMoney " +
                                  "WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
                SqlParameter[] stockPars = {
            new SqlParameter("@Quantity", intQuantity),
            new SqlParameter("@AvePrice", decPrice),
            new SqlParameter("@STMoney", decMoney),
            new SqlParameter("@StoreCode", strStoreCode),
            new SqlParameter("@InvenCode", strInvenCode)
        };
                commands.Add((stockSql, stockPars));

                string puSql = "UPDATE PUInStore SET IsFlag = '0' WHERE PUInCode = @PUInCode";
                commands.Add((puSql, new SqlParameter[] { new SqlParameter("@PUInCode", strPUInCode) }));

                if (db.ExecDataBySqls(commands))
                    MessageBox.Show("弃审成功！", "软件提示");
                else
                    MessageBox.Show("弃审失败！", "软件提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                if (sdr != null) sdr.Close();
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

                    strWhere = " WHERE PUInCode LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "单据日期":

                    strWhere = " WHERE SUBSTRING(CONVERT(VARCHAR(20),PUInDate,20),1,10) LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                default:
                    break;
            }
        }

        private void dgvPUInStoreInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
