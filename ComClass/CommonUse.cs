using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using ERP.BS;
using ERP.PU;
using ERP.SE;
using ERP.ST;
using ERP.PR;
using ERP.CU;
using ERP.FI;
using ERP.SY;
using ERP.DataClass;

namespace ERP.ComClass
{
    /// <summary>
    /// 公共的通用类，提供一些通用的方法
    /// </summary>
    public class CommonUse
    {
        DataBase db = new DataBase();

        public CommonUse()
        {
        }

        /// <summary>
        /// TreeView控件绑定到数据源
        /// </summary>
        ///
        public void BuildTree(TreeView tv, ImageList imgList, string rootName, string strTable, string strCode, string strName)//建立树形结构
        {
            string strSql = null;
            DataSet ds = null;
            DataTable dt = null;
            TreeNode rootNode = null;
            TreeNode childNode = null;

            strSql = "SELECT " + strCode + " , " + strName + " FROM " + strTable;

            tv.Nodes.Clear();
            tv.ImageList = imgList;

            rootNode = new TreeNode();
            rootNode.Tag = null;
            rootNode.Text = rootName;
            rootNode.ImageIndex = 1;
            rootNode.SelectedImageIndex = 0;

            try
            {
                ds = db.GetDataSet(strSql, strTable);
                dt = ds.Tables[strTable];

                foreach (DataRow row in dt.Rows)
                {
                    childNode = new TreeNode();
                    childNode.Tag = row[strCode];
                    childNode.Text = row[strName].ToString();
                    childNode.ImageIndex = 1;
                    childNode.SelectedImageIndex = 0;
                    rootNode.Nodes.Add(childNode);
                }

                tv.Nodes.Add(rootNode);
                tv.ExpandAll();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "软件提示");
                throw;  // ✅ 保留原始堆栈
            }
        }

        /// <summary>
        /// 清空DataGridView
        /// </summary>
        public void DataGridViewReset(DataGridView dgv)
        {
            if (dgv.DataSource != null)
            {
                if (dgv.DataSource.GetType() == typeof(DataTable))
                {
                    DataTable dt = dgv.DataSource as DataTable;
                    dt.Clear();
                }

                if (dgv.DataSource.GetType() == typeof(BindingSource))
                {
                    BindingSource bs = dgv.DataSource as BindingSource;
                    DataTable dt = bs.DataSource as DataTable;
                    dt.Clear();
                }
            }
        }

        /// <summary>
        /// 在DataGridView控件的指定位置插入行
        /// </summary>
        public DataGridViewRow DataGridViewInsertRow(DataGridView dgv, BindingSource bs, DataTable dt, int intPosIndex)
        {
            DataGridViewRow dgvr = null;

            try
            {
                DataRow dr = dt.NewRow();
                dt.Rows.InsertAt(dr, intPosIndex);
                bs.DataSource = dt;//BindingSource 重新绑定
                dgv.DataSource = bs;//DataGridView 重新绑定
                dgvr = dgv.Rows[intPosIndex];
            }
            catch (Exception)
            {
                throw;  // ✅ 保留原始堆栈
            }

            return dgvr;
        }

        /// <summary>
        /// 在DataGridView控件的末尾添加行
        /// </summary>
        public DataGridViewRow DataGridViewInsertRowAtEnd(DataGridView dgv, BindingSource bs, DataTable dt)
        {
            DataGridViewRow dgvr = null;

            try
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
                bs.DataSource = dt;
                dgv.DataSource = bs;
                dgvr = dgv.Rows[dgv.RowCount - 1];
            }
            catch (Exception)
            {
                throw;  // ✅ 保留原始堆栈
            }

