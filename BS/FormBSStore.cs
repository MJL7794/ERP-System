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
    public partial class FormBSStore : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormBSStore()
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
            this.txtStoreCode.ReadOnly = !this.txtStoreCode.ReadOnly;
            this.txtStoreName.ReadOnly = !this.txtStoreName.ReadOnly;
            this.txtArea.ReadOnly = !this.txtArea.ReadOnly;
            this.cbxEmployeeCode.Enabled = !this.cbxEmployeeCode.Enabled;
            this.rtbRemark.ReadOnly = !this.rtbRemark.ReadOnly;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            //窗体控件状态切换
            this.txtStoreCode.Text = "";
            this.txtStoreName.Text = "";
            this.txtArea.Text = "";
            this.cbxEmployeeCode.SelectedIndex = -1;
            this.rtbRemark.Text = "";
        }

        private void BindToolStripComboBox()
        {
            this.cbxCondition.Items.Add("仓库名称");
            this.cbxCondition.Items.Add("备注");
        }

        /// <summary>
        /// 设置控件的显示值
        /// </summary>
        private void FillControls()
        {
            this.txtStoreCode.Text = this.dgvStoreInfo[0, this.dgvStoreInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtStoreName.Text = this.dgvStoreInfo[1, this.dgvStoreInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtArea.Text = this.dgvStoreInfo[2, this.dgvStoreInfo.CurrentCell.RowIndex].Value.ToString();
            this.cbxEmployeeCode.SelectedValue = this.dgvStoreInfo[3, this.dgvStoreInfo.CurrentCell.RowIndex].Value;
            this.rtbRemark.Text = this.dgvStoreInfo[4, this.dgvStoreInfo.CurrentCell.RowIndex].Value.ToString();
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT StoreCode,StoreName,Area,";
            strSql += "EmployeeCode,Remark ";
            strSql += "FROM BSStore" + strWhere; ;

            try
            {
                this.dgvStoreInfo.DataSource = db.GetDataSet(strSql, "BSStore").Tables["BSStore"];
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

            pars.Add(new SqlParameter("@StoreCode", txtStoreCode.Text.Trim()));
            pars.Add(new SqlParameter("@StoreName", txtStoreName.Text.Trim()));

            if (String.IsNullOrEmpty(txtArea.Text.Trim()))
                pars.Add(new SqlParameter("@Area", DBNull.Value));
            else
                pars.Add(new SqlParameter("@Area", Convert.ToDecimal(txtArea.Text.Trim())));

            if (cbxEmployeeCode.SelectedValue == null)
                pars.Add(new SqlParameter("@EmployeeCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@EmployeeCode", cbxEmployeeCode.SelectedValue.ToString()));

            pars.Add(new SqlParameter("@Remark", rtbRemark.Text.Trim()));

            return pars.ToArray();
        }

        private void FormStore_Load(object sender, EventArgs e)
        {
            //权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            //ComboBox绑定到数据源
            commUse.BindComboBox(cbxEmployeeCode, "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
            //DataGridViewComboBoxColumn绑定到数据源
            commUse.BindComboBox(this.dgvStoreInfo.Columns[3], "EmployeeCode", "EmployeeName", "select EmployeeCode,EmployeeName from BSEmployee", "BSEmployee");
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
            toolStrip1.Tag = "ADD"; //添加操作
            txtStoreCode.Enabled = true;
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //修改操作
            txtStoreCode.Enabled = false;
        }

        private void toolreflush_Click(object sender, EventArgs e)
        {
            this.BindDataGridView("");
        }

        private void dgvStoreInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                if (dgvStoreInfo.RowCount > 0)
                {
                    //判断当前记录的主键值是否存在外键约束
                    if (commUse.IsExistConstraint("BSStore", dgvStoreInfo[0, dgvStoreInfo.CurrentCell.RowIndex].Value.ToString()))
                    {
                        this.txtStoreCode.Enabled = false;
                    }
                    else
                    {
                        this.txtStoreCode.Enabled = true;
                    }

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

        private void toolSave_Click(object sender, EventArgs e)
        {
            string strCode = null; ;
            SqlDataReader sdr = null;

            if (String.IsNullOrEmpty(txtStoreCode.Text.Trim()))
            {
                MessageBox.Show("仓库编号不许为空！", "软件提示");
                txtStoreCode.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtStoreName.Text.Trim()))
            {
                MessageBox.Show("仓库名称不许为空！", "软件提示");
                txtStoreName.Focus();
                return;
            }

            if (cbxEmployeeCode.SelectedValue == null)
            {
                MessageBox.Show("管理员不许为空！", "软件提示");
                cbxEmployeeCode.Focus();
                return;
            }

            //添加操作
            //添加操作
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                strCode = "SELECT * FROM BSStore WHERE StoreCode = @StoreCode";
                SqlParameter[] checkPars = { new SqlParameter("@StoreCode", txtStoreCode.Text.Trim()) };

                try
                {
                    sdr = db.GetDataReader(strCode, checkPars);
                    sdr.Read();
                    if (!sdr.HasRows)
                    {
                        sdr.Close();

                        strCode = "INSERT INTO BSStore(StoreCode,StoreName,Area,EmployeeCode,Remark) ";
                        strCode += "VALUES(@StoreCode,@StoreName,@Area,@EmployeeCode,@Remark)";

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
                        MessageBox.Show("编码重复，请重新设置", "软件提示");
                        this.txtStoreCode.Focus();
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

            //修改操作
            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                string strOldStoreCode = null;

                strOldStoreCode = this.dgvStoreInfo[0, this.dgvStoreInfo.CurrentCell.RowIndex].Value.ToString();

                //仓库代码被修改过
                if (strOldStoreCode != txtStoreCode.Text.Trim())
                {
                    strCode = "SELECT * FROM BSStore WHERE StoreCode = @StoreCode";
                    SqlParameter[] checkPars = { new SqlParameter("@StoreCode", txtStoreCode.Text.Trim()) };

                    try
                    {
                        sdr = db.GetDataReader(strCode, checkPars);
                        sdr.Read();
                        if (sdr.HasRows)
                        {
                            MessageBox.Show("编码重复，请重新设置", "软件提示");
                            this.txtStoreCode.Focus();
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
                    strCode = "UPDATE BSStore SET StoreCode = @StoreCode,StoreName = @StoreName,";
                    strCode += "Area = @Area,EmployeeCode = @EmployeeCode,Remark = @Remark ";
                    strCode += "WHERE StoreCode = @OldStoreCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldStoreCode", strOldStoreCode));

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

        private void toolDelete_Click(object sender, EventArgs e)
        {
            string strStoreCode = null;
            string strSql = null;

            if (this.dgvStoreInfo.RowCount == 0)
            {
                return;
            }

            strStoreCode = this.dgvStoreInfo[0, this.dgvStoreInfo.CurrentCell.RowIndex].Value.ToString();

            if (commUse.IsExistConstraint("BSStore", strStoreCode))
            {
                MessageBox.Show("已发生业务关系，无法删除", "软件提示");
                return;
            }

            strSql = "DELETE FROM BSStore WHERE StoreCode = @StoreCode";
            SqlParameter[] pars = { new SqlParameter("@StoreCode", strStoreCode) };

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

        private void txtOK_Click(object sender, EventArgs e)
        {
            string strWhere = String.Empty;
            string strConditonName = String.Empty;

            strConditonName = this.cbxCondition.Items[this.cbxCondition.SelectedIndex].ToString();
            switch (strConditonName)
            {
                case "仓库名称":

                    strWhere = " WHERE StoreName LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "备注":

                    strWhere = " WHERE Remark LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                default:
                    break;
            }
        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtArea_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputNumeric(e, sender as Control);
        }
    }
}
