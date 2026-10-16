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

namespace ERP.PR
{
    public partial class FormPRInStore : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormPRInStore()
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
            this.btnChoice.Enabled = !this.btnChoice.Enabled;
            this.cbxStoreCode.Enabled = !this.cbxStoreCode.Enabled;
            this.txtInQuantity.ReadOnly = !this.txtInQuantity.ReadOnly;
            this.cbxEmployeeCode.Enabled = !this.cbxEmployeeCode.Enabled;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            this.txtPRInCode.Text = "";
            this.dtpPRInDate.Value = Convert.ToDateTime("1900-01-01");
            this.cbxOperatorCode.SelectedIndex = -1;
            this.txtPRProduceCode.Text = "";
            this.cbxStoreCode.SelectedIndex = -1;
            this.cbxInvenCode.SelectedIndex = -1;
            this.txtPRQuantity.Text = "";
            this.txtInQuantity.Text = "";
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
            this.txtPRInCode.Text = this.dgvPRInStoreInfo["PRInCode", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            this.dtpPRInDate.Value = Convert.ToDateTime(this.dgvPRInStoreInfo["PRInDate", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value);
            this.cbxOperatorCode.SelectedValue = this.dgvPRInStoreInfo["OperatorCode", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value;
            this.txtPRProduceCode.Text = this.dgvPRInStoreInfo["PRProduceCode", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxStoreCode.SelectedValue = this.dgvPRInStoreInfo["StoreCode", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value;
            this.cbxInvenCode.SelectedValue = this.dgvPRInStoreInfo["InvenCode", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value;
            this.txtPRQuantity.Text = this.dgvPRInStoreInfo["PRQuantity", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtInQuantity.Text = this.dgvPRInStoreInfo["InQuantity", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxEmployeeCode.SelectedValue = this.dgvPRInStoreInfo["EmployeeCode", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value;
            this.cbxIsFlag.SelectedValue = this.dgvPRInStoreInfo["IsFlag", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value;
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM PRInStore " + strWhere; ;

            try
            {
                this.dgvPRInStoreInfo.DataSource = db.GetDataSet(strSql, "PRInStore").Tables["PRInStore"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw ex;
            }
        }

        /// <summary>
        /// 计算产品的成本
        /// </summary>
        /// <param name="strPRProduceCode">生产单编号</param>
        /// <returns>产品的成本</returns>
        private decimal ComputeMaterialCost(string strPRProduceCode)
        {
            string strSql = null;
            DataTable dt = null;
            decimal decAvePrice;
            decimal decMaterialCost = 0;

            strSql = "SELECT InvenCode,UseQuantity FROM PRProduceItem WHERE PRProduceCode = @PRProduceCode";
            SqlParameter[] pars = { new SqlParameter("@PRProduceCode", strPRProduceCode) };

            try
            {
                dt = db.GetDataTable(strSql, "PRProduceItem", pars);

                foreach (DataRow dr in dt.Rows)
                {
                    decAvePrice = commUse.GetAvePriceBySTGetMaterial(strPRProduceCode, dr["InvenCode"].ToString());
                    decMaterialCost += (decAvePrice * Convert.ToInt32(dr["UseQuantity"]));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw;
            }

            return decMaterialCost;
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        private SqlParameter[] ParametersAddValue()
        {
            List<SqlParameter> pars = new List<SqlParameter>();

            pars.Add(new SqlParameter("@PRInCode", txtPRInCode.Text.Trim()));
            pars.Add(new SqlParameter("@PRInDate", dtpPRInDate.Value));

            if (cbxOperatorCode.SelectedValue == null)
                pars.Add(new SqlParameter("@OperatorCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@OperatorCode", cbxOperatorCode.SelectedValue.ToString()));

            pars.Add(new SqlParameter("@PRProduceCode", txtPRProduceCode.Text.Trim()));

            if (cbxStoreCode.SelectedValue == null)
                pars.Add(new SqlParameter("@StoreCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@StoreCode", cbxStoreCode.SelectedValue.ToString()));

            if (cbxInvenCode.SelectedValue == null)
                pars.Add(new SqlParameter("@InvenCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@InvenCode", cbxInvenCode.SelectedValue.ToString()));

            if (String.IsNullOrEmpty(txtPRQuantity.Text.Trim()))
                pars.Add(new SqlParameter("@PRQuantity", 0));
            else
                pars.Add(new SqlParameter("@PRQuantity", Convert.ToInt32(txtPRQuantity.Text.Trim())));

            if (String.IsNullOrEmpty(txtInQuantity.Text.Trim()))
                pars.Add(new SqlParameter("@InQuantity", 0));
            else
                pars.Add(new SqlParameter("@InQuantity", Convert.ToInt32(txtInQuantity.Text.Trim())));

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

        private void FormPRInStore_Load(object sender, EventArgs e)
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
            commUse.BindComboBox(cbxInvenCode, "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(cbxEmployeeCode, "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(cbxIsFlag, "Code", "Name", "select * from INCheckFlag", "INCheckFlag");

            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvPRInStoreInfo.Columns["OperatorCode"], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvPRInStoreInfo.Columns["StoreCode"], "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(this.dgvPRInStoreInfo.Columns["InvenCode"], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvPRInStoreInfo.Columns["EmployeeCode"], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvPRInStoreInfo.Columns["IsFlag"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
            
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
            toolStrip1.Tag = "ADD"; //添加标识

            dtpPRInDate.Value = DateTime.Today;
            cbxOperatorCode.SelectedValue = PropertyClass.OperatorCode;
            cbxIsFlag.SelectedValue = "0";

            txtPRInCode.Text = commUse.BuildBillCode("PRInStore", "PRInCode", "PRInDate", dtpPRInDate.Value);
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //修改标识
        }

        private void dgvPRInStoreInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT" && dgvPRInStoreInfo.RowCount > 0)
            {
                if (this.dgvPRInStoreInfo["IsFlag", this.dgvPRInStoreInfo.CurrentRow.Index].Value.ToString() == "1")
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

        private void txtInQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputInteger(e);
        }

        private void btnChoice_Click(object sender, EventArgs e)
        {
            FormBrowsePRProduce formBrowsePRProduce = new FormBrowsePRProduce();
            formBrowsePRProduce.Owner = this;
            formBrowsePRProduce.ShowDialog();
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            string strCode = null;

            if (String.IsNullOrEmpty(txtPRInCode.Text.Trim()))
            {
                MessageBox.Show("单据编号不许为空！", "软件提示");
                txtPRInCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(this.txtPRProduceCode.Text.Trim()))
            {
                MessageBox.Show("生产单号不许为空！", "软件提示");
                txtPRProduceCode.Focus();
                return;
            }

            if (cbxStoreCode.SelectedValue == null)
            {
                MessageBox.Show("仓库不许为空！", "软件提示");
                cbxStoreCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtInQuantity.Text.Trim()))
            {
                MessageBox.Show("入库数量不许为空！", "软件提示");
                txtInQuantity.Focus();
                return;
            }
            else
            {
                if (Convert.ToInt32(txtInQuantity.Text.Trim()) == 0)
                {
                    MessageBox.Show("入库数量不能等于零", "软件提示");
                    txtInQuantity.Focus();
                    return;
                }

                if (Convert.ToInt32(txtInQuantity.Text.Trim()) > Convert.ToInt32(txtPRQuantity.Text.Trim()))
                {
                    MessageBox.Show("入库数量不许大于生产数量", "软件提示");
                    txtInQuantity.Focus();
                    return;
                }
            }

            //添加
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                try
                {
                    strCode = "INSERT INTO PRInStore(PRInCode,PRInDate,OperatorCode,PRProduceCode,StoreCode,InvenCode,PRQuantity,InQuantity,EmployeeCode,IsFlag) ";
                    strCode += "VALUES(@PRInCode,@PRInDate,@OperatorCode,@PRProduceCode,@StoreCode,@InvenCode,@PRQuantity,@InQuantity,@EmployeeCode,@IsFlag)";

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
                string strPRInCode = txtPRInCode.Text.Trim();

                try
                {
                    strCode = "UPDATE PRInStore SET PRProduceCode = @PRProduceCode,StoreCode = @StoreCode,";
                    strCode += "InvenCode = @InvenCode,PRQuantity = @PRQuantity,InQuantity = @InQuantity,";
                    strCode += "EmployeeCode = @EmployeeCode ";
                    strCode += "WHERE PRInCode = @OldPRInCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldPRInCode", strPRInCode));

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
            string strPRInCode = null; //单据编号
            string strSql = null;
            string strFlag = null;

            if (this.dgvPRInStoreInfo.RowCount <= 0)
            {
                return;
            }

            strPRInCode = this.dgvPRInStoreInfo["PRInCode", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            strFlag = this.dgvPRInStoreInfo["IsFlag", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();

            if (strFlag == "1")
            {
                MessageBox.Show("该单据已审核，不许删除！", "软件提示");
                return;
            }

            strSql = "DELETE FROM PRInStore WHERE PRInCode = @PRInCode";
            SqlParameter[] pars = { new SqlParameter("@PRInCode", strPRInCode) };

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
            List<string> strSqls = new List<string>();
            DataTable dt = null;
            string strCode = null;
            string strPRInCode = null;
            string strPRProduceCode = null;
            string strIsFlag = null;

            string strPRInStoreSql = null;
            string strSTStockSql = null;

            string strStoreCode = null;
            string strInvenCode = null;

            int intInQuantity;
            int intQuantity;
            decimal decPrice;
            decimal decMoney;
            decimal decMaterialCost;

            if (dgvPRInStoreInfo.RowCount <= 0) return;

            strStoreCode = this.dgvPRInStoreInfo["StoreCode", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            strInvenCode = this.dgvPRInStoreInfo["InvenCode", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            strPRInCode = this.dgvPRInStoreInfo["PRInCode", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            strIsFlag = this.dgvPRInStoreInfo["IsFlag", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            intInQuantity = Convert.ToInt32(this.dgvPRInStoreInfo["InQuantity", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value);
            strPRProduceCode = this.dgvPRInStoreInfo["PRProduceCode", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();

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
                dt = db.GetDataTable(strCode, "STStock", checkPars);

                if (dt.Rows.Count > 0)
                {
                    decMaterialCost = ComputeMaterialCost(strPRProduceCode);
                    intQuantity = intInQuantity + Convert.ToInt32(dt.Rows[0]["Quantity"]);
                    decMoney = decMaterialCost + Convert.ToDecimal(dt.Rows[0]["STMoney"]);
                    decPrice = Decimal.Round(decMoney / intQuantity, 2);

                    strSTStockSql = "UPDATE STStock SET Quantity = " + intQuantity + ",AvePrice = " + decPrice + ",STMoney = " + decMoney + " ";
                    strSTStockSql += "WHERE StoreCode = '" + strStoreCode + "' AND InvenCode = '" + strInvenCode + "'";
                }
                else
                {
                    decMaterialCost = ComputeMaterialCost(strPRProduceCode);
                    intQuantity = intInQuantity;
                    decMoney = decMaterialCost;
                    decPrice = Decimal.Round(decMoney / intQuantity, 2);

                    strSTStockSql = "INSERT INTO STStock(StoreCode,InvenCode,Quantity,AvePrice,STMoney) ";
                    strSTStockSql += "VALUES('" + strStoreCode + "','" + strInvenCode + "'," + intQuantity + "," + decPrice + "," + decMoney + ")";
                }

                strPRInStoreSql = "UPDATE PRInStore SET IsFlag = '1' WHERE PRInCode = '" + strPRInCode + "'";

                strSqls.Add(strSTStockSql);
                strSqls.Add(strPRInStoreSql);

                if (db.ExecDataBySqls(strSqls))
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
            List<string> strSqls = new List<string>();
            DataTable dt = null;
            string strCode = null;
            string strPRInCode = null;          // 单据的编号
            string strIsFlag = null;            // 审核标记
            string strPRProduceCode = null;     // 生产单号

            string strPRInStoreSql = null;      // 提交 PRInStore 表的 SQL
            string strSTStockSql = null;        // 提交 STStock 表的 SQL

            string strStoreCode = null;         // 仓库代码
            string strInvenCode = null;         // 产品代码

            int intQuantity;                    // 产品的库存数量
            int intInQuantity;                  // 入库数量
            decimal decPrice;                   // 库存单价
            decimal decMoney;                   // 库存金额
            decimal decMaterialCost;            // 产品的原材料成本

            if (dgvPRInStoreInfo.RowCount <= 0)
            {
                return;
            }

            // 从选中的行里取值
            strStoreCode = this.dgvPRInStoreInfo["StoreCode", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            strInvenCode = this.dgvPRInStoreInfo["InvenCode", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            strPRInCode = this.dgvPRInStoreInfo["PRInCode", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            strIsFlag = this.dgvPRInStoreInfo["IsFlag", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();
            intInQuantity = Convert.ToInt32(this.dgvPRInStoreInfo["InQuantity", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value);
            strPRProduceCode = this.dgvPRInStoreInfo["PRProduceCode", this.dgvPRInStoreInfo.CurrentCell.RowIndex].Value.ToString();

            if (strIsFlag == "0")
            {
                MessageBox.Show("该单据未审核，无需弃审！", "软件提示");
                return;
            }

            // ✅ 参数化查询库存
            strCode = "SELECT Quantity,AvePrice,STMoney FROM STStock WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
            SqlParameter[] checkPars = {
        new SqlParameter("@StoreCode", strStoreCode),
        new SqlParameter("@InvenCode", strInvenCode)
    };

            try
            {
                dt = db.GetDataTable(strCode, "STStock", checkPars);

                if (dt.Rows.Count > 0)
                {
                    // 库存不够，无法弃审
                    if (Convert.ToInt32(dt.Rows[0]["Quantity"]) < intInQuantity)
                    {
                        MessageBox.Show("该笔入库的产品，已经发生了相关业务，无法弃审！", "软件提示");
                        return;
                    }

                    // 计算产品的原材料直接成本
                    decMaterialCost = ComputeMaterialCost(strPRProduceCode);

                    // 计算库存所需要的数据
                    intQuantity = Convert.ToInt32(dt.Rows[0]["Quantity"]) - intInQuantity;
                    decMoney = Convert.ToDecimal(dt.Rows[0]["STMoney"]) - decMaterialCost;

                    // 计算单价
                    if (intQuantity == 0)
                    {
                        decPrice = 0;
                        decMoney = 0;
                    }
                    else
                    {
                        decPrice = Decimal.Round(decMoney / intQuantity, 2);
                    }

                    // 更新 STStock
                    strSTStockSql = "UPDATE STStock SET Quantity = " + intQuantity + ",AvePrice = " + decPrice + ",STMoney = " + decMoney + " ";
                    strSTStockSql += "WHERE StoreCode = '" + strStoreCode + "' AND InvenCode = '" + strInvenCode + "'";
                }
                else
                {
                    MessageBox.Show("库存数据异常，无法处理！", "软件提示");
                    return;
                }

                // 打弃审标记
                strPRInStoreSql = "UPDATE PRInStore SET IsFlag = '0' WHERE PRInCode = '" + strPRInCode + "'";

                strSqls.Add(strSTStockSql);
                strSqls.Add(strPRInStoreSql);

                // 执行事务
                if (db.ExecDataBySqls(strSqls))
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
                throw;  // ✅ 保留原始堆栈
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

                    strWhere = " WHERE PRInCode LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "单据日期":

                    strWhere = " WHERE SUBSTRING(CONVERT(VARCHAR(20),PRInDate,20),1,10) LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                default:
                    break;
            }
        }

        private void dgvPRInStoreInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
