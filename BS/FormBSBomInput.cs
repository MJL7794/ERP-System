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
using System.Reflection;  //引入PropertyInfo类

namespace ERP.BS
{
    public partial class FormBSBomInput : Form
    {
        DataBase db = new DataBase();//实例化DataBase类
        CommonUse commUse = new CommonUse();//实例化CommonUse类

        FormBSBom formBom = null; //物料清单窗体的引用
        int intNodeIndex;//定义一个整型变量，表示树形节点的索引

        List<PropertyClass> propInvens = null; //属性类的List泛型引用
        List<PropertyClass> propBoms = null; //属性类的List泛型引用

        public FormBSBomInput()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 加载存货信息(存货代码、存货名称)到List泛型
        /// </summary>
        /// <returns>包含存货信息的List泛型列表</returns>
        private List<PropertyClass> LoadInven()
        {
            PropertyClass proCla = new PropertyClass();  //实例化PropertyClass类
            Type elemnetType = proCla.GetType();  //获取proCla的Type
            PropertyInfo[] publicProperties = elemnetType.GetProperties();  //得到实例proCla的Type所拥有的所有公共属性
            //得到存货信息，该存货信息为DataTable类型
            DataTable dt = db.GetDataSet("select InvenCode,InvenName ,SpecsModel from BSInven", "BSInven").Tables["BSInven"];
            List<PropertyClass> tempProperties = new List<PropertyClass>();  //实例化List<PropertyClass>泛型类
            //逐行读取得到的存货信息
            foreach (DataRow row in dt.Rows)  
            {
                //循环数据源dt中的所有列
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    //循环实例proCla的Type拥有的所有公共属性
                    for (int j = 0; j < publicProperties.Length; j++)
                    {
                        //如果数据源dt中指定的列名称与某个公共属性的名称相同
                        if (dt.Columns[i].ColumnName == publicProperties[j].Name)
                        {
                            if (Convert.IsDBNull(row[i]))  //如果数据源dt中的当前数据项为null
                            {
                                continue;  //则终止本次循环，进入下一次循环
                            }
                            publicProperties[j].SetValue(proCla, row[i], null);  //为当前实例proCla中的某个属性赋值，该值为row[i]
                        }
                    }
                }

                tempProperties.Add(proCla);  //向泛型列表tempProperties中添加PropertyClass类型的元素，即PropertyClass类的实例proCla
                proCla = new PropertyClass();  //重新创建PropertyClass类的实例
            }

            return tempProperties;  //返回包含存货信息的List泛型列表
         }

