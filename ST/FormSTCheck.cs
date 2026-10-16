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
    public partial class FormSTCheck : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormSTCheck()
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
            this.txtCheckQuantity.ReadOnly = !this.txtCheckQuantity.ReadOnly;
            this.cbxEmployeeCode.Enabled = !this.cbxEmployeeCode.Enabled;
            
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            this.txtSTCheckCode.Text = "";
            this.dtpSTCheckDate.Value = Convert.ToDateTime("1900-01-01");
            this.cbxOperatorCode.SelectedIndex = -1;
            this.cbxStoreCode.SelectedIndex = -1;
            this.cbxInvenCode.SelectedIndex = -1;
            this.txtAccQuantity.Text = "";
            this.txtCheckQuantity.Text = "";
            this.txtBalQuantity.Text = "";
            this.txtAvePrice.Text = "";
            this.txtBalMoney.Text = "";
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
            this.txtSTCheckCode.Text = this.dgvSTCheckInfo["STCheckCode", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();
            this.dtpSTCheckDate.Value = Convert.ToDateTime(this.dgvSTCheckInfo["STCheckDate", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value);
            this.cbxOperatorCode.SelectedValue = this.dgvSTCheckInfo["OperatorCode", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value;
            this.cbxStoreCode.SelectedValue = this.dgvSTCheckInfo["StoreCode", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value;
            this.cbxInvenCode.SelectedValue = this.dgvSTCheckInfo["InvenCode", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value;
            this.txtAccQuantity.Text = this.dgvSTCheckInfo["AccQuantity", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtCheckQuantity.Text = this.dgvSTCheckInfo["CheckQuantity", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtBalQuantity.Text = this.dgvSTCheckInfo["BalQuantity", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtAvePrice.Text = this.dgvSTCheckInfo["AvePrice", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtBalMoney.Text = this.dgvSTCheckInfo["BalMoney", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxEmployeeCode.SelectedValue = this.dgvSTCheckInfo["EmployeeCode", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value;
            this.cbxIsFlag.SelectedValue = this.dgvSTCheckInfo["IsFlag", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value;
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM STCheck " + strWhere; ;

            try
            {
                this.dgvSTCheckInfo.DataSource = db.GetDataSet(strSql, "STCheck").Tables["STCheck"];
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

            pars.Add(new SqlParameter("@STCheckCode", txtSTCheckCode.Text.Trim()));
            pars.Add(new SqlParameter("@STCheckDate", dtpSTCheckDate.Value));

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

            if (String.IsNullOrEmpty(txtAccQuantity.Text.Trim()))
                pars.Add(new SqlParameter("@AccQuantity", 0));
            else
                pars.Add(new SqlParameter("@AccQuantity", Convert.ToInt32(txtAccQuantity.Text.Trim())));

            if (String.IsNullOrEmpty(txtCheckQuantity.Text.Trim()))
                pars.Add(new SqlParameter("@CheckQuantity", 0));
            else
                pars.Add(new SqlParameter("@CheckQuantity", Convert.ToInt32(txtCheckQuantity.Text.Trim())));

            if (String.IsNullOrEmpty(txtBalQuantity.Text.Trim()))
                pars.Add(new SqlParameter("@BalQuantity", 0));
            else
                pars.Add(new SqlParameter("@BalQuantity", Convert.ToInt32(txtBalQuantity.Text.Trim())));

            if (String.IsNullOrEmpty(txtAvePrice.Text.Trim()))
                pars.Add(new SqlParameter("@AvePrice", 0));
            else
                pars.Add(new SqlParameter("@AvePrice", Convert.ToDecimal(txtAvePrice.Text.Trim())));

            if (String.IsNullOrEmpty(txtBalMoney.Text.Trim()))
                pars.Add(new SqlParameter("@BalMoney", 0));
            else
                pars.Add(new SqlParameter("@BalMoney", Convert.ToDecimal(txtBalMoney.Text.Trim())));

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

        private void FormSTCheck_Load(object sender, EventArgs e)
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
            commUse.BindComboBox(this.dgvSTCheckInfo.Columns["OperatorCode"], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvSTCheckInfo.Columns["StoreCode"], "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(this.dgvSTCheckInfo.Columns["InvenCode"], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvSTCheckInfo.Columns["EmployeeCode"], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvSTCheckInfo.Columns["IsFlag"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
            
            //DataGridView绑定数据源
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

            dtpSTCheckDate.Value = DateTime.Today;
            cbxOperatorCode.SelectedValue = PropertyClass.OperatorCode;
            cbxIsFlag.SelectedValue = "0";

            txtSTCheckCode.Text = commUse.BuildBillCode("STCheck", "STCheckCode", "STCheckDate", dtpSTCheckDate.Value);
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //修改标识

        }

        private void dgvSTCheckInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT" && dgvSTCheckInfo.RowCount > 0)
            {
                if (this.dgvSTCheckInfo["IsFlag", this.dgvSTCheckInfo.CurrentRow.Index].Value.ToString() == "1")
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

        private void cbxStoreCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxStoreCode.SelectedIndex != -1)
            {
                // ⚠️ CommUse.BindComboBox 不支持参数，这里只能用拼接
                // 但 cbxStoreCode.SelectedValue 来自 ComboBox，相对安全
                commUse.BindComboBox(cbxInvenCode, "InvenCode", "InvenName",
                    "SELECT InvenCode, InvenName FROM BSInven WHERE InvenCode IN (SELECT InvenCode FROM STStock WHERE StoreCode = '" + cbxStoreCode.SelectedValue + "')",
                    "BSInven");
            }
        }

        private void cbxInvenCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strSql = null;
            SqlDataReader sdr = null;

            if (cbxInvenCode.SelectedIndex == -1) return;

            // ✅ 参数化
            strSql = "SELECT AvePrice, Quantity FROM STStock WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
            SqlParameter[] pars = {
        new SqlParameter("@StoreCode", cbxStoreCode.SelectedValue),
        new SqlParameter("@InvenCode", cbxInvenCode.SelectedValue)
    };

            try
            {
                sdr = db.GetDataReader(strSql, pars);
                sdr.Read();

                if (!sdr.HasRows)
                {
                    MessageBox.Show("库存数据异常!", "软件提示");
                    sdr.Close();
                    return;
                }

                this.txtAvePrice.Text = sdr.GetDecimal(0).ToString();
                this.txtAccQuantity.Text = sdr.GetInt32(1).ToString();
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

        private void txtCheckQuantity_TextChanged(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(txtAccQuantity.Text.Trim()) && !String.IsNullOrEmpty(txtCheckQuantity.Text.Trim()))
            {
                txtBalQuantity.Text = (Convert.ToInt32(txtCheckQuantity.Text.Trim()) - Convert.ToInt32(txtAccQuantity.Text.Trim())).ToString();
            }
        }

        private void txtBalQuantity_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtBalQuantity.Text.Trim()) && !String.IsNullOrEmpty(txtAvePrice.Text.Trim()))
            {
                txtBalMoney.Text = Decimal.Round(Convert.ToInt32(txtBalQuantity.Text.Trim()) * Convert.ToDecimal(txtAvePrice.Text.Trim()), 2).ToString();
            }
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            string strCode = null;

            if (String.IsNullOrEmpty(txtSTCheckCode.Text.Trim()))
            {
                MessageBox.Show("单据编号不许为空！", "软件提示");
                txtSTCheckCode.Focus();
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

            if (String.IsNullOrEmpty(txtCheckQuantity.Text.Trim()))
            {
                MessageBox.Show("实盘数量不许为空！", "软件提示");
                txtCheckQuantity.Focus();
                return;
            }
            else
            {
                if (Convert.ToInt32(txtCheckQuantity.Text.Trim()) == 0)
                {
                    MessageBox.Show("实盘数量不能等于零", "软件提示");
                    txtCheckQuantity.Focus();
                    return;
                }
            }

            //添加
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                try
                {
                    strCode = "INSERT INTO STCheck(STCheckCode,STCheckDate,OperatorCode,StoreCode,InvenCode,AccQuantity,CheckQuantity,BalQuantity,AvePrice,BalMoney,EmployeeCode,IsFlag) ";
                    strCode += "VALUES(@STCheckCode,@STCheckDate,@OperatorCode,@StoreCode,@InvenCode,@AccQuantity,@CheckQuantity,@BalQuantity,@AvePrice,@BalMoney,@EmployeeCode,@IsFlag)";

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
                string strSTCheckCode = txtSTCheckCode.Text.Trim();

                try
                {
                    strCode = "UPDATE STCheck SET STCheckCode = @STCheckCode,STCheckDate = @STCheckDate,OperatorCode = @OperatorCode, StoreCode = @StoreCode,InvenCode = @InvenCode,";
                    strCode += "AccQuantity = @AccQuantity,CheckQuantity = @CheckQuantity,BalQuantity = @BalQuantity,AvePrice = @AvePrice,BalMoney = @BalMoney,EmployeeCode = @EmployeeCode,IsFlag = @IsFlag ";
                    strCode += "WHERE STCheckCode = @OldSTCheckCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldSTCheckCode", strSTCheckCode));

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
            string strSTCheckCode = null; //单据编号
            string strSql = null;
            string strIsFlag = null; //审核标记

            if (this.dgvSTCheckInfo.RowCount <= 0)
            {
                return;
            }

            strSTCheckCode = this.dgvSTCheckInfo["STCheckCode", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();
            strIsFlag = this.dgvSTCheckInfo["IsFlag", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();

            if (strIsFlag == "1")
            {
                MessageBox.Show("该单据已审核，不许删除！", "软件提示");
                return;
            }

            strSql = "DELETE FROM STCheck WHERE STCheckCode = @STCheckCode";
            SqlParameter[] pars = { new SqlParameter("@STCheckCode", strSTCheckCode) };

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
            string strSTCheckCode = null;
            string strIsFlag = null;

            string strStoreCode = null;
            string strInvenCode = null;

            int intBalQuantity;
            decimal decBalMoney;
            int intQuantity;
            decimal decMoney;

            if (dgvSTCheckInfo.RowCount == 0) return;

            strStoreCode = this.dgvSTCheckInfo["StoreCode", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();
            strInvenCode = this.dgvSTCheckInfo["InvenCode", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();
            strSTCheckCode = this.dgvSTCheckInfo["STCheckCode", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();
            strIsFlag = this.dgvSTCheckInfo["IsFlag", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();

            intBalQuantity = Convert.ToInt32(this.dgvSTCheckInfo["BalQuantity", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value);
            decBalMoney = Convert.ToDecimal(this.dgvSTCheckInfo["BalMoney", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value);

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

                intQuantity = sdr.GetInt32(0) + intBalQuantity;
                decMoney = sdr.GetDecimal(2) + decBalMoney;

                if (intQuantity < 0 || decMoney < 0)
                {
                    MessageBox.Show("库存数据异常(平衡后库存数量或库存金额为负数)，无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }
                sdr.Close();

                // ✅ 事务参数化
                List<(string, SqlParameter[])> commands = new List<(string, SqlParameter[])>();

                string stockSql = "UPDATE STStock SET Quantity = @Quantity, STMoney = @STMoney WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
                SqlParameter[] stockPars = {
            new SqlParameter("@Quantity", intQuantity),
            new SqlParameter("@STMoney", decMoney),
            new SqlParameter("@StoreCode", strStoreCode),
            new SqlParameter("@InvenCode", strInvenCode)
        };
                commands.Add((stockSql, stockPars));

                string checkSql = "UPDATE STCheck SET IsFlag = '1' WHERE STCheckCode = @STCheckCode";
                commands.Add((checkSql, new SqlParameter[] { new SqlParameter("@STCheckCode", strSTCheckCode) }));

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
            string strSTCheckCode = null;
            string strIsFlag = null;

            string strStoreCode = null;
            string strInvenCode = null;

            int intBalQuantity;
            decimal decBalMoney;
            int intQuantity;
            decimal decMoney;

            if (dgvSTCheckInfo.RowCount == 0) return;

            strStoreCode = this.dgvSTCheckInfo["StoreCode", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();
            strInvenCode = this.dgvSTCheckInfo["InvenCode", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();
            strSTCheckCode = this.dgvSTCheckInfo["STCheckCode", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();
            strIsFlag = this.dgvSTCheckInfo["IsFlag", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value.ToString();

            intBalQuantity = Convert.ToInt32(this.dgvSTCheckInfo["BalQuantity", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value);
            decBalMoney = Convert.ToDecimal(this.dgvSTCheckInfo["BalMoney", this.dgvSTCheckInfo.CurrentCell.RowIndex].Value);

            if (strIsFlag == "0")
            {
                MessageBox.Show("该单据未审核，无需弃审时！", "软件提示");
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

                intQuantity = sdr.GetInt32(0) - intBalQuantity;
                decMoney = sdr.GetDecimal(2) - decBalMoney;

                if (intQuantity < 0 || decMoney < 0)
                {
                    MessageBox.Show("库存数据异常(弃审后库存数量或库存金额将变为负数)，无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }
                sdr.Close();

                List<(string, SqlParameter[])> commands = new List<(string, SqlParameter[])>();

                string stockSql = "UPDATE STStock SET Quantity = @Quantity, STMoney = @STMoney WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
                SqlParameter[] stockPars = {
            new SqlParameter("@Quantity", intQuantity),
            new SqlParameter("@STMoney", decMoney),
            new SqlParameter("@StoreCode", strStoreCode),
            new SqlParameter("@InvenCode", strInvenCode)
        };
                commands.Add((stockSql, stockPars));

                string checkSql = "UPDATE STCheck SET IsFlag = '0' WHERE STCheckCode = @STCheckCode";
                commands.Add((checkSql, new SqlParameter[] { new SqlParameter("@STCheckCode", strSTCheckCode) }));

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

                    strWhere = " WHERE STCheckCode LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "单据日期":

                    strWhere = " WHERE SUBSTRING(CONVERT(VARCHAR(20),STCheckDate,20),1,10) LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                default:
                    break;
            }
        }

        private void dgvSTCheckInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
