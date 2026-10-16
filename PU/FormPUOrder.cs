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

namespace ERP.PU
{
    public partial class FormPUOrder : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormPUOrder()
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
            this.cbxSupplierCode.Enabled = !this.cbxSupplierCode.Enabled;
            this.cbxStoreCode.Enabled = !this.cbxStoreCode.Enabled;
            this.cbxInvenCode.Enabled = !this.cbxInvenCode.Enabled;
            this.txtUnitPrice.ReadOnly = !this.txtUnitPrice.ReadOnly;
            this.txtQuantity.ReadOnly = !this.txtQuantity.ReadOnly;    
            this.dtpRecInvenDate.Enabled = !this.dtpRecInvenDate.Enabled;
            this.cbxEmployeeCode.Enabled = !this.cbxEmployeeCode.Enabled;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            this.txtPUOrderCode.Text = "";
            this.dtpPUOrderDate.Value = Convert.ToDateTime("1900-01-01");
            this.cbxOperatorCode.SelectedIndex = -1;
            this.cbxSupplierCode.SelectedIndex = -1;
            this.cbxStoreCode.SelectedIndex = -1;
            this.cbxInvenCode.SelectedIndex = -1;
            this.txtUnitPrice.Text = "";
            this.txtQuantity.Text = "";
            this.txtPUMoney.Text = "";
            this.dtpRecInvenDate.Value = Convert.ToDateTime("1900-01-01");
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
            this.txtPUOrderCode.Text = this.dgvPUOrderInfo[0, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value.ToString();
            this.dtpPUOrderDate.Value = Convert.ToDateTime(this.dgvPUOrderInfo[1, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value);
            this.cbxOperatorCode.SelectedValue= this.dgvPUOrderInfo[2, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value;
            this.cbxSupplierCode.SelectedValue = this.dgvPUOrderInfo[3, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value;
            this.cbxStoreCode.SelectedValue = this.dgvPUOrderInfo[4, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value;
            this.cbxInvenCode.SelectedValue = this.dgvPUOrderInfo[5, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value;
            this.txtUnitPrice.Text = this.dgvPUOrderInfo[6, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtQuantity.Text = this.dgvPUOrderInfo[7, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtPUMoney.Text = this.dgvPUOrderInfo[8, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value.ToString();
            this.dtpRecInvenDate.Value = Convert.ToDateTime(this.dgvPUOrderInfo[9, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value);
            this.cbxEmployeeCode.SelectedValue = this.dgvPUOrderInfo[10, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value;
            this.cbxIsFlag.SelectedValue = this.dgvPUOrderInfo[11, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value;
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM PUOrder " + strWhere; ;

            try
            {
                this.dgvPUOrderInfo.DataSource = db.GetDataSet(strSql, "PUOrder").Tables["PUOrder"];
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

            pars.Add(new SqlParameter("@PUOrderCode", txtPUOrderCode.Text.Trim()));
            pars.Add(new SqlParameter("@PUOrderDate", dtpPUOrderDate.Value));

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

            pars.Add(new SqlParameter("@RecInvenDate", dtpRecInvenDate.Value));

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

        private void FormPUOrder_Load(object sender, EventArgs e)
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
            commUse.BindComboBox(this.dgvPUOrderInfo.Columns[2], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvPUOrderInfo.Columns[3], "SupplierCode", "SupplierName", "select SupplierCode,SupplierName from BSSupplier", "BSSupplier");
            commUse.BindComboBox(this.dgvPUOrderInfo.Columns[4], "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(this.dgvPUOrderInfo.Columns[5], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvPUOrderInfo.Columns[10], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvPUOrderInfo.Columns[11], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
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

            dtpPUOrderDate.Value = DateTime.Today;
            cbxOperatorCode.SelectedValue = PropertyClass.OperatorCode;
            txtQuantity.Text = "1";
            dtpRecInvenDate.Value = DateTime.Today;
            cbxIsFlag.SelectedValue = "0";
            txtPUOrderCode.Text = commUse.BuildBillCode("PUOrder", "PUOrderCode", "PUOrderDate", dtpPUOrderDate.Value);
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //修改标识
        }

        private void dgvPUOrderInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT" && dgvPUOrderInfo.RowCount > 0)
            {
                if (this.dgvPUOrderInfo[11, this.dgvPUOrderInfo.CurrentRow.Index].Value.ToString() == "1")
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

        private void toolSave_Click(object sender, EventArgs e)
        {
            string strCode = null;

            if (String.IsNullOrEmpty(txtPUOrderCode.Text.Trim()))
            {
                MessageBox.Show("单据编号不许为空！", "软件提示");
                txtPUOrderCode.Focus();
                return;
            }

            if (cbxSupplierCode.SelectedValue == null)
            {
                MessageBox.Show("供应商不许为空！", "软件提示");
                cbxSupplierCode.Focus();
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
                    strCode = "INSERT INTO PUOrder(PUOrderCode,PUOrderDate,OperatorCode,SupplierCode,StoreCode,InvenCode,UnitPrice,Quantity,PUMoney,RecInvenDate,EmployeeCode,IsFlag) ";
                    strCode += "VALUES(@PUOrderCode,@PUOrderDate,@OperatorCode,@SupplierCode,@StoreCode,@InvenCode,@UnitPrice,@Quantity,@PUMoney,@RecInvenDate,@EmployeeCode,@IsFlag)";

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
                string strPUOrderCode = txtPUOrderCode.Text.Trim();

                try
                {
                    strCode = "UPDATE PUOrder SET PUOrderCode = @PUOrderCode,PUOrderDate = @PUOrderDate,OperatorCode = @OperatorCode, SupplierCode = @SupplierCode,StoreCode = @StoreCode,";
                    strCode += "InvenCode = @InvenCode,UnitPrice = @UnitPrice,Quantity = @Quantity,";
                    strCode += "PUMoney = @PUMoney,RecInvenDate = @RecInvenDate,EmployeeCode = @EmployeeCode,IsFlag = @IsFlag";
                    strCode += " WHERE PUOrderCode = @OldPUOrderCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldPUOrderCode", strPUOrderCode));

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
            string strPUOrderCode = null; //订单编号
            string strSql = null;
            string strFlag = null; //审核标记

            if (this.dgvPUOrderInfo.RowCount <= 0)
            {
                return;
            }

            strPUOrderCode = this.dgvPUOrderInfo[0, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value.ToString();
            strFlag = this.dgvPUOrderInfo[11, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value.ToString();

            if (strFlag == "1")
            {
                MessageBox.Show("该单据已审核，不许删除！","软件提示");
                return; 
            }
            strSql = "DELETE FROM PUOrder WHERE PUOrderCode = @PUOrderCode";
            SqlParameter[] pars = { new SqlParameter("@PUOrderCode", strPUOrderCode) };

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
            string strPUOrderCode = null; //单据编号
            string strSql = null;
            string strFlag = null; //审核标记

            if (dgvPUOrderInfo.RowCount == 0)
            {
                return;
            }

            strPUOrderCode = this.dgvPUOrderInfo[0, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value.ToString();
            strFlag = this.dgvPUOrderInfo[11, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value.ToString();

            if (strFlag == "1")
            {
                MessageBox.Show("该单据已审核过，不许再次审核！", "软件提示");
                return;
            }

            strSql = "UPDATE PUOrder SET IsFlag = '1' WHERE PUOrderCode = @PUOrderCode";
            SqlParameter[] pars = { new SqlParameter("@PUOrderCode", strPUOrderCode) };

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
            string strPUOrderCode = null;   // 单据编号
            string strSql = null;
            string strFlag = null;          // 审核标记
            SqlDataReader sdr = null;

            if (dgvPUOrderInfo.RowCount == 0)
            {
                return;
            }

            strPUOrderCode = this.dgvPUOrderInfo[0, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value.ToString();
            strFlag = this.dgvPUOrderInfo[11, this.dgvPUOrderInfo.CurrentCell.RowIndex].Value.ToString();

            // ✅ 检查是否已存在采购入库单（参数化）
            strSql = "SELECT * FROM PUInStore WHERE PUOrderCode = @PUOrderCode";
            SqlParameter[] checkPars = { new SqlParameter("@PUOrderCode", strPUOrderCode) };

            try
            {
                sdr = db.GetDataReader(strSql, checkPars);
                sdr.Read();

                // 该订单已经存在对应的采购入库单，不许弃审
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

            // ✅ 打上弃审标记（参数化）
            strSql = "UPDATE PUOrder SET IsFlag = '0' WHERE PUOrderCode = @PUOrderCode";
            SqlParameter[] pars = { new SqlParameter("@PUOrderCode", strPUOrderCode) };

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

                    strWhere = " WHERE PUOrderCode LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "单据日期":

                    strWhere = " WHERE SUBSTRING(CONVERT(VARCHAR(20),PUOrderDate,20),1,10) LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                default:
                    break;
            }
        }

        private void dgvPUOrderInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
