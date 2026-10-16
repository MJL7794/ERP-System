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

namespace ERP.BS
{
    public partial class FormInitStock : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormInitStock()
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
            //窗体控件状态切换
            this.cbxStoreCode.Enabled = !this.cbxStoreCode.Enabled;
            this.cbxInvenCode.Enabled = !this.cbxInvenCode.Enabled;
            this.txtQuantity.ReadOnly = !this.txtQuantity.ReadOnly;
            this.txtLossQuantity.ReadOnly = !this.txtLossMoney.ReadOnly;
            this.txtAvePrice.ReadOnly = !this.txtAvePrice.ReadOnly;
            this.txtLossMoney.ReadOnly = !this.txtLossMoney.ReadOnly;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            //窗体控件状态切换
            this.cbxStoreCode.SelectedIndex = -1;
            this.cbxInvenCode.SelectedIndex = -1;
            this.txtQuantity.Text = "";
            this.txtLossQuantity.Text = "";
            this.txtAvePrice.Text = "";
            this.txtSTMoney.Text = "";
            this.txtLossMoney.Text = "";
        }

        /// <summary>
        /// 计算库存金额
        /// </summary>
        private void ComputeMoney()
        {
            int int_Quantity;
            decimal dec_AvePrice;

            if (!String.IsNullOrEmpty(txtQuantity.Text.Trim()) && !String.IsNullOrEmpty(txtAvePrice.Text.Trim()))
            {
                int_Quantity = Convert.ToInt32(txtQuantity.Text.Trim());
                dec_AvePrice = Convert.ToDecimal(txtAvePrice.Text.Trim());
                txtSTMoney.Text = Decimal.Round(int_Quantity * dec_AvePrice, 2).ToString();
            }
        }

        /// <summary>
        /// 设置控件的显示值
        /// </summary>
        private void FillControls()
        {
            this.cbxStoreCode.SelectedValue = this.dgvStockInfo[0, this.dgvStockInfo.CurrentCell.RowIndex].Value;
            this.cbxInvenCode.SelectedValue = this.dgvStockInfo[1, this.dgvStockInfo.CurrentCell.RowIndex].Value;
            this.txtQuantity.Text = this.dgvStockInfo[2, this.dgvStockInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtLossQuantity.Text = this.dgvStockInfo[3, this.dgvStockInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtAvePrice.Text = this.dgvStockInfo[4, this.dgvStockInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtSTMoney.Text = this.dgvStockInfo[5, this.dgvStockInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtLossMoney.Text = this.dgvStockInfo[6, this.dgvStockInfo.CurrentCell.RowIndex].Value.ToString();
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT * ";
            strSql += "FROM STStock" + strWhere; ;

            try
            {
                this.dgvStockInfo.DataSource = db.GetDataSet(strSql, "STStock").Tables["STStock"];
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

            if (String.IsNullOrEmpty(txtLossQuantity.Text.Trim()))
                pars.Add(new SqlParameter("@LossQuantity", 0));
            else
                pars.Add(new SqlParameter("@LossQuantity", Convert.ToInt32(txtLossQuantity.Text.Trim())));

            if (String.IsNullOrEmpty(txtAvePrice.Text.Trim()))
                pars.Add(new SqlParameter("@AvePrice", 0));
            else
                pars.Add(new SqlParameter("@AvePrice", Convert.ToDecimal(txtAvePrice.Text.Trim())));

            if (String.IsNullOrEmpty(txtSTMoney.Text.Trim()))
                pars.Add(new SqlParameter("@STMoney", 0));
            else
                pars.Add(new SqlParameter("@STMoney", Convert.ToDecimal(txtSTMoney.Text.Trim())));

            if (String.IsNullOrEmpty(txtLossMoney.Text.Trim()))
                pars.Add(new SqlParameter("@LossMoney", 0));
            else
                pars.Add(new SqlParameter("@LossMoney", Convert.ToDecimal(txtLossMoney.Text.Trim())));

            return pars.ToArray();
        }

        private void FormInitStock_Load(object sender, EventArgs e)
        {
            //权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            //ComboBox绑定到数据源
            commUse.BindComboBox(cbxStoreCode, "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(cbxInvenCode, "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvStockInfo.Columns[0], "StoreCode", "StoreName", "select StoreCode,StoreName from BSStore", "BSStore");
            commUse.BindComboBox(this.dgvStockInfo.Columns[1], "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            BindDataGridView("");
            toolStrip1.Tag = "";
        }

        private void toolAdd_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "ADD"; //添加操作
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //修改操作
        }

        private void toolreflush_Click(object sender, EventArgs e)
        {
            this.BindDataGridView("");
        }

        private void dgvStoreInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                if (dgvStockInfo.RowCount > 0)
                {
                    FillControls();
                }
            }
        }

        private void toolCancel_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "";
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            this.ComputeMoney();
        }

        private void txtLossQuantity_TextChanged(object sender, EventArgs e)
        {
            this.ComputeMoney();
        }

        private void txtAvePrice_TextChanged(object sender, EventArgs e)
        {
            this.ComputeMoney();
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            string strCode = null;
            SqlDataReader sdr = null;
            string strStoreCode = null; //仓库代码
            string strInvenCode = null; //存货代码

            if (cbxStoreCode.SelectedIndex == -1)
            {
                MessageBox.Show("请选择仓库！", "软件提示");
                cbxStoreCode.Focus();
                return;
            }

            if (cbxInvenCode.SelectedIndex == -1)
            {
                MessageBox.Show("请选择存货！", "软件提示");
                cbxInvenCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                MessageBox.Show("库存数量不许为空！", "软件提示"); 
                txtQuantity.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtAvePrice.Text.Trim()))
            {
                MessageBox.Show("成本价不许为空！", "软件提示"); 
                txtQuantity.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtLossQuantity.Text.Trim()))
            {
                MessageBox.Show("损失数量不许为空！", "软件提示"); 
                txtLossQuantity.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtLossMoney.Text.Trim()))
            {
                MessageBox.Show("损失金额不许为空！", "软件提示"); 
                txtLossMoney.Focus();
                return;
            }

            //添加操作
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                strStoreCode = cbxStoreCode.SelectedValue.ToString();
                strInvenCode = cbxInvenCode.SelectedValue.ToString();

                strCode = "SELECT * FROM STStock WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
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
                        sdr.Close();

                        strCode = "INSERT INTO STStock(StoreCode,InvenCode,Quantity,LossQuantity,AvePrice,STMoney,LossMoney) ";
                        strCode += "VALUES(@StoreCode,@InvenCode,@Quantity,@LossQuantity,@AvePrice,@STMoney,@LossMoney)";

                        SqlParameter[] pars = ParametersAddValue();

                        if (db.ExecDataBySql(strCode, pars) > 0)
                        {
                            MessageBox.Show("保存成功！", "软件提示");
                            toolStrip1.Tag = "";
                            this.BindDataGridView("");
                            ControlStatus();
                        }
                        else
                        {
                            MessageBox.Show("保存失败！", "软件提示");
                        }
                    }
                    else
                    {
                        MessageBox.Show("该存货的库存已经被初始化过！", "软件提示");
                        this.cbxInvenCode.Focus();
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
            }
            //修改
            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                string strOldStoreCode = null;
                string strOldInvenCode = null;

                strOldStoreCode = this.dgvStockInfo[0, this.dgvStockInfo.CurrentCell.RowIndex].Value.ToString();
                strOldInvenCode = this.dgvStockInfo[1, this.dgvStockInfo.CurrentCell.RowIndex].Value.ToString();

                strStoreCode = cbxStoreCode.SelectedValue.ToString();
                strInvenCode = cbxInvenCode.SelectedValue.ToString();

                if (strOldStoreCode != cbxStoreCode.SelectedValue.ToString() || strOldInvenCode != cbxInvenCode.SelectedValue.ToString())
                {
                    strCode = "SELECT * FROM STStock WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
                    SqlParameter[] checkPars = {
            new SqlParameter("@StoreCode", strStoreCode),
            new SqlParameter("@InvenCode", strInvenCode)
        };

                    try
                    {
                        sdr = db.GetDataReader(strCode, checkPars);
                        sdr.Read();
                        if (sdr.HasRows)
                        {
                            MessageBox.Show("该存货的库存已经被初始化过！", "软件提示");
                            this.cbxInvenCode.Focus();
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
                }

                //更新数据库
                try
                {
                    strCode = "UPDATE STStock SET StoreCode = @StoreCode,InvenCode = @InvenCode,";
                    strCode += "Quantity = @Quantity,LossQuantity = @LossQuantity,AvePrice = @AvePrice,";
                    strCode += "STMoney = @STMoney,LossMoney = @LossMoney ";
                    strCode += "WHERE StoreCode = @OldStoreCode AND InvenCode = @OldInvenCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldStoreCode", strOldStoreCode));
                    parsList.Add(new SqlParameter("@OldInvenCode", strOldInvenCode));

                    if (db.ExecDataBySql(strCode, parsList.ToArray()) > 0)
                    {
                        MessageBox.Show("保存成功！", "软件提示");
                        toolStrip1.Tag = "";
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
        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolDelete_Click(object sender, EventArgs e)
        {
            string strStoreCode = null;
            string strInvenCode = null;
            string strSql = null;

            if (this.dgvStockInfo.RowCount == 0)
            {
                return;
            }

            strStoreCode = this.dgvStockInfo[0, this.dgvStockInfo.CurrentCell.RowIndex].Value.ToString();
            strInvenCode = this.dgvStockInfo[1, this.dgvStockInfo.CurrentCell.RowIndex].Value.ToString();

            strSql = "DELETE FROM STStock WHERE StoreCode = @StoreCode AND InvenCode = @InvenCode";
            SqlParameter[] pars = {
        new SqlParameter("@StoreCode", strStoreCode),
        new SqlParameter("@InvenCode", strInvenCode)
    };

            if (MessageBox.Show("确定要删除吗？", "软件提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                try
                {
                    if (db.ExecDataBySql(strSql, pars) > 0)
                    {
                        MessageBox.Show("删除成功！", "软件提示");
                    }
                    else
                    {
                        MessageBox.Show("删除失败！", "软件提示");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "软件提示");
                    throw;
                }

                this.BindDataGridView("");
            }
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputInteger(e);
        }

        private void txtAvePrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputNumeric(e, sender as Control);
        }

    }
}