        /// <summary>
        /// 加载Bom信息(物料清单)到List泛型(使用List泛型封装Bom信息)
        /// </summary>
        /// <returns>包含Bom信息的List泛型列表</returns>
        private List<PropertyClass> LoadBom()
        {
            List<PropertyClass> temps = new List<PropertyClass>();  //实例化List<PropertyClass>泛型类
            //得到Bom信息，该Bom信息为DataTable类型
            DataTable dt = db.GetDataSet("select ProInvenCode,MatInvenCode from BSBom", "BSBom").Tables["BSBom"];
            //逐行读取得到的Bom信息
            foreach (DataRow row in dt.Rows)
            {
                //创建PropertyClass类的实例temp
                PropertyClass temp = new PropertyClass();
                //给实例temp的属性赋值
                temp.ProInvenCode = row["ProInvenCode"].ToString();
                temp.MatInvenCode = row["MatInvenCode"].ToString();
                //向泛型列表temps中添加PropertyClass类型的元素，即PropertyClass类的实例temp
                temps.Add(temp);
            }
            return temps;
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        private SqlParameter[] ParametersAddValue()
        {
            List<SqlParameter> pars = new List<SqlParameter>();

            if (cbxProInvenCode.SelectedValue == null)
                pars.Add(new SqlParameter("@ProInvenCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@ProInvenCode", cbxProInvenCode.SelectedValue.ToString()));

            if (cbxMatInvenCode.SelectedValue == null)
                pars.Add(new SqlParameter("@MatInvenCode", DBNull.Value));
            else
                pars.Add(new SqlParameter("@MatInvenCode", cbxMatInvenCode.SelectedValue.ToString()));

            if (String.IsNullOrEmpty(txtQuantity.Text.Trim()))
                pars.Add(new SqlParameter("@Quantity", DBNull.Value));
            else
                pars.Add(new SqlParameter("@Quantity", Convert.ToInt32(txtQuantity.Text.Trim())));

            return pars.ToArray();
        }

        private void FormBomInput_Load(object sender, EventArgs e)
        {
            formBom = (FormBSBom)this.Owner;
            intNodeIndex = formBom.tvInven.SelectedNode.Index;
            propInvens = LoadInven();
            propBoms = LoadBom();
            commUse.BindComboBox(cbxProInvenCode, "InvenCode", "InvenName", "select InvenCode,InvenName ,SpecsModel from BSInven", "BSInven");
            commUse.BindComboBox(cbxMatInvenCode, "InvenCode", "InvenName", "select InvenCode,InvenName ,SpecsModel from BSInven", "BSInven");

            //在添加操作下打开FormBomInput窗体
            if (this.Tag.ToString() == "Add")
            {
                if (formBom.tvInven.SelectedNode.Tag == null)
                {
                    cbxProInvenCode.SelectedIndex = -1;
                    cbxMatInvenCode.SelectedIndex = -1;
                }
                else
                {
                    cbxProInvenCode.SelectedValue = formBom.tvInven.SelectedNode.Tag;
                    cbxMatInvenCode.SelectedIndex = -1;
                }
            }

            //在修改操作下打开FormBomInput窗体
            if (this.Tag.ToString() == "Edit")
            {
                cbxProInvenCode.SelectedValue = formBom.tvInven.SelectedNode.Tag;
                cbxProInvenCode.Enabled = false;
                cbxMatInvenCode.SelectedValue = formBom.dgvStructInfo[0, formBom.dgvStructInfo.CurrentRow.Index].Value;
                txtSpecsModel2.Text = formBom.dgvStructInfo[2, formBom.dgvStructInfo.CurrentRow.Index].Value.ToString();
                txtQuantity.Text = formBom.dgvStructInfo[4, formBom.dgvStructInfo.CurrentRow.Index].Value.ToString();
            }
        }

        private void cbxProInvenCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strItemValue;

            if (cbxProInvenCode.SelectedIndex != -1) //没有选中任何“Item”
            {
                strItemValue = cbxProInvenCode.SelectedValue.ToString();
                
                foreach (PropertyClass item in propInvens)
                {
                    if (item.InvenCode == strItemValue) //通过判断母件代码得到母件的规格型号
                    {
                        txtSpecsModel1.Text = item.SpecsModel;
                    }
                }
            }
        }

        private void cbxMatInvenCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strItemValue;

