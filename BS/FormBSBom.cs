using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ERP.ComClass;//引入CommonUse类
using ERP.DataClass;//引入DataBase类

namespace ERP.BS
{
    public partial class FormBSBom : Form
    {
        DataBase db = new DataBase();//创建DataBase类的实例，用于操作数据
        CommonUse commUse = new CommonUse();//创建CommonUse类的实例，调用该类的相关方法

        public FormBSBom()
        {
            InitializeComponent();
        }

        /// <summary>
        /// DataGridView控件绑定数据源
        /// </summary>
        /// <param name="strInvenCode">母件代码</param>
        private void BindDataGridView(string strInvenCode){
            string strSql = null;

            strSql = "SELECT BSBom.MatInvenCode,BSInven.InvenName,BSInven.SpecsModel,BSInven.MeaUnit,BSBom.Quantity ";
            strSql += "FROM BSBom,BSInven ";
            strSql += "WHERE BSBom.MatInvenCode = BSInven.InvenCode and BSBom.ProInvenCode = '" + strInvenCode + "'";

            try
            {
                this.dgvStructInfo.DataSource = db.GetDataSet(strSql, "InvenBom").Tables["InvenBom"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw ex;
            }
        }

        private void FormBom_Load(object sender, EventArgs e)
        {
            //设置用户的操作权限
            commUse.CortrolButtonEnabled(toolAdd, this);
            commUse.CortrolButtonEnabled(toolAmend, this);
            commUse.CortrolButtonEnabled(toolDelete, this);
            //TreeView绑定到数据源，显示现有的母件
            commUse.BuildTree(tvInven, imageList1, "母件", "V_BomStruct", "InvenCode", "InvenName");
        }

        private void tvInven_AfterSelect(object sender, TreeViewEventArgs e)
        {
            commUse.DataGridViewReset(dgvStructInfo); //清空DataGridView

            if (tvInven.SelectedNode != null)//如果是非空节点
            {
                if (tvInven.SelectedNode.Tag != null)//如果是非根节点
                {
                    BindDataGridView(tvInven.SelectedNode.Tag.ToString());//检索并显示该母件的子件信息
                }
            }
        }

        private void toolAdd_Click(object sender, EventArgs e)
        {
            if (tvInven.SelectedNode != null)//如果是非空节点
            {
                FormBSBomInput formBomInput = new FormBSBomInput(); //实例化FormBSBomInput窗体（物料清单编辑窗体）
                formBomInput.Tag = "Add"; //表示添加操作，说明修改时Edit
                formBomInput.Owner = this;//设置拥有此窗体的窗体，即FormBSBom窗体
                formBomInput.ShowDialog();//将窗体显示为模式对话框
            }
        }

        private void toolAmend_Click(object sender, EventArgs e)
        {
            if (tvInven.SelectedNode != null)
            {
                if (tvInven.SelectedNode.Tag != null)
                {
                    FormBSBomInput formBomInput = new FormBSBomInput();
                    formBomInput.Tag = "Edit"; //修改状态
                    formBomInput.Owner = this;
                    formBomInput.ShowDialog();
                }
            }
        }

        private void toolDelete_Click(object sender, EventArgs e)
        {
            string strProInvenCode = null; //母件代码
            string strMatInvenCode = null; //子件代码
            string strSql = null;

            if (dgvStructInfo.Rows.Count > 0 && tvInven.SelectedNode != null)
            {
                strProInvenCode = this.tvInven.SelectedNode.Tag.ToString();
                strMatInvenCode = this.dgvStructInfo[0, this.dgvStructInfo.CurrentRow.Index].Value.ToString();

                strSql = "DELETE FROM BSBom WHERE ProInvenCode = '" + strProInvenCode + "' AND  MatInvenCode = '" + strMatInvenCode + "'";

                if (MessageBox.Show("确定要删除吗？", "软件提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    try
                    {
                        if (db.ExecDataBySql(strSql) > 0)
                        {
                            MessageBox.Show("删除成功！", "软件提示");
                            this.dgvStructInfo.Rows.RemoveAt(this.dgvStructInfo.CurrentRow.Index);
                        }
                        else
                        {
                            MessageBox.Show("删除失败！", "软件提示");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "软件提示");
                        throw ex;
                    }

                    if (dgvStructInfo.Rows.Count > 0)
                    {
                        this.BindDataGridView(tvInven.SelectedNode.Tag.ToString());
                    }
                    else
                    {
                        commUse.BuildTree(tvInven, imageList1, "母件", "V_BomStruct", "InvenCode", "InvenName");
                        tvInven.SelectedNode = tvInven.Nodes[0];
                    }
                }
            }
        }

        private void toolreflush_Click(object sender, EventArgs e)
        {
            commUse.BuildTree(tvInven, imageList1, "母件", "V_BomStruct", "InvenCode", "InvenName");
            tvInven.SelectedNode = tvInven.Nodes[0]; //选中根节点
        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
