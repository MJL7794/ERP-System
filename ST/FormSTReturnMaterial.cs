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
    public partial class FormSTReturnMaterial : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();
        public int intMaxReturnQuantity;

        public FormSTReturnMaterial()
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
            this.txtQuantity.ReadOnly = !this.txtQuantity.ReadOnly;
            this.cbxEmployeeCode.Enabled = !this.cbxEmployeeCode.Enabled;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            this.txtSTGetCode.Text = "";
            this.dtpSTGetDate.Value = Convert.ToDateTime("1900-01-01");
            this.cbxOperatorCode.SelectedIndex = -1;
            this.txtPRProduceCode.Text = "";
            this.cbxStoreCode.SelectedIndex = -1;
            this.cbxInvenCode.SelectedIndex = -1;
            this.txtQuantity.Text = "";
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
            this.txtSTGetCode.Text = this.dgvSTGetMaterialInfo["STGetCode", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value.ToString();
            this.dtpSTGetDate.Value = Convert.ToDateTime(this.dgvSTGetMaterialInfo["STGetDate", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value);
            this.cbxOperatorCode.SelectedValue = this.dgvSTGetMaterialInfo["OperatorCode", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value;
            this.txtPRProduceCode.Text = this.dgvSTGetMaterialInfo["PRProduceCode", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxStoreCode.SelectedValue = this.dgvSTGetMaterialInfo["StoreCode", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value;
            this.cbxInvenCode.SelectedValue = this.dgvSTGetMaterialInfo["InvenCode", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value;
            this.txtQuantity.Text = this.dgvSTGetMaterialInfo["Quantity", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxEmployeeCode.SelectedValue = this.dgvSTGetMaterialInfo["EmployeeCode", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value;
            this.cbxIsFlag.SelectedValue = this.dgvSTGetMaterialInfo["IsFlag", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value;
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM STGetMaterial " + strWhere; ;

            try
            {
                this.dgvSTGetMaterialInfo.DataSource = db.GetDataSet(strSql, "STGetMaterial").Tables["STGetMaterial"];
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

            pars.Add(new SqlParameter("@STGetCode", txtSTGetCode.Text.Trim()));
            pars.Add(new SqlParameter("@STGetDate", dtpSTGetDate.Value));

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

            if (String.IsNullOrEmpty(txtQuantity.Text.Trim()))
                pars.Add(new SqlParameter("@Quantity", 0));
            else
                pars.Add(new SqlParameter("@Quantity", Convert.ToInt32(txtQuantity.Text.Trim())));

            pars.Add(new SqlParameter("@BillType", "2")); // 退料单

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


        private void FormSTReturnMaterial_Load(object sender, EventArgs e)
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
            commUse.BindComboBox(this.dgvSTGetMaterialInfo.Columns["OperatorCode"], "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(this.dgvSTGetMaterialInfo.Columns["StoreCode"], "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(this.dgvSTGetMaterialInfo.Columns["InvenCode"], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(this.dgvSTGetMaterialInfo.Columns["EmployeeCode"], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            commUse.BindComboBox(this.dgvSTGetMaterialInfo.Columns["IsFlag"], "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
            
            //
            BindDataGridView("Where BillType = '2'");
            this.BindToolStripComboBox();
            this.cbxCondition.SelectedIndex = 0;
            toolStrip1.Tag = "";
        }

        private void toolAdd_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "ADD"; //添加标识

            dtpSTGetDate.Value = DateTime.Today;
            cbxOperatorCode.SelectedValue = PropertyClass.OperatorCode;
            cbxIsFlag.SelectedValue = "0";

            txtSTGetCode.Text = commUse.BuildBillCode("STGetMaterial", "STGetCode", "STGetDate", dtpSTGetDate.Value);
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //修改标识 
        }

        private void dgvSTGetMaterialInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT" && dgvSTGetMaterialInfo.RowCount > 0)
            {
                if (this.dgvSTGetMaterialInfo["IsFlag", this.dgvSTGetMaterialInfo.CurrentRow.Index].Value.ToString() == "1")
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

        private void btnChoice_Click(object sender, EventArgs e)
        {
            FormSTReturnBrowseProduce formSTReturnBrowseProduce = new FormSTReturnBrowseProduce();
            formSTReturnBrowseProduce.Owner = this;
            formSTReturnBrowseProduce.ShowDialog();
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputInteger(e);
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            string strCode = null;

            if (String.IsNullOrEmpty(txtSTGetCode.Text.Trim()))
            {
                MessageBox.Show("单据编号不许为空！", "软件提示");
                txtSTGetCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtPRProduceCode.Text.Trim()))
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
                else
                {
                    if (Convert.ToInt32(txtQuantity.Text.Trim()) > this.intMaxReturnQuantity)
                    {
                        MessageBox.Show("本次退料量不许大于最大可退料量" + this.intMaxReturnQuantity.ToString(), "软件提示");
                        txtQuantity.Focus();
                        return;
                    }
                }
            }

            //添加
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                try
                {
                    strCode = "INSERT INTO STGetMaterial(STGetCode,STGetDate,OperatorCode,PRProduceCode,StoreCode,InvenCode,Quantity,BillType,EmployeeCode,IsFlag) ";
                    strCode += "VALUES(@STGetCode,@STGetDate,@OperatorCode,@PRProduceCode,@StoreCode,@InvenCode,@Quantity,@BillType,@EmployeeCode,@IsFlag)";

                    SqlParameter[] pars = ParametersAddValue();

                    if (db.ExecDataBySql(strCode, pars) > 0)
                    {
                        MessageBox.Show("保存成功！", "软件提示");
                        this.BindDataGridView(" Where BillType = '2'");
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

            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                string strSTGetCode = txtSTGetCode.Text.Trim();

                try
                {
                    strCode = "UPDATE STGetMaterial SET STGetCode = @STGetCode,STGetDate = @STGetDate,OperatorCode = @OperatorCode,PRProduceCode = @PRProduceCode,StoreCode = @StoreCode,";
                    strCode += "InvenCode = @InvenCode,Quantity = @Quantity,";
                    strCode += "EmployeeCode = @EmployeeCode,IsFlag = @IsFlag";
                    strCode += " WHERE STGetCode = @OldSTGetCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldSTGetCode", strSTGetCode));

                    if (db.ExecDataBySql(strCode, parsList.ToArray()) > 0)
                    {
                        MessageBox.Show("保存成功！", "软件提示");
                        this.BindDataGridView(" Where BillType = '2'");
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

            this.BindDataGridView(" Where BillType = '2' ");
            toolStrip1.Tag = "";
        }

        private void toolDelete_Click(object sender, EventArgs e)
        {
            string strSTGetCode = null; //单据编号
            string strSql = null;
            string strFlag = null; //审核标记

            if (this.dgvSTGetMaterialInfo.RowCount <= 0)
            {
                return;
            }

            strSTGetCode = this.dgvSTGetMaterialInfo["STGetCode", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value.ToString();
            strFlag = this.dgvSTGetMaterialInfo["IsFlag", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value.ToString();

            if (strFlag == "1")
            {
                MessageBox.Show("该单据已审核，不许删除！", "软件提示");
                return;
            }

            strSql = "DELETE FROM STGetMaterial WHERE STGetCode = @STGetCode";
            SqlParameter[] pars = { new SqlParameter("@STGetCode", strSTGetCode) };

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

                this.BindDataGridView(" Where BillType = '2'");
            }
        }

        private void txtOK_Click(object sender, EventArgs e)
        {
            string strWhere = String.Empty;
            string strConditonName = String.Empty;

            strConditonName = this.cbxCondition.Items[this.cbxCondition.SelectedIndex].ToString();
            switch (strConditonName)
            {
                case "单据编号":

                    strWhere = " WHERE BillType = '2' AND STGetCode LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "单据日期":

                    strWhere = " WHERE BillType = '2' AND SUBSTRING(CONVERT(VARCHAR(20),STGetDate,20),1,10) LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                default:
                    break;
            }
        }

        private void toolCheck_Click(object sender, EventArgs e)
        {
            SqlDataReader sdr = null;
            string strCode = null;

            string strSTGetCode = null;
            string strPRProduceCode = null;
            string strStoreCode = null;
            string strInvenCode = null;
            string strIsFlag = null;

            int intQuantity;
            decimal decUnitPrice;
            decimal decAvePrice;
            decimal decMoney;
            int intGetQuantity;
            int intStockQuantity;

            strSTGetCode = this.dgvSTGetMaterialInfo["STGetCode", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value.ToString();
            strPRProduceCode = this.dgvSTGetMaterialInfo["PRProduceCode", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value.ToString();
            strStoreCode = this.dgvSTGetMaterialInfo["StoreCode", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value.ToString();
            strInvenCode = this.dgvSTGetMaterialInfo["InvenCode", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value.ToString();
            strIsFlag = this.dgvSTGetMaterialInfo["IsFlag", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value.ToString();

            intQuantity = Convert.ToInt32(this.dgvSTGetMaterialInfo["Quantity", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value);

            if (strIsFlag == "1")
            {
                MessageBox.Show("该单据已审核，不许再次审核！", "软件提示");
                return;
            }

            // ✅ 参数化查询库存
            strCode = "SELECT Quantity,AvePrice,STMoney FROM STStock WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
            SqlParameter[] stockPars = {
        new SqlParameter("@StoreCode", strStoreCode),
        new SqlParameter("@InvenCode", strInvenCode)
    };

            try
            {
                sdr = db.GetDataReader(strCode, stockPars);
                sdr.Read();

                if (!sdr.HasRows)
                {
                    MessageBox.Show("该存货的库存未初始化，无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }

                decUnitPrice = commUse.GetAvePriceBySTGetMaterial(strPRProduceCode, strInvenCode);
                decMoney = sdr.GetDecimal(2) + Decimal.Round(decUnitPrice * intQuantity, 2);
                intStockQuantity = sdr.GetInt32(0) + intQuantity;
                decAvePrice = Decimal.Round(decMoney / intStockQuantity, 2);
                sdr.Close();

                // ✅ 参数化查询生产单子表
                strCode = "SELECT GetQuantity FROM PRProduceItem WHERE PRProduceCode = @PRProduceCode AND InvenCode = @InvenCode";
                SqlParameter[] producePars = {
            new SqlParameter("@PRProduceCode", strPRProduceCode),
            new SqlParameter("@InvenCode", strInvenCode)
        };
                sdr = db.GetDataReader(strCode, producePars);
                sdr.Read();

                if (!sdr.HasRows)
                {
                    MessageBox.Show("生产单数据异常，无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }

                if (sdr.GetInt32(0) < intQuantity)
                {
                    MessageBox.Show("生产单数据异常（现存退料量小于本次退料量），无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }

                intGetQuantity = sdr.GetInt32(0) - intQuantity;
                sdr.Close();

                // ✅ 事务参数化
                List<(string, SqlParameter[])> commands = new List<(string, SqlParameter[])>();

                // 库存表
                string stockSql = "UPDATE STStock SET Quantity = @Quantity, STMoney = @STMoney, AvePrice = @AvePrice WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
                SqlParameter[] stockUpdatePars = {
            new SqlParameter("@Quantity", intStockQuantity),
            new SqlParameter("@STMoney", decMoney),
            new SqlParameter("@AvePrice", decAvePrice),
            new SqlParameter("@StoreCode", strStoreCode),
            new SqlParameter("@InvenCode", strInvenCode)
        };
                commands.Add((stockSql, stockUpdatePars));

                // 生产单子表
                string produceSql = "UPDATE PRProduceItem SET GetQuantity = @GetQuantity WHERE PRProduceCode = @PRProduceCode AND InvenCode = @InvenCode";
                SqlParameter[] produceUpdatePars = {
            new SqlParameter("@GetQuantity", intGetQuantity),
            new SqlParameter("@PRProduceCode", strPRProduceCode),
            new SqlParameter("@InvenCode", strInvenCode)
        };
                commands.Add((produceSql, produceUpdatePars));

                // 退料表
                string getSql = "UPDATE STGetMaterial SET UnitPrice = @UnitPrice, IsFlag = '1' WHERE STGetCode = @STGetCode";
                SqlParameter[] getPars = {
            new SqlParameter("@UnitPrice", decUnitPrice),
            new SqlParameter("@STGetCode", strSTGetCode)
        };
                commands.Add((getSql, getPars));

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

            this.BindDataGridView(" Where BillType = '2'");
        }

        private void toolUnCheck_Click(object sender, EventArgs e)
        {
            SqlDataReader sdr = null;
            string strCode = null;

            string strSTGetCode = null;
            string strPRProduceCode = null;
            string strStoreCode = null;
            string strInvenCode = null;
            string strIsFlag = null;

            int intQuantity;
            decimal decMoney;
            decimal decUnitPrice;
            int intGetQuantity;
            int intStockQuantity;

            strSTGetCode = this.dgvSTGetMaterialInfo["STGetCode", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value.ToString();
            strPRProduceCode = this.dgvSTGetMaterialInfo["PRProduceCode", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value.ToString();
            strStoreCode = this.dgvSTGetMaterialInfo["StoreCode", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value.ToString();
            strInvenCode = this.dgvSTGetMaterialInfo["InvenCode", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value.ToString();
            strIsFlag = this.dgvSTGetMaterialInfo["IsFlag", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value.ToString();

            intQuantity = Convert.ToInt32(this.dgvSTGetMaterialInfo["Quantity", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value);
            decUnitPrice = Convert.ToDecimal(this.dgvSTGetMaterialInfo["UnitPrice", this.dgvSTGetMaterialInfo.CurrentCell.RowIndex].Value);

            if (strIsFlag == "0")
            {
                MessageBox.Show("该单据未审核，无需弃审！", "软件提示");
                return;
            }

            // ✅ 参数化查询库存
            strCode = "SELECT Quantity,AvePrice,STMoney FROM STStock WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
            SqlParameter[] stockPars = {
        new SqlParameter("@StoreCode", strStoreCode),
        new SqlParameter("@InvenCode", strInvenCode)
    };

            try
            {
                sdr = db.GetDataReader(strCode, stockPars);
                sdr.Read();

                if (!sdr.HasRows)
                {
                    MessageBox.Show("库存数据异常，无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }

                if (sdr.GetInt32(0) < intQuantity)
                {
                    MessageBox.Show("库存数据异常（库存量小于退料的弃审量），无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }

                decMoney = sdr.GetDecimal(2) - Decimal.Round(intQuantity * decUnitPrice, 2);
                intStockQuantity = sdr.GetInt32(0) - intQuantity;
                sdr.Close();

                // ✅ 参数化查询生产单子表
                strCode = "SELECT GetQuantity FROM PRProduceItem WHERE PRProduceCode = @PRProduceCode AND InvenCode = @InvenCode";
                SqlParameter[] producePars = {
            new SqlParameter("@PRProduceCode", strPRProduceCode),
            new SqlParameter("@InvenCode", strInvenCode)
        };
                sdr = db.GetDataReader(strCode, producePars);
                sdr.Read();

                if (!sdr.HasRows)
                {
                    MessageBox.Show("生产单数据异常，无法处理！", "软件提示");
                    sdr.Close();
                    return;
                }

                if (Convert.IsDBNull(sdr.GetValue(0)))
                    intGetQuantity = intQuantity;
                else
                    intGetQuantity = sdr.GetInt32(0) + intQuantity;

                sdr.Close();

                // ✅ 事务参数化
                List<(string, SqlParameter[])> commands = new List<(string, SqlParameter[])>();

                string stockSql = "UPDATE STStock SET Quantity = @Quantity, STMoney = @STMoney WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
                SqlParameter[] stockUpdatePars = {
            new SqlParameter("@Quantity", intStockQuantity),
            new SqlParameter("@STMoney", decMoney),
            new SqlParameter("@StoreCode", strStoreCode),
            new SqlParameter("@InvenCode", strInvenCode)
        };
                commands.Add((stockSql, stockUpdatePars));

                string produceSql = "UPDATE PRProduceItem SET GetQuantity = @GetQuantity WHERE PRProduceCode = @PRProduceCode AND InvenCode = @InvenCode";
                SqlParameter[] produceUpdatePars = {
            new SqlParameter("@GetQuantity", intGetQuantity),
            new SqlParameter("@PRProduceCode", strPRProduceCode),
            new SqlParameter("@InvenCode", strInvenCode)
        };
                commands.Add((produceSql, produceUpdatePars));

                string getSql = "UPDATE STGetMaterial SET IsFlag = '0' WHERE STGetCode = @STGetCode";
                commands.Add((getSql, new SqlParameter[] { new SqlParameter("@STGetCode", strSTGetCode) }));

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

            this.BindDataGridView("Where BillType = '2'");
        }

        private void dgvSTGetMaterialInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
