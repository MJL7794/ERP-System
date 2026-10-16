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

namespace ERP.CU
{
    public partial class FormBaseTypeInput : Form
    {
        DataBase db = new DataBase();
        CommonUse commUse = new CommonUse();
        FormBaseType formBaseType = null;

        public FormBaseTypeInput()
        {
            InitializeComponent();
        }

        private void FormBaseTypeInput_Load(object sender, EventArgs e)
        {
            formBaseType = (FormBaseType)this.Owner;
            this.Text = formBaseType.tvBaseType.SelectedNode.Text+"编辑";

            //在修改操作下打开formCUGradeInput窗体
            if (this.Tag.ToString() == "Edit")
            {
                txtCode.Text = formBaseType.dgvBaseTypeInfo[0, formBaseType.dgvBaseTypeInfo.CurrentCell.RowIndex].Value.ToString();
                txtName.Text = formBaseType.dgvBaseTypeInfo[1, formBaseType.dgvBaseTypeInfo.CurrentCell.RowIndex].Value.ToString();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string strSql = null;
            string strOldCode = null;
            SqlDataReader sdr = null;
            string strTable = null;      // 数据表名称
            string strCodeColumn = null; // 代码列
            string strNameColumn = null; // 名称列

            errorInfo.Clear();

            // 校验：编码不许为空
            if (String.IsNullOrEmpty(txtCode.Text.Trim()))
            {
                errorInfo.SetError(txtCode, "类别编码不许为空！");
                return;
            }

            // 校验：名称不许为空
            if (String.IsNullOrEmpty(txtName.Text.Trim()))
            {
                errorInfo.SetError(txtName, "类别名称不许为空！");
                return;
            }

            strTable = formBaseType.tvBaseType.SelectedNode.Tag.ToString();
            strCodeColumn = formBaseType.dgvBaseTypeInfo.Columns[0].Name;
            strNameColumn = formBaseType.dgvBaseTypeInfo.Columns[1].Name;

            // ============ 添加操作 ============
            if (this.Tag.ToString() == "Add")
            {
                strSql = "SELECT * FROM " + strTable + " WHERE " + strCodeColumn + " = @Code";
                SqlParameter[] checkPars = { new SqlParameter("@Code", txtCode.Text.Trim()) };

                try
                {
                    sdr = db.GetDataReader(strSql, checkPars);
                    sdr.Read();

                    if (!sdr.HasRows)
                    {
                        sdr.Close();

                        strSql = "INSERT INTO " + strTable + "(" + strCodeColumn + "," + strNameColumn + ") ";
                        strSql += "VALUES(@Code, @Name)";

                        SqlParameter[] pars = {
                    new SqlParameter("@Code", txtCode.Text.Trim()),
                    new SqlParameter("@Name", txtName.Text.Trim())
                };

                        if (db.ExecDataBySql(strSql, pars) > 0)
                        {
                            MessageBox.Show("保存成功！", "软件提示");
                            formBaseType.BindDataGridView(strTable, strCodeColumn, strNameColumn);
                            btnQuit_Click(sender, e);
                        }
                        else
                        {
                            MessageBox.Show("保存失败！", "软件提示");
                        }
                    }
                    else
                    {
                        MessageBox.Show("编码重复，请重新设置", "软件提示");
                        txtCode.Focus();
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
            // ============ 修改操作 ============
            else
            {
                // 未修改之前的代码
                strOldCode = formBaseType.dgvBaseTypeInfo[0, formBaseType.dgvBaseTypeInfo.CurrentCell.RowIndex].Value.ToString();

                // 代码被修改过 → 检查是否重复
                if (strOldCode != txtCode.Text.Trim())
                {
                    strSql = "SELECT * FROM " + strTable + " WHERE " + strCodeColumn + " = @Code";
                    SqlParameter[] checkPars = { new SqlParameter("@Code", txtCode.Text.Trim()) };

                    try
                    {
                        sdr = db.GetDataReader(strSql, checkPars);
                        sdr.Read();

                        if (sdr.HasRows)
                        {
                            MessageBox.Show("编码重复，请重新设置", "软件提示");
                            txtCode.Focus();
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

                // 更新数据库
                try
                {
                    strSql = "UPDATE " + strTable + " SET " + strCodeColumn + " = @Code, " + strNameColumn + " = @Name ";
                    strSql += "WHERE " + strCodeColumn + " = @OldCode";

                    SqlParameter[] pars = {
                new SqlParameter("@Code", txtCode.Text.Trim()),
                new SqlParameter("@Name", txtName.Text.Trim()),
                new SqlParameter("@OldCode", strOldCode)
            };

                    if (db.ExecDataBySql(strSql, pars) > 0)
                    {
                        MessageBox.Show("保存成功！", "软件提示");
                        formBaseType.BindDataGridView(strTable, strCodeColumn, strNameColumn);
                        btnQuit_Click(sender, e);
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

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
