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

namespace ERP
{
    public partial class Login : Form
    {
        DataBase db = new DataBase();
        SqlDataReader sdr = null;

        public Login()
        {
            InitializeComponent();
        }
        //登录用户文本框敲回车键
        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//判断是否按下回车键
            {
                txtPwd.Focus();//使密码文本框获得焦点
            }
        }
        //登录密码文本框敲回车键
        private void txtPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//判断是否按下回车键
            {
                picLogin_Click(sender, e);//执行登录事件
            }
        }
        //登录
        private void picLogin_Click(object sender, EventArgs e)
        {
            this.errInfo.Clear();

            if (String.IsNullOrEmpty(this.txtCode.Text.Trim()))
            {
                try
                {
                    this.errInfo.SetError(this.txtCode, "用户名不能为空！");
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "软件提示");
                    throw ex;
                }
                finally
                {

                }
            }

            if (String.IsNullOrEmpty(this.txtPwd.Text.Trim()))
            {
                try
                {
                    this.errInfo.SetError(this.txtPwd, "用户密码不能为空！");
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "软件提示");
                    throw ex;
                }
                finally
                {

                }
            }
            //根据输入的用户名和密码查询数据
            string strSql = "select * from SYOperator where OperatorCode = '" + txtCode.Text.Trim() + "' and PassWord = '" + txtPwd.Text.Trim() + "'";

            try
            {
                sdr = db.GetDataReader(strSql);//获取查询结果
                sdr.Read();//读取数据
                if (sdr.HasRows)//判断是否查询到结果
                {
                    FormMain formMain = new FormMain();//实例化主窗体对象
                    this.Hide();//隐藏当前窗体
                    PropertyClass.OperatorCode = sdr["OperatorCode"].ToString();//记录当前操作员
                    PropertyClass.OperatorName = sdr["OperatorName"].ToString();//记录当前操作用户名
                    PropertyClass.PassWord = sdr["PassWord"].ToString();//记录输入的密码
                    PropertyClass.IsAdmin = sdr["IsAdmin"].ToString();//记录是否为管理员
                    formMain.Show();//显示主窗体
                }
                else
                {
                    MessageBox.Show("用户编码或用户密码不正确！", "软件提示");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw ex;
            }
            finally
            {
                sdr.Close();
            }
        }
        //重置
        private void picReset_Click(object sender, EventArgs e)
        {
            txtCode.Text = "";
            txtPwd.Text = "";
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void txtCode_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
