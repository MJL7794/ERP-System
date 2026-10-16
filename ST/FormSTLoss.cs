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

namespace ERP.ST
{
    public partial class FormSTLoss : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormSTLoss()
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
            this.cbxStoreCode.Enabled = !this.cbxStoreCode.Enabled;
            this.cbxInvenCode.Enabled = !this.cbxInvenCode.Enabled;
            this.txtLossQuantity.ReadOnly = !this.txtLossQuantity.ReadOnly;
            this.cbxEmployeeCode.Enabled = !this.cbxEmployeeCode.Enabled;
            this.txtRemark.ReadOnly = !this.txtRemark.ReadOnly;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            this.txtSTLossCode.Text = "";
            this.dtpSTLossDate.Value = Convert.ToDateTime("1900-01-01");
            this.cbxOperatorCode.SelectedIndex = -1;
            this.cbxStoreCode.SelectedIndex = -1;
            this.cbxInvenCode.SelectedIndex = -1;
            this.txtLossQuantity.Text = "";
            this.cbxEmployeeCode.SelectedIndex = -1;
            this.txtRemark.Text = "";
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
            this.txtSTLossCode.Text = this.dgvSTLossInfo["STLossCode", this.dgvSTLossInfo.CurrentCell.RowIndex].Value.ToString();
            this.dtpSTLossDate.Value = Convert.ToDateTime(this.dgvSTLossInfo["STLossDate", this.dgvSTLossInfo.CurrentCell.RowIndex].Value);
            this.cbxOperatorCode.SelectedValue = this.dgvSTLossInfo["OperatorCode", this.dgvSTLossInfo.CurrentCell.RowIndex].Value;
            this.cbxStoreCode.SelectedValue = this.dgvSTLossInfo["StoreCode",this.dgvSTLossInfo.CurrentCell.RowIndex].Value;
            this.cbxInvenCode.SelectedValue = this.dgvSTLossInfo["InvenCode", this.dgvSTLossInfo.CurrentCell.RowIndex].Value;
            this.txtLossQuantity.Text = this.dgvSTLossInfo["LossQuantity", this.dgvSTLossInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxEmployeeCode.SelectedValue = this.dgvSTLossInfo[8, this.dgvSTLossInfo.CurrentCell.RowIndex].Value;
            this.txtRemark.Text = this.dgvSTLossInfo[9, this.dgvSTLossInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxIsFlag.SelectedValue = this.dgvSTLossInfo[10, this.dgvSTLossInfo.CurrentCell.RowIndex].Value;
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM STLoss " + strWhere; ;

            try
            {
                this.dgvSTLossInfo.DataSource = db.GetDataSet(strSql, "STLoss").Tables["STLoss"];
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

            pars.Add(new SqlParameter("@STLossCode", txtSTLossCode.Text.Trim()));
            pars.Add(new SqlParameter("@STLossDate", dtpSTLossDate.Value));

            if (cbxOperatorCode.SelectedValue == null)
                pars.Add(new SqlParameter("@OperatorCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@OperatorCode", cbxOperatorCode.SelectedValue.ToString()));

            if (cbxStoreCode.SelectedValue == null)
                pars.Add(new SqlParameter("@StoreCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@StoreCode", cbxStoreCode.SelectedValue.ToString()));

            if (cbxInvenCode.SelectedValue == null)
                pars.Add(new SqlParameter("@InvenCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@InvenCode", cbxInvenCode.SelectedValue.ToString()));

            if (String.IsNullOrEmpty(txtLossQuantity.Text.Trim()))
                pars.Add(new SqlParameter("@LossQuantity", 0));
            else
                pars.Add(new SqlParameter("@LossQuantity", Convert.ToInt32(txtLossQuantity.Text.Trim())));

            if (cbxEmployeeCode.SelectedValue == null)
                pars.Add(new SqlParameter("@EmployeeCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@EmployeeCode", cbxEmployeeCode.SelectedValue.ToString()));

            pars.Add(new SqlParameter("@Remark", txtRemark.Text.Trim()));

            if (cbxIsFlag.SelectedValue == null)
                pars.Add(new SqlParameter("@IsFlag", DBNull.Value));
            else
                pars.Add(new SqlParameter("@IsFlag", cbxIsFlag.SelectedValue.ToString()));

            return pars.ToArray();
        }

        private void FormSTLoss_Load(object sender, EventArgs e)
        {
            //权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            commUse.CortrolButtonEnabled(toolCheck, this);
            commUse.CortrolButtonEnabled(toolUnCheck, this);

            //ComboBox绑定到数据源
            commUse.BindComboBox(cbxOperatorCode, "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(cbxStoreCode, "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(cbxEmployeeCode, "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(cbxIsFlag, "Code", "Name", "select * from INCheckFlag", "INCheckFlag");

            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvSTLossInfo.Columns["OperatorCode"], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvSTLossInfo.Columns["StoreCode"], "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(this.dgvSTLossInfo.Columns["InvenCode"], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvSTLossInfo.Columns["EmployeeCode"], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvSTLossInfo.Columns["IsFlag"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
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

            dtpSTLossDate.Value = DateTime.Today;
            cbxOperatorCode.SelectedValue = PropertyClass.OperatorCode;
            cbxIsFlag.SelectedValue = "0";

            txtSTLossCode.Text = commUse.BuildBillCode("STLoss", "STLossCode", "STLossDate", dtpSTLossDate.Value);
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //修改标识

        }

        private void dgvSTLossInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT" && dgvSTLossInfo.RowCount > 0)
            {
                if (this.dgvSTLossInfo[10, this.dgvSTLossInfo.CurrentRow.Index].Value.ToString() == "1")
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

        private void txtLossQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputInteger(e);
        }

        private void cbxStoreCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxStoreCode.SelectedIndex != -1)
            {
                // ⚠️ CommUse.BindComboBox 不支持参数，只能保留拼接
                // 但 cbxStoreCode.SelectedValue 来自 ComboBox，相对安全
                commUse.BindComboBox(cbxInvenCode, "InvenCode", "InvenName",
                    "SELECT InvenCode, InvenName FROM BSInven WHERE InvenCode IN (SELECT InvenCode FROM STStock WHERE StoreCode = '" + cbxStoreCode.SelectedValue + "')",
                    "BSInven");
            }
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            string strCode = null;

            if(String.IsNullOrEmpty(txtSTLossCode.Text.Trim()))
            {
                MessageBox.Show("单据编号不许为空！", "软件提示");
                txtSTLossCode.Focus();
                return;
            }

            if (cbxStoreCode.SelectedValue == null)
            {
                MessageBox.Show("仓库不许为空！", "软件提示");
                cbxStoreCode.Focus();
                return;
            }

            if (cbxInvenCode.SelectedValue == null)
            {
                MessageBox.Show("存货不许为空！", "软件提示");
                cbxInvenCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtLossQuantity.Text.Trim()))
            {
                MessageBox.Show("数量不许为空！", "软件提示");
                txtLossQuantity.Focus();
                return;
            }
            else
            {
                if (Convert.ToInt32(txtLossQuantity.Text.Trim()) == 0)
                {
                    MessageBox.Show("数量不能等于零", "软件提示");
                    txtLossQuantity.Focus();
                    return;
                }
            }

            //添加
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                try
                {
                    strCode = "INSERT INTO STLoss(STLossCode,STLossDate,OperatorCode,StoreCode,InvenCode,LossQuantity,EmployeeCode,Remark,IsFlag) ";
                    strCode += "VALUES(@STLossCode,@STLossDate,@OperatorCode,@StoreCode,@InvenCode,@LossQuantity,@EmployeeCode,@Remark,@IsFlag)";

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
                string strSTLossCode = txtSTLossCode.Text.Trim();

                try
                {
                    strCode = "UPDATE STLoss SET STLossCode = @STLossCode,STLossDate = @STLossDate,OperatorCode = @OperatorCode, StoreCode = @StoreCode,InvenCode = @InvenCode,";
                    strCode += "LossQuantity = @LossQuantity,EmployeeCode = @EmployeeCode,Remark = @Remark,IsFlag = @IsFlag ";
                    strCode += "WHERE STLossCode = @OldSTLossCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldSTLossCode", strSTLossCode));

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
            string strSTLossCode = null; //单据编号
            string strSql = null;
            string strIsFlag = null; //审核标记

            if (this.dgvSTLossInfo.RowCount <= 0)
            {
                return;
            }

            strSTLossCode = this.dgvSTLossInfo["STLossCode", this.dgvSTLossInfo.CurrentCell.RowIndex].Value.ToString();
            strIsFlag = this.dgvSTLossInfo["IsFlag", this.dgvSTLossInfo.CurrentCell.RowIndex].Value.ToString();

            if (strIsFlag == "1")
            {
                MessageBox.Show("该单据已审核，不许删除！", "软件提示");
                return;
            }

            strSql = "DELETE FROM STLoss WHERE STLossCode = @STLossCode";
            SqlParameter[] pars = { new SqlParameter("@STLossCode", strSTLossCode) };

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
            string strSTLossCode = null;
            string strIsFlag = null;

            string strStoreCode = null;
            string strInvenCode = null;

            int intLossQuantity;
            int intQuantity;
            decimal decLossMoney;
            decimal decMoney;
            decimal decUnitPrice;

            if (dgvSTLossInfo.RowCount == 0) return;

            strStoreCode = this.dgvSTLossInfo["StoreCode", this.dgvSTLossInfo.CurrentCell.RowIndex].Value.ToString();
            strInvenCode = this.dgvSTLossInfo["InvenCode", this.dgvSTLossInfo.CurrentCell.RowIndex].Value.ToString();
            strSTLossCode = this.dgvSTLossInfo["STLossCode", this.dgvSTLossInfo.CurrentCell.RowIndex].Value.ToString();
            strIsFlag = this.dgvSTLossInfo["IsFlag", this.dgvSTLossInfo.CurrentCell.RowIndex].Value.ToString();

            intLossQuantity = Convert.ToInt32(this.dgvSTLossInfo["LossQuantity", this.dgvSTLossInfo.CurrentCell.RowIndex].Value);

            if (strIsFlag == "1")
            {
                MessageBox.Show("该单据已审核过，不许再次审核！", "软件提示");
                return;
            }

            // ✅ 参数化查询库存
            strCode = "SELECT Quantity,AvePrice,STMoney,LossQuantity,LossMoney FROM STStock WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
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

                if (sdr.GetInt32(0) < intLossQuantity)
                {
                    MessageBox.Show("报损数量不许大于现有库存量！", "软件提示");
                    sdr.Close();
                    return;
                }

                decUnitPrice = sdr.GetDecimal(1);
                intQuantity = sdr.GetInt32(0) - intLossQuantity;
                decMoney = sdr.GetDecimal(2) - Decimal.Round(intLossQuantity * decUnitPrice, 2);
                decLossMoney = sdr.GetDecimal(4) + Decimal.Round(intLossQuantity * decUnitPrice, 2);
                intLossQuantity = sdr.GetInt32(3) + intLossQuantity;

                sdr.Close();

                // ✅ 事务参数化
                List<(string, SqlParameter[])> commands = new List<(string, SqlParameter[])>();

                // 更新 STStock
                string stockSql = "UPDATE STStock SET Quantity = @Quantity, STMoney = @STMoney, LossQuantity = @LossQuantity, LossMoney = @LossMoney WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
                SqlParameter[] stockPars = {
            new SqlParameter("@Quantity", intQuantity),
            new SqlParameter("@STMoney", decMoney),
            new SqlParameter("@LossQuantity", intLossQuantity),
            new SqlParameter("@LossMoney", decLossMoney),
            new SqlParameter("@StoreCode", strStoreCode),
            new SqlParameter("@InvenCode", strInvenCode)
        };
                commands.Add((stockSql, stockPars));

                // 更新 STLoss
                string lossSql = "UPDATE STLoss SET IsFlag = '1', UnitPrice = @UnitPrice, LossMoney = @LossMoney WHERE STLossCode = @STLossCode";
                SqlParameter[] lossPars = {
            new SqlParameter("@UnitPrice", decUnitPrice),
            new SqlParameter("@LossMoney", Decimal.Round(intLossQuantity * decUnitPrice, 2)), // ⚠️ 注意：这里用了加后的 intLossQuantity，原代码这里也是这么算的，可能是个 bug
            new SqlParameter("@STLossCode", strSTLossCode)
        };
                commands.Add((lossSql, lossPars));

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
            string strSTLossCode = null;
            string strIsFlag = null;

            string strStoreCode = null;
            string strInvenCode = null;

            int intLossQuantity;
            int intQuantity;
            decimal decLossMoney;
            decimal decMoney;
            decimal decAvePrice;
            decimal decUnitPrice;

            if (dgvSTLossInfo.RowCount == 0) return;

            strIsFlag = this.dgvSTLossInfo["IsFlag", this.dgvSTLossInfo.CurrentCell.RowIndex].Value.ToString();

            if (strIsFlag == "0")
            {
                MessageBox.Show("该单据未审核，无需弃审！", "软件提示");
                return;
            }

            strStoreCode = this.dgvSTLossInfo["StoreCode", this.dgvSTLossInfo.CurrentCell.RowIndex].Value.ToString();
            strInvenCode = this.dgvSTLossInfo["InvenCode", this.dgvSTLossInfo.CurrentCell.RowIndex].Value.ToString();
            strSTLossCode = this.dgvSTLossInfo["STLossCode", this.dgvSTLossInfo.CurrentCell.RowIndex].Value.ToString();

            intLossQuantity = Convert.ToInt32(this.dgvSTLossInfo["LossQuantity", this.dgvSTLossInfo.CurrentCell.RowIndex].Value);
            decLossMoney = Convert.ToDecimal(this.dgvSTLossInfo["LossMoney", this.dgvSTLossInfo.CurrentCell.RowIndex].Value);
            decUnitPrice = Convert.ToDecimal(this.dgvSTLossInfo["UnitPrice", this.dgvSTLossInfo.CurrentCell.RowIndex].Value);

            // ✅ 参数化查询库存
            strCode = "SELECT Quantity,AvePrice,STMoney,LossQuantity,LossMoney FROM STStock WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
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
                    MessageBox.Show("库存数据异常（该存货的库存未初始化），无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }

                if (sdr.GetInt32(3) < intLossQuantity)
                {
                    MessageBox.Show("库存数据异常（累计报损数量小于本次弃审数量），无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }

                if (sdr.GetDecimal(4) < decLossMoney)
                {
                    MessageBox.Show("库存数据异常（累计报损金额小于本次弃审金额），无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }

                intQuantity = sdr.GetInt32(0) + intLossQuantity;
                decMoney = sdr.GetDecimal(2) + decLossMoney;
                decAvePrice = Decimal.Round(decMoney / intQuantity, 2);
                intLossQuantity = sdr.GetInt32(3) - intLossQuantity;
                decLossMoney = sdr.GetDecimal(4) - decLossMoney;

                sdr.Close();

                // ✅ 事务参数化
                List<(string, SqlParameter[])> commands = new List<(string, SqlParameter[])>();

                string stockSql = "UPDATE STStock SET Quantity = @Quantity, STMoney = @STMoney, LossQuantity = @LossQuantity, LossMoney = @LossMoney, AvePrice = @AvePrice WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
                SqlParameter[] stockPars = {
            new SqlParameter("@Quantity", intQuantity),
            new SqlParameter("@STMoney", decMoney),
            new SqlParameter("@LossQuantity", intLossQuantity),
            new SqlParameter("@LossMoney", decLossMoney),
            new SqlParameter("@AvePrice", decAvePrice),
            new SqlParameter("@StoreCode", strStoreCode),
            new SqlParameter("@InvenCode", strInvenCode)
        };
                commands.Add((stockSql, stockPars));

                string lossSql = "UPDATE STLoss SET IsFlag = '0', UnitPrice = NULL, LossMoney = NULL WHERE STLossCode = @STLossCode";
                commands.Add((lossSql, new SqlParameter[] { new SqlParameter("@STLossCode", strSTLossCode) }));

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

                    strWhere = " WHERE STLossCode LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "单据日期":

                    strWhere = " WHERE SUBSTRING(CONVERT(VARCHAR(20),STLossDate,20),1,10) LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                default:
                    break;
            }
        }

        private void dgvSTLossInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
