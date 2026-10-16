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

namespace ERP.BS
{
    public partial class FormBSSupplier : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();

        public FormBSSupplier()
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
            this.txtSupplierCode.ReadOnly = !this.txtSupplierCode.ReadOnly;
            this.txtSupplierName.ReadOnly = !this.txtSupplierName.ReadOnly;
            this.txtTelephoneCode.ReadOnly = !this.txtTelephoneCode.ReadOnly;
            this.txtEmail.ReadOnly = !this.txtEmail.ReadOnly;
            this.txtPostCode.ReadOnly = !this.txtPostCode.ReadOnly;
            this.txtLinkman.ReadOnly = !this.txtLinkman.ReadOnly;
            this.txtUrl.ReadOnly = !this.txtUrl.ReadOnly;
            this.txtAddress.ReadOnly = !this.txtAddress.ReadOnly;
        }

        /// <summary>
        /// 将控件恢复到原始状态
        /// </summary>
        private void ClearControls()
        {
            this.txtSupplierCode.Text = "";
            this.txtSupplierName.Text = "";
            this.txtTelephoneCode.Text = "";
            this.txtEmail.Text = "";
            this.txtPostCode.Text = "";
            this.txtLinkman.Text = "";
            this.txtUrl.Text = "";
            this.txtAddress.Text = "";
        }

        private void BindToolStripComboBox()
        {
            this.cbxCondition.Items.Add("供应商名称");
            this.cbxCondition.Items.Add("联系人");
            this.cbxCondition.Items.Add("地址");
        }

        /// <summary>
        /// 设置控件的显示值
        /// </summary>
        private void FillControls()
        {
            this.txtSupplierCode.Text = this.dgvSupplierInfo[0, this.dgvSupplierInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtSupplierName.Text =  this.dgvSupplierInfo[1, this.dgvSupplierInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtTelephoneCode.Text =  this.dgvSupplierInfo[2, this.dgvSupplierInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtEmail.Text =  this.dgvSupplierInfo[3, this.dgvSupplierInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtPostCode.Text =  this.dgvSupplierInfo[4, this.dgvSupplierInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtLinkman.Text =  this.dgvSupplierInfo[5, this.dgvSupplierInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtUrl.Text =  this.dgvSupplierInfo[6, this.dgvSupplierInfo.CurrentCell.RowIndex].Value.ToString();
            this.txtAddress.Text = this.dgvSupplierInfo[7, this.dgvSupplierInfo.CurrentCell.RowIndex].Value.ToString();
        }

        /// <summary>
        /// DataGridView控件绑定到数据源
        /// </summary>
        /// <param name="strWhere">Where条件子句</param>
        private void BindDataGridView(string strWhere)
        {
            string strSql = null;

            strSql = "SELECT SupplierCode as 供应商编码,SupplierName as 供应商名称,Linkman as 联系人,";
            strSql += "TelephoneCode as 联系电话,Email as 电子信箱,PostCode as 邮政编码,Url as 网址,";
            strSql += "Address as 地址 FROM BSSupplier " + strWhere;

            try
            {
                this.dgvSupplierInfo.DataSource = db.GetDataSet(strSql, "BSSupplier").Tables["BSSupplier"];
                
                foreach (DataGridViewColumn dgvc in dgvSupplierInfo.Columns)
                {
                    dgvc.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw ex;
            }
        }

        private void FormSupplier_Load(object sender, EventArgs e)
        {
            //权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            //
            this.BindDataGridView("");
            this.BindToolStripComboBox();
            this.cbxCondition.SelectedIndex = 0;
            toolStrip1.Tag = "";
        }
        private SqlParameter[] ParametersAddValue()
        {
            List<SqlParameter> pars = new List<SqlParameter>();

            pars.Add(new SqlParameter("@SupplierCode", txtSupplierCode.Text.Trim()));
            pars.Add(new SqlParameter("@SupplierName", txtSupplierName.Text.Trim()));
            pars.Add(new SqlParameter("@TelephoneCode", txtTelephoneCode.Text.Trim()));
            pars.Add(new SqlParameter("@Email", txtEmail.Text.Trim()));
            pars.Add(new SqlParameter("@PostCode", txtPostCode.Text.Trim()));
            pars.Add(new SqlParameter("@Linkman", txtLinkman.Text.Trim()));
            pars.Add(new SqlParameter("@Url", txtUrl.Text.Trim()));
            pars.Add(new SqlParameter("@Address", txtAddress.Text.Trim()));

            return pars.ToArray();
        }

        private void toolAdd_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "ADD"; //添加操作
            txtSupplierCode.Enabled = true;
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            ControlStatus();
            ClearControls();
            toolStrip1.Tag = "EDIT"; //修改操作
            txtSupplierCode.Enabled = false;
        }

        private void toolreflush_Click(object sender, EventArgs e)
        {
            this.BindDataGridView("");
        }

        private void dgvSupplierInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (toolStrip1.Tag.ToString() == "EDIT")
            {
                if (dgvSupplierInfo.RowCount > 0)
                {
                    //判断当前记录的主键值是否存在外键约束
                    if (commUse.IsExistConstraint("BSSupplier", dgvSupplierInfo[0, dgvSupplierInfo.CurrentCell.RowIndex].Value.ToString()))
                    {
                        this.txtSupplierCode.Enabled = false;
                    }
                    else
                    {
                        this.txtSupplierCode.Enabled = true;
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

            //添加操作
            if (toolStrip1.Tag.ToString() == "ADD")
            {
                strCode = "SELECT * FROM BSSupplier WHERE SupplierCode = @SupplierCode";
                SqlParameter[] checkPars = { new SqlParameter("@SupplierCode", txtSupplierCode.Text.Trim()) };

                try
                {
                    sdr = db.GetDataReader(strCode, checkPars);
                    sdr.Read();
                    if (!sdr.HasRows)
                    {
                        sdr.Close();

                        strCode = "INSERT INTO BSSupplier(SupplierCode,SupplierName,TelephoneCode,Email,PostCode,Linkman,Url,Address) ";
                        strCode += "VALUES(@SupplierCode,@SupplierName,@TelephoneCode,@Email,@PostCode,@Linkman,@Url,@Address)";

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
                        this.txtSupplierCode.Focus();
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
                string strOldSupplierCode = null;

                strOldSupplierCode = this.dgvSupplierInfo[0, this.dgvSupplierInfo.CurrentCell.RowIndex].Value.ToString();

                //供应商代码被修改过
                if (strOldSupplierCode != txtSupplierCode.Text.Trim())
                {
                    strCode = "SELECT * FROM BSSupplier WHERE SupplierCode = @SupplierCode";
                    SqlParameter[] checkPars = { new SqlParameter("@SupplierCode", txtSupplierCode.Text.Trim()) };

                    try
                    {
                        sdr = db.GetDataReader(strCode, checkPars);
                        sdr.Read();
                        if (sdr.HasRows)
                        {
                            MessageBox.Show("编码重复，请重新设置", "软件提示");
                            this.txtSupplierCode.Focus();
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
                    strCode = "UPDATE BSSupplier SET SupplierCode = @SupplierCode,SupplierName = @SupplierName,";
                    strCode += "TelephoneCode = @TelephoneCode,Email = @Email,PostCode = @PostCode,";
                    strCode += "Linkman = @Linkman,Url = @Url,Address = @Address ";
                    strCode += "WHERE SupplierCode = @OldSupplierCode";

                    List<SqlParameter> parsList = new List<SqlParameter>(ParametersAddValue());
                    parsList.Add(new SqlParameter("@OldSupplierCode", strOldSupplierCode));

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
            string strSupplierCode = null;
            string strSql = null;

            if (this.dgvSupplierInfo.RowCount == 0)
            {
                return;
            }

            strSupplierCode = this.dgvSupplierInfo[0, this.dgvSupplierInfo.CurrentCell.RowIndex].Value.ToString();

            if (commUse.IsExistConstraint("BSSupplier", strSupplierCode))
            {
                MessageBox.Show("已发生业务关系，无法删除", "软件提示");
                return;
            }

            strSql = "DELETE FROM BSSupplier WHERE SupplierCode = @SupplierCode";
            SqlParameter[] pars = { new SqlParameter("@SupplierCode", strSupplierCode) };

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
                case "供应商名称":

                    strWhere = " WHERE SupplierName LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "联系人":

                    strWhere = " WHERE Linkman LIKE '%" + txtKeyWord.Text.Trim() + "%'";
                    this.BindDataGridView(strWhere);
                    break;

                case "地址":

                    strWhere = " WHERE Address LIKE '%" + txtKeyWord.Text.Trim() + "%'";
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
    }
}