            if (cbxMatInvenCode.SelectedIndex != -1)
            {
                strItemValue = cbxMatInvenCode.SelectedValue.ToString();

                foreach (PropertyClass item in propInvens) //通过判断子件代码得到子件的规格型号
                {
                    if (item.InvenCode == strItemValue)
                    {
                        txtSpecsModel2.Text = item.SpecsModel;
                    }
                }
            }
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            commUse.InputInteger(e);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string strProInvenCode = null;                              //表示母件代码
            string strMatInvenCode = null;                              //表示子件代码
            string strOldMatInvenCode = null;                               //表示未修改之前的子件代码
            string strCode = null;                                      //表示SQL语句字符串
            if (cbxProInvenCode.SelectedIndex == -1)                    //母件不许为空
            {
                MessageBox.Show("请选择母件！", "软件提示");
                cbxProInvenCode.Focus();
                return;
            }
            if (cbxMatInvenCode.SelectedIndex == -1)                    //子件不许为空
            {
                MessageBox.Show("请选择子件！", "软件提示");
                cbxMatInvenCode.Focus();
                return;
            }
            //母件与子件不许相同
            if (cbxMatInvenCode.SelectedValue.ToString() == cbxProInvenCode.SelectedValue.ToString())
            {
                MessageBox.Show("母件与子件不许相同！", "软件提示");
                cbxProInvenCode.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtQuantity.Text.Trim()))              //组成数量不许为空
            {
                MessageBox.Show("组成数量不许为空！", "软件提示");
                txtQuantity.Focus();
                return;
            }
            if (Convert.ToInt32(txtQuantity.Text.Trim()) == 0)              //组成数量不许为零
            {
                MessageBox.Show("组成数量不许为零！", "软件提示");
                txtQuantity.Focus();
                return;
            }
            strProInvenCode = cbxProInvenCode.SelectedValue.ToString();     //获取当前的母件代码
            strMatInvenCode = cbxMatInvenCode.SelectedValue.ToString(); //获取当前的子件代码
                                                                        //如果是添加操作，则需要判断将要添加的子件是否与现有的子件重复
            if (this.Tag.ToString() == "Add")
            {
                foreach (PropertyClass item in propBoms)
                {
                    if (item.ProInvenCode == strProInvenCode && item.MatInvenCode == strMatInvenCode)
                    {
                        MessageBox.Show("子件不许重复！", "软件提示");
                        return;
                    }
                }

                SqlParameter[] pars = ParametersAddValue();   // ← 改成接收返回值

                strCode = "INSERT INTO BSBom(ProInvenCode,MatInvenCode,Quantity) ";
                strCode += "VALUES(@ProInvenCode,@MatInvenCode,@Quantity)";

                if (db.ExecDataBySql(strCode, pars) > 0)      // ← 传入参数
                {
                    MessageBox.Show("保存成功！", "软件提示");
                }
                else
                {
                    MessageBox.Show("保存失败！", "软件提示");
                }
            }

            if (this.Tag.ToString() == "Edit")
            {
                strOldMatInvenCode = formBom.dgvStructInfo[0, formBom.dgvStructInfo.CurrentRow.Index].Value.ToString();

                if (strMatInvenCode != strOldMatInvenCode)
                {
                    foreach (PropertyClass item in propBoms)
                    {
                        if (item.ProInvenCode == strProInvenCode && item.MatInvenCode == strMatInvenCode)
                        {
                            MessageBox.Show("子件不许重复！", "软件提示");
                            return;
                        }
                    }
                }

                SqlParameter[] pars = ParametersAddValue();   // ← 改成接收返回值

                strCode = "UPDATE BSBom SET ProInvenCode=@ProInvenCode,MatInvenCode = @MatInvenCode,Quantity = @Quantity ";
                strCode += "WHERE ProInvenCode = @OldProInvenCode AND MatInvenCode = @OldMatInvenCode";  // ← 改成参数化

                // 加上旧值参数
                List<SqlParameter> parsList = new List<SqlParameter>(pars);
                parsList.Add(new SqlParameter("@OldProInvenCode", strProInvenCode));
                parsList.Add(new SqlParameter("@OldMatInvenCode", strOldMatInvenCode));

                if (db.ExecDataBySql(strCode, parsList.ToArray()) > 0)   // ← 传入参数
                {
                    MessageBox.Show("保存成功！", "软件提示");
                }
                else
                {
                    MessageBox.Show("保存失败！", "软件提示");
                }
            }
            commUse.BuildTree(formBom.tvInven, formBom.imageList1, "母件", "V_BomStruct", "InvenCode",
                    "InvenName");                                   //TreeView控件重新绑定到数据源
                                                                    //重新设置物料清单窗体中TreeView控件的被选定节点
            formBom.tvInven.SelectedNode = formBom.tvInven.Nodes[0].Nodes[intNodeIndex];
            this.Close();												//关闭当前窗体

        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