            return dgvr;
        }

        /// <summary>
        /// ComboBox或DataGridViewComboBoxColumn绑定到数据源
        /// </summary>
        public void BindComboBox(Object obj, string strValueColumn, string strTextColumn, string strSql, string strTable)
        {
            try
            {
                // ✅ 用 is 模式匹配，不用字符串比较
                if (obj is ComboBox cbx)
                {
                    cbx.BeginUpdate();
                    cbx.DataSource = db.GetDataSet(strSql, strTable).Tables[strTable];
                    cbx.DisplayMember = strTextColumn;
                    cbx.ValueMember = strValueColumn;
                    cbx.EndUpdate();
                }
                else if (obj is DataGridViewComboBoxColumn dgvcbx)
                {
                    dgvcbx.DataSource = db.GetDataSet(strSql, strTable).Tables[strTable];
                    dgvcbx.DisplayMember = strTextColumn;
                    dgvcbx.ValueMember = strValueColumn;
                }
            }
            catch (Exception)
            {
                throw;  // ✅ 保留原始堆栈
            }
        }

        /// <summary>
        /// 控制可编辑控件的键盘输入，只接收非负十进制数
        /// </summary>
        public void InputNumeric(KeyPressEventArgs e, Control con)//这里用粘贴会导致不符合要求的字符输入，记得修
        {
            // 文本框为空时，不允许输入 "."
            if (String.IsNullOrEmpty(con.Text) && e.KeyChar == '.')
            {
                e.Handled = true;
            }

            // 已包含 "." 时，不允许再输入 "."
            if (con.Text.Contains(".") && e.KeyChar == '.')
            {
                e.Handled = true;
            }

            // ✅ 只允许：数字、"."、退格键、控制键
            if (!char.IsDigit(e.KeyChar)
                && e.KeyChar != '.'
                && e.KeyChar != '\b'
                && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 控制可编辑控件的键盘输入，只接收非负整数
        /// </summary>
        public void InputInteger(KeyPressEventArgs e)
        {
            // ✅ 只允许：数字、退格键、控制键
            if (!char.IsDigit(e.KeyChar)
                && e.KeyChar != '\b'
                && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 获取数据库系统的时间
        /// </summary>
        public DateTime GetDBTime()
        {
            DateTime dtDBTime;

            try
            {
                dtDBTime = Convert.ToDateTime(db.GetSingleObject("SELECT GETDATE()"));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "软件提示");
                throw;  // ✅ 保留原始堆栈
            }

            return dtDBTime;
        }

        /// <summary>
        /// 生成单据编号
        /// </summary>
        public string BuildBillCode(string strTable, string strBillCodeColumn, string strBillDateColumn, DateTime dtBillDate)
        {
            string strSql;
            string strBillDate;
            string strMaxSeqNum;
            string strNewSeqNum;
            string strBillCode;

            try
            {
                strBillDate = dtBillDate.ToString("yyyyMMdd");

                // ✅ 参数化日期，列名/表名不能参数化（用白名单或固定值）
                strSql = "SELECT SUBSTRING(MAX(" + strBillCodeColumn + "),10,4) FROM " + strTable + " WHERE " + strBillDateColumn + " = @BillDate";
                SqlParameter[] pars = { new SqlParameter("@BillDate", dtBillDate.ToString("yyyy-MM-dd")) };

                strMaxSeqNum = db.GetSingleObject(strSql, pars) as string;

                if (String.IsNullOrEmpty(strMaxSeqNum))
                {
                    strMaxSeqNum = "0000";
                }

                strNewSeqNum = (Convert.ToInt32(strMaxSeqNum) + 1).ToString("0000");
                strBillCode = strBillDate + "-" + strNewSeqNum;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw;  // ✅ 保留原始堆栈
            }

            return strBillCode;
        }

        /// <summary>
        /// 通过若干条领料单记录计算平均单价
        /// </summary>
        public decimal GetAvePriceBySTGetMaterial(string strPRProduceCode, string strInvenCode)
        {
            string strSql = null;
            decimal decAvePrice;

            // ✅ 参数化
            strSql = "SELECT SUM(UnitPrice * Quantity) / SUM(Quantity) FROM STGetMaterial WHERE BillType = '1' AND IsFlag = '1' AND PRProduceCode = @PRProduceCode AND InvenCode = @InvenCode";
            SqlParameter[] pars = {
                new SqlParameter("@PRProduceCode", strPRProduceCode),
                new SqlParameter("@InvenCode", strInvenCode)
            };

            try
            {
                decAvePrice = Convert.ToDecimal(db.GetSingleObject(strSql, pars));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw;  // ✅ 保留原始堆栈
            }

            return decAvePrice;
        }

        /// <summary>
        /// DataGridView导出到Excel
        /// </summary>
        public void DataGridViewExportToExcel(DataGridView dgv, string strTitle)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xls)|*.xls";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = false;
            saveFileDialog.FileName = strTitle + ".xls";

            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            Stream myStream = saveFileDialog.OpenFile();
            // ✅ 用 UTF-8 BOM，Excel 能正确识别中文
            StreamWriter sw = new StreamWriter(myStream, new UTF8Encoding(true));

            string strHeaderText = "";

            try
            {
                // 写标题
                for (int i = 0; i < dgv.ColumnCount; i++)
                {
                    if (i > 0) strHeaderText += "\t";
                    strHeaderText += dgv.Columns[i].HeaderText;
                }
                sw.WriteLine(strHeaderText);

                // 写内容
                string strItemValue = "";
                for (int j = 0; j < dgv.RowCount; j++)
                {
                    strItemValue = "";
                    for (int k = 0; k < dgv.ColumnCount; k++)
                    {
                        if (k > 0) strItemValue += "\t";
                        // ✅ 单元格可能为 null
                        strItemValue += dgv.Rows[j].Cells[k].Value?.ToString() ?? "";
                    }
                    sw.WriteLine(strItemValue);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw;  // ✅ 保留原始堆栈
            }
            finally
            {
                sw.Close();
                myStream.Close();
            }
        }

        /// <summary>
        /// 执行相关操作前判断数据表中记录的主键值是否存在外键约束
        /// </summary>
        public bool IsExistConstraint(string strPrimaryTable, string strPrimaryValue)
        {
            bool booIsExist = false;
            string strSql = null;
            string strForeignColumn = null;
            string strForeignTable = null;
            SqlDataReader sdr = null;

            try
            {   //准备参数化查询，防止SQL注入
                SqlParameter param = new SqlParameter("@PrimaryTable", SqlDbType.VarChar);
                param.Value = strPrimaryTable;
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(param);
                SqlParameter[] inputParameters = parameters.ToArray();


                DataTable dt = db.GetDataTable("P_QueryForeignConstraint", inputParameters);

                foreach (DataRow dr in dt.Rows)//遍历每个外键关系
                {
                    strForeignTable = dr["ForeignTable"].ToString();
                    strForeignColumn = dr["ForeignColumn"].ToString();

                    // ✅ 参数化主键值
                    strSql = "SELECT " + strForeignColumn + " FROM " + strForeignTable + " WHERE " + strForeignColumn + " = @PrimaryValue";
                    SqlParameter[] pars = { new SqlParameter("@PrimaryValue", strPrimaryValue) };

                    sdr = db.GetDataReader(strSql, pars);

                    if (sdr.HasRows)
                    {
                        booIsExist = true;
                        sdr.Close();
                        break;
                    }

                    sdr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "软件提示");
                throw;  // ✅ 保留原始堆栈
            }

            return booIsExist;
        }

        /// <summary>
        /// 通过控制按钮的Enabled属性来达到控制操作权限的目的
        /// </summary>
        public void CortrolButtonEnabled(IComponent iComp, Form form)
        {
            string strRightTag = null;
            Button btn = null;
            ToolStripButton tsb = null;

            // 判断控件类型
            if (iComp is Button b)
            {
                btn = b;
                strRightTag = btn.Name.Substring(3);
            }
            else if (iComp is ToolStripButton t)
            {
                tsb = t;
                strRightTag = tsb.Name.Substring(4);
            }
            else
            {
                return;  // 其他类型不处理
            }

            // 系统管理员不受限制
            if (PropertyClass.IsAdmin == "1")
            {
                // ✅ 管理员：所有按钮可用
                if (btn != null) btn.Enabled = true;
                else if (tsb != null) tsb.Enabled = true;
            }
            else
            {
                // ✅ 参数化（其实这里的值都是内部生成的，风险低）
                string moduleTag = form.Tag?.ToString() ?? "0";
                string strSql = "SELECT IsRight FROM SYAssignRight WHERE OperatorCode = @OperatorCode AND ModuleTag = @ModuleTag AND RightTag = @RightTag";

                SqlParameter[] pars = {
                    new SqlParameter("@OperatorCode", PropertyClass.OperatorCode),
                    new SqlParameter("@ModuleTag", moduleTag),
                    new SqlParameter("@RightTag", strRightTag)
                };

                try
                {
                    DataTable dt = db.GetDataTable(strSql, "SYAssignRight", pars);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        DataColumn dc = dt.Columns["IsRight"];

                        if (dr[dc].ToString() == "1")  // 有权限
                        {
                            if (btn != null) btn.Enabled = true;
                            else if (tsb != null) tsb.Enabled = true;
                        }
                        else  // 无权限
                        {
                            if (btn != null) btn.Enabled = false;
                            else if (tsb != null) tsb.Enabled = false;
                        }
                    }
                    else  // 没有权限记录
                    {
                        if (btn != null) btn.Enabled = false;
                        else if (tsb != null) tsb.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "软件提示");
                    throw;  // ✅ 保留原始堆栈
                }
            }
        }

        /// <summary>
        /// 打开Form窗体（菜单）
        /// </summary>
        public void ShowForm(ToolStripMenuItem menuItem, Form form)
        {
            // ✅ 保持原样（switch 太长，但你不想改签名）
            // 这里只把 throw 处理一下（如果有的话）
            switch (menuItem.Tag.ToString())
            {
                case "111":
                    FormBSInvenType invenType = new FormBSInvenType();
                    invenType.MdiParent = form;
                    invenType.StartPosition = FormStartPosition.CenterScreen;
                    invenType.Tag = menuItem.Tag.ToString();
                    invenType.Show();
                    break;
                case "112":
                    FormBSDepartment department = new FormBSDepartment();
                    department.MdiParent = form;
                    department.StartPosition = FormStartPosition.CenterScreen;
                    department.Tag = menuItem.Tag.ToString();
                    department.Show();
                    break;
                case "113":
                    FormBSCostType costType = new FormBSCostType();
                    costType.MdiParent = form;
                    costType.StartPosition = FormStartPosition.CenterScreen;
                    costType.Tag = menuItem.Tag.ToString();
                    costType.Show();
                    break;
                case "121":
                    FormBSInven inven = new FormBSInven();
                    inven.MdiParent = form;
                    inven.StartPosition = FormStartPosition.CenterScreen;
                    inven.Tag = menuItem.Tag.ToString();
                    inven.Show();
                    break;
                case "122":
                    FormBSSupplier supplier = new FormBSSupplier();
                    supplier.MdiParent = form;
                    supplier.StartPosition = FormStartPosition.CenterScreen;
                    supplier.Tag = menuItem.Tag.ToString();
                    supplier.Show();
                    break;
                case "123":
                    FormBSCustomer customer = new FormBSCustomer();
                    customer.MdiParent = form;
                    customer.StartPosition = FormStartPosition.CenterScreen;
                    customer.Tag = menuItem.Tag.ToString();
                    customer.Show();
                    break;
                case "124":
                    FormBSCost cost = new FormBSCost();
                    cost.MdiParent = form;
                    cost.StartPosition = FormStartPosition.CenterScreen;
                    cost.Tag = menuItem.Tag.ToString();
                    cost.Show();
                    break;
                case "125":
                    FormBSStore store = new FormBSStore();
                    store.MdiParent = form;
                    store.StartPosition = FormStartPosition.CenterScreen;
                    store.Tag = menuItem.Tag.ToString();
                    store.Show();
                    break;
                case "126":
                    FormBSEmployee employee = new FormBSEmployee();
                    employee.MdiParent = form;
                    employee.StartPosition = FormStartPosition.CenterScreen;
                    employee.Tag = menuItem.Tag.ToString();
                    employee.Show();
                    break;
                case "130":
                    FormBSAccount account = new FormBSAccount();
                    account.MdiParent = form;
                    account.StartPosition = FormStartPosition.CenterScreen;
                    account.Tag = menuItem.Tag.ToString();
                    account.Show();
                    break;
                case "140":
                    FormBSBom bom = new FormBSBom();
                    bom.MdiParent = form;
                    bom.StartPosition = FormStartPosition.CenterScreen;
                    bom.Tag = menuItem.Tag.ToString();
                    bom.Show();
                    break;
                case "150":
                    FormInitStock initStock = new FormInitStock();
                    initStock.MdiParent = form;
                    initStock.StartPosition = FormStartPosition.CenterScreen;
                    initStock.Tag = menuItem.Tag.ToString();
                    initStock.Show();
                    break;
                case "210":
                    FormPUOrder puOrder = new FormPUOrder();
                    puOrder.MdiParent = form;
                    puOrder.StartPosition = FormStartPosition.CenterScreen;
                    puOrder.Tag = menuItem.Tag.ToString();
                    puOrder.Show();
                    break;
                case "220":
                    FormPUInStore puInStore = new FormPUInStore();
                    puInStore.MdiParent = form;
                    puInStore.StartPosition = FormStartPosition.CenterScreen;
                    puInStore.Tag = menuItem.Tag.ToString();
                    puInStore.Show();
                    break;
                case "230":
                    FormPUPay formPUPay = new FormPUPay();
                    formPUPay.MdiParent = form;
                    formPUPay.StartPosition = FormStartPosition.CenterScreen;
                    formPUPay.Tag = menuItem.Tag.ToString();
                    formPUPay.Show();
                    break;
                case "310":
                    FormSEOrder formSEOrder = new FormSEOrder();
                    formSEOrder.MdiParent = form;
                    formSEOrder.StartPosition = FormStartPosition.CenterScreen;
                    formSEOrder.Tag = menuItem.Tag.ToString();
                    formSEOrder.Show();
                    break;
                case "320":
                    FormSEOutStore formSEOutStore = new FormSEOutStore();
                    formSEOutStore.MdiParent = form;
                    formSEOutStore.StartPosition = FormStartPosition.CenterScreen;
                    formSEOutStore.Tag = menuItem.Tag.ToString();
                    formSEOutStore.Show();
                    break;
                case "330":
                    FormSEGather formSEGather = new FormSEGather();
                    formSEGather.MdiParent = form;
                    formSEGather.StartPosition = FormStartPosition.CenterScreen;
                    formSEGather.Tag = menuItem.Tag.ToString();
                    formSEGather.Show();
                    break;
                case "410":
                    FormSTGetMaterial formSTGetMaterial = new FormSTGetMaterial();
                    formSTGetMaterial.MdiParent = form;
                    formSTGetMaterial.StartPosition = FormStartPosition.CenterScreen;
                    formSTGetMaterial.Tag = menuItem.Tag.ToString();
                    formSTGetMaterial.Show();
                    break;
                case "420":
                    FormSTReturnMaterial formSTReturnMaterial = new FormSTReturnMaterial();
                    formSTReturnMaterial.MdiParent = form;
                    formSTReturnMaterial.StartPosition = FormStartPosition.CenterScreen;
                    formSTReturnMaterial.Tag = menuItem.Tag.ToString();
                    formSTReturnMaterial.Show();
                    break;
                case "430":
                    FormSTLoss formSTLoss = new FormSTLoss();
                    formSTLoss.MdiParent = form;
                    formSTLoss.StartPosition = FormStartPosition.CenterScreen;
                    formSTLoss.Tag = menuItem.Tag.ToString();
                    formSTLoss.Show();
                    break;
                case "440":
                    FormSTCheck formSTCheck = new FormSTCheck();
                    formSTCheck.MdiParent = form;
                    formSTCheck.StartPosition = FormStartPosition.CenterScreen;
                    formSTCheck.Tag = menuItem.Tag.ToString();
                    formSTCheck.Show();
                    break;
                case "450":
                    FormStockQuery formStockQuery = new FormStockQuery();
                    formStockQuery.MdiParent = form;
                    formStockQuery.StartPosition = FormStartPosition.CenterScreen;
                    formStockQuery.Tag = menuItem.Tag.ToString();
                    formStockQuery.Show();
                    break;
                case "510":
                    FormPRPlan formPRPlan = new FormPRPlan();
                    formPRPlan.MdiParent = form;
                    formPRPlan.StartPosition = FormStartPosition.CenterScreen;
                    formPRPlan.Tag = menuItem.Tag.ToString();
                    formPRPlan.Show();
                    break;
                case "520":
                    FormPRProduce formPRProduce = new FormPRProduce();
                    formPRProduce.MdiParent = form;
                    formPRProduce.StartPosition = FormStartPosition.CenterScreen;
                    formPRProduce.Tag = menuItem.Tag.ToString();
                    formPRProduce.Show();
                    break;
                case "530":
                    FormProduceComplete formProduceComplete = new FormProduceComplete();
                    formProduceComplete.MdiParent = form;
                    formProduceComplete.StartPosition = FormStartPosition.CenterScreen;
                    formProduceComplete.Tag = menuItem.Tag.ToString();
                    formProduceComplete.Show();
                    break;
                case "540":
                    FormPRInStore formPRInStore = new FormPRInStore();
                    formPRInStore.MdiParent = form;
                    formPRInStore.StartPosition = FormStartPosition.CenterScreen;
                    formPRInStore.Tag = menuItem.Tag.ToString();
                    formPRInStore.Show();
                    break;
                case "610":
                    FormCustomerCourse formCustomerCourse = new FormCustomerCourse();
                    formCustomerCourse.MdiParent = form;
                    formCustomerCourse.StartPosition = FormStartPosition.CenterScreen;
                    formCustomerCourse.Tag = menuItem.Tag.ToString();
                    formCustomerCourse.Show();
                    break;
                case "620":
                    FormBaseType formBaseType = new FormBaseType();
                    formBaseType.MdiParent = form;
                    formBaseType.StartPosition = FormStartPosition.CenterScreen;
                    formBaseType.Tag = menuItem.Tag.ToString();
                    formBaseType.Show();
                    break;
                case "630":
                    FormCustomerAnalyse formCustomerAnalyse = new FormCustomerAnalyse();
                    formCustomerAnalyse.MdiParent = form;
                    formCustomerAnalyse.StartPosition = FormStartPosition.CenterScreen;
                    formCustomerAnalyse.Tag = menuItem.Tag.ToString();
                    formCustomerAnalyse.Show();
                    break;
                case "710":
                    FormFIDeposit formFIDeposit = new FormFIDeposit();
                    formFIDeposit.MdiParent = form;
                    formFIDeposit.StartPosition = FormStartPosition.CenterScreen;
                    formFIDeposit.Tag = menuItem.Tag.ToString();
                    formFIDeposit.Show();
                    break;
                case "720":
                    FormFIPurCost formFIPurCost = new FormFIPurCost();
                    formFIPurCost.MdiParent = form;
                    formFIPurCost.StartPosition = FormStartPosition.CenterScreen;
                    formFIPurCost.Tag = menuItem.Tag.ToString();
                    formFIPurCost.Show();
                    break;
                case "730":
                    FormFISelCost formFISelCost = new FormFISelCost();
                    formFISelCost.MdiParent = form;
                    formFISelCost.StartPosition = FormStartPosition.CenterScreen;
                    formFISelCost.Tag = menuItem.Tag.ToString();
                    formFISelCost.Show();
                    break;
                case "910":
                    FormSYOperator formSYOperator = new FormSYOperator();
                    formSYOperator.MdiParent = form;
                    formSYOperator.StartPosition = FormStartPosition.CenterScreen;
                    formSYOperator.Tag = menuItem.Tag.ToString();
                    formSYOperator.Show();
                    break;
                case "920":
                    FormPassWord formPassWord = new FormPassWord();
                    formPassWord.MdiParent = form;
                    formPassWord.StartPosition = FormStartPosition.CenterScreen;
                    formPassWord.Tag = menuItem.Tag.ToString();
                    formPassWord.Show();
                    break;
                case "930":
                    FormAssignRight formAssignRight = new FormAssignRight();
                    formAssignRight.MdiParent = form;
                    formAssignRight.StartPosition = FormStartPosition.CenterScreen;
                    formAssignRight.Tag = menuItem.Tag.ToString();
                    formAssignRight.Show();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 打开Form窗体（工具栏）
        /// </summary>
        public void ShowForm(string str, Form form)
        {
            switch (str)
            {
                case "采购入库单":
                    FormPUInStore puInStore = new FormPUInStore();
                    puInStore.MdiParent = form;
                    puInStore.StartPosition = FormStartPosition.CenterScreen;
                    puInStore.Show();
                    break;
                case "销售出库单":
                    FormSEOutStore formSEOutStore = new FormSEOutStore();
                    formSEOutStore.MdiParent = form;
                    formSEOutStore.StartPosition = FormStartPosition.CenterScreen;
                    formSEOutStore.Show();
                    break;
                case "库存盘点":
                    FormSTCheck formSTCheck = new FormSTCheck();
                    formSTCheck.MdiParent = form;
                    formSTCheck.StartPosition = FormStartPosition.CenterScreen;
                    formSTCheck.Show();
                    break;
                case "类型分析":
                    FormCustomerAnalyse formCustomerAnalyse = new FormCustomerAnalyse();
                    formCustomerAnalyse.MdiParent = form;
                    formCustomerAnalyse.StartPosition = FormStartPosition.CenterScreen;
                    formCustomerAnalyse.Show();
                    break;
                case "采购费用单":
                    FormFIPurCost formFIPurCost = new FormFIPurCost();
                    formFIPurCost.MdiParent = form;
                    formFIPurCost.StartPosition = FormStartPosition.CenterScreen;
                    formFIPurCost.Show();
                    break;
                case "销售费用单":
                    FormFISelCost formFISelCost = new FormFISelCost();
                    formFISelCost.MdiParent = form;
                    formFISelCost.StartPosition = FormStartPosition.CenterScreen;
                    formFISelCost.Show();
                    break;
                case "生产入库单":
                    FormPRInStore formPRInStore = new FormPRInStore();
                    formPRInStore.MdiParent = form;
                    formPRInStore.StartPosition = FormStartPosition.CenterScreen;
                    formPRInStore.Show();
                    break;
            }
        }
    }
}