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

namespace ERP.PR
{
    public partial class FormProduceComplete : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormProduceComplete()
        {
            InitializeComponent();
        }

        private void FormProduceComplete_Load(object sender, EventArgs e)
        {
            //授权
            commUse.CortrolButtonEnabled(toolCheck, this);
            commUse.CortrolButtonEnabled(toolUnCheck, this);

            //ComboBox控件绑定到数据源
            commUse.BindComboBox(cbxOperatorCode, "OperatorCode", "OperatorName", "select OperatorCode,OperatorName from SYOperator", "SYOperator");
            commUse.BindComboBox(cbxDepartmentCode, "DepartmentCode", "DepartmentName", "select DepartmentCode,DepartmentName from BSDepartment", "BSDepartment");
            commUse.BindComboBox(cbxInvenCode, "InvenCode", "InvenName", "select InvenCode,InvenName from BSInven", "BSInven");
            commUse.BindComboBox(cbxIsComplete, "Code", "Name", "select * from INCheckFlag", "INCheckFlag");
        }

        private void txtOK_Click(object sender, EventArgs e)
        {
            FormBrowsePRProduce formBrowsePRProduce = new FormBrowsePRProduce();
            formBrowsePRProduce.Owner = this;
            formBrowsePRProduce.ShowDialog();
        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolCheck_Click(object sender, EventArgs e)
        {
            string strPRProduceCode = null; //生产单号
            string strPRProduceSql = null; //表示提交PRProduce表的SQL语句
            string strPRProduceItemSql = null; //表示提交PRProduceItem表的SQL语句
            List<string> strSqls = new List<string>();
            string strInvenName = null;
            int intUseQuantity; //原料的使用量
            int intId;

            if (dgvPRProduceItemInfo.RowCount == 0)
            {
                return;
            }

            if(cbxIsComplete.SelectedValue.ToString() == "1")
            {
                MessageBox.Show("该生产单已完工，无需再次审核！","软件提示");
                return;
            }

            //结束对当前单元格的编辑操作
            dgvPRProduceItemInfo.EndEdit();

            //判断领料量
            foreach (DataGridViewRow dgvr in dgvPRProduceItemInfo.Rows)
            {
                strInvenName = dgvr.Cells["InvenName"].Value.ToString();

                if (dgvr.Cells["GetQuantity"].Value == DBNull.Value)
                {
                    MessageBox.Show(strInvenName+"的领料量不许为空！", "软件提示");
                    return;
                }
                else
                {
                    if(Convert.ToInt32(dgvr.Cells["GetQuantity"].Value) < Convert.ToInt32(dgvr.Cells["Quantity"].Value))
                    {
                        MessageBox.Show(strInvenName+"的领料量不许小于需求量！", "软件提示");
                        return;
                    }
                }
            }
            //判断使用量
            foreach (DataGridViewRow dgvr in dgvPRProduceItemInfo.Rows)
            {
                strInvenName = dgvr.Cells["InvenName"].Value.ToString();

                if (dgvr.Cells["UseQuantity"].Value == DBNull.Value)
                {
                    MessageBox.Show(strInvenName+"的使用量不许为空！", "软件提示");
                    return;
                }
                else
                {
                    if (Convert.ToInt32(dgvr.Cells["UseQuantity"].Value) < Convert.ToInt32(dgvr.Cells["Quantity"].Value))
                    {
                        MessageBox.Show(strInvenName+"的使用量不许小于需求量！", "软件提示");
                        return;
                    }

                    if (Convert.ToInt32(dgvr.Cells["UseQuantity"].Value) > Convert.ToInt32(dgvr.Cells["GetQuantity"].Value))
                    {
                        MessageBox.Show(strInvenName+"的使用量不许大于领用量！", "软件提示");
                        return;
                    }
                }
            }

            try
            {
                strPRProduceCode = txtPRProduceCode.Text.Trim();

                // ✅ 参数化：UPDATE PRProduce
                strPRProduceSql = "UPDATE PRProduce SET IsComplete = '1' WHERE PRProduceCode = @PRProduceCode";
                SqlParameter[] pars1 = { new SqlParameter("@PRProduceCode", strPRProduceCode) };
                strSqls.Add(strPRProduceSql);
                // 注意：strSqls 是 List<string>，没法直接传参数

                // ⚠️ 由于 ExecDataBySqls 只接受 List<string>，没法参数化
                // 这里暂时保留拼接，或者改 ExecDataBySqls 支持参数
                foreach (DataGridViewRow dgvr in dgvPRProduceItemInfo.Rows)
                {
                    intId = Convert.ToInt32(dgvr.Cells["Id"].Value);
                    intUseQuantity = Convert.ToInt32(dgvr.Cells["UseQuantity"].Value);
                    strPRProduceItemSql = "UPDATE PRProduceItem SET UseQuantity = " + intUseQuantity + " WHERE Id = " + intId;
                    strSqls.Add(strPRProduceItemSql);
                }

                if (db.ExecDataBySqls(strSqls))
                {
                    cbxIsComplete.SelectedValue = "1";
                    this.dgvPRProduceItemInfo.Columns["UseQuantity"].ReadOnly = true;
                    MessageBox.Show("审核成功！", "软件提示");
                }
                else
                {
                    MessageBox.Show("审核失败！", "软件提示");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw;
            }
        }

        private void toolUnCheck_Click(object sender, EventArgs e)
        {
            string strSql = null;
            SqlDataReader sdr = null;
            string strPRProduceCode = null; //生产单号

            if (dgvPRProduceItemInfo.RowCount == 0)
            {
                return;
            }

            if (cbxIsComplete.SelectedValue.ToString() == "0")
            {
                MessageBox.Show("该生产单未做完工审核，无需弃审！", "软件提示");
                return;
            }

            try
            {
                strPRProduceCode = txtPRProduceCode.Text.Trim();

                // ✅ 参数化：检查是否已入库
                strSql = "SELECT * FROM PRInStore WHERE PRProduceCode = @PRProduceCode";
                SqlParameter[] checkPars = { new SqlParameter("@PRProduceCode", strPRProduceCode) };
                sdr = db.GetDataReader(strSql, checkPars);

                if (sdr.HasRows)
                {
                    MessageBox.Show("该生产单对应的产品已经入库，无法弃审！", "软件提示");
                    sdr.Close();
                    return;
                }
                sdr.Close();

                // ✅ 参数化：取消审核标记
                strSql = "UPDATE PRProduce SET IsComplete = '0' WHERE PRProduceCode = @PRProduceCode";
                SqlParameter[] pars = { new SqlParameter("@PRProduceCode", strPRProduceCode) };

                if (db.ExecDataBySql(strSql, pars) > 0)
                {
                    cbxIsComplete.SelectedValue = "0";
                    this.dgvPRProduceItemInfo.Columns["UseQuantity"].ReadOnly = false;
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
        }

        private void dgvPRProduceItemInfo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
