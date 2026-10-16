using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text;
//

namespace ERP.DataClass
{
    public class DataBase
    {
        // 连接字符串，从 app.config 的 connectionStrings 里读取
        private readonly string _connStr;

        /// <summary>
        /// 构造函数：从配置文件读取连接字符串
        /// </summary>
        public DataBase()
        {
            // 方式 1：从 app.config 读
            _connStr = ConfigurationManager.ConnectionStrings["ERPConn"]?.ConnectionString;
            
            // 如果没配置，抛出异常提示
            if (string.IsNullOrEmpty(_connStr))
            {
                throw new Exception("未找到名为 ERPConn 的连接字符串，请检查 app.config。");
            }

            // 方式 2（可选）：从 ERP.ini 读，然后把值拼成连接字符串
            // string strServer = OperatorFile.GetIniFileString("DataBase", "Server", "", Application.StartupPath + "\\ERP.ini");
            // string strUserID = OperatorFile.GetIniFileString("DataBase", "UserID", "", Application.StartupPath + "\\ERP.ini");
            // string strPwd = OperatorFile.GetIniFileString("DataBase", "Pwd", "", Application.StartupPath + "\\ERP.ini");
            // string strDB = OperatorFile.GetIniFileString("DataBase", "Database", "db_ERP", Application.StartupPath + "\\ERP.ini");
            //
            // SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            // builder.DataSource = strServer;
            // builder.InitialCatalog = strDB;
            // builder.UserID = strUserID;
            // builder.Password = strPwd;
            // _connStr = builder.ConnectionString;
        }

        /// <summary>
        /// 通过 Transact-SQL 语句提交数据（增、删、改）
        /// </summary>
        /// <param name="strSql">Transact-SQL 语句</param>
        /// <param name="parameters">参数数组（可选，推荐传，防止 SQL 注入）</param>
        /// <returns>受影响的行数</returns>
        /// ExecDataBySql 方法用于执行 SQL 语句（增删改）
        public int ExecDataBySql(string strSql, SqlParameter[] parameters = null)//SqlParameter[] parameters是可选的参数数组，用于传递 SQL 语句中的参数值，防止 SQL 注入攻击。（INSERT INTO Users(Name, Age, City) VALUES(@Name, @Age, @City)3 个参数）
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.CommandType = CommandType.Text;

                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// 获取 SqlDataAdapter（用于 DataTable 批量更新）
        /// </summary>
        public SqlDataAdapter GetDataAdapter(string strSql, string strTable)
        {
            SqlConnection conn = new SqlConnection(_connStr);
            SqlDataAdapter sda = new SqlDataAdapter(strSql, conn);
            new SqlCommandBuilder(sda);  // 自动生成 Update/Insert/Delete 命令
            return sda;
        }


        /// <summary>
        /// 执行多条 SQL（事务），支持参数
        /// </summary>
        public bool ExecDataBySqls(List<(string Sql, SqlParameter[] Parameters)> commands)
        {
            if (commands == null || commands.Count == 0)
                return false;

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var cmd in commands)
                        {
                            using (SqlCommand command = new SqlCommand(cmd.Sql, conn, tran))
                            {
                                if (cmd.Parameters != null)
                                    command.Parameters.AddRange(cmd.Parameters);
                                command.ExecuteNonQuery();
                            }
                        }
                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();

                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 通过多条 Transact-SQL 语句提交数据（事务）
        /// </summary>
        /// <param name="strSqls">多条 SQL 语句的集合</param>
        /// <returns>提交是否成功</returns>
        public bool ExecDataBySqls(List<string> strSqls)
        {
            if (strSqls == null || strSqls.Count == 0)
                return false;

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();

                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (string sql in strSqls)
                        {
                            using (SqlCommand cmd = new SqlCommand(sql, conn, tran))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();   // 提交事务
                        return true;
                    }
                    catch
                    {
                        tran.Rollback(); // 回滚事务

                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 通过 Transact-SQL 语句得到 DataSet 实例
        /// </summary>
        /// <param name="strSql">Transact-SQL 语句</param>
        /// <param name="strTable">数据表名称</param>
        /// <param name="parameters">参数数组（可选）</param>
        /// <returns>DataSet 实例</returns>
        public DataSet GetDataSet(string strSql, string strTable, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    sda.Fill(ds, strTable);//fill 方法将数据填充到 DataSet 中，并指定数据表的名称为 strTable,fill会自动打开和关闭连接，所以不用手动open
                    return ds;
                }
            }
        }

        /// <summary>
        /// 通过 Transact-SQL 语句得到 SqlDataReader 实例
        /// 注意：调用方用完 reader 后必须 Close 或 Dispose
        /// </summary>
        /// <param name="strSql">Transact-SQL 语句</param>
        /// <param name="parameters">参数数组（可选）</param>
        /// <returns>SqlDataReader 实例</returns>
        public SqlDataReader GetDataReader(string strSql, SqlParameter[] parameters = null)
        {
            SqlConnection conn = new SqlConnection(_connStr);
            SqlCommand cmd = new SqlCommand(strSql, conn)
            {
                CommandType = CommandType.Text
            };

            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }

            try
            {
                conn.Open();
                // CommandBehavior.CloseConnection：reader 关闭时自动关闭连接
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);//用来做大数据SQL执行查询
            }
            catch
            {
                conn.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 执行 Transact-SQL 语句，返回结果集第一行第一列
        /// </summary>
        /// <param name="strSql">Transact-SQL 语句</param>
        /// <param name="parameters">参数数组（可选）</param>
        /// <returns>结果集第一行第一列</returns>
        public object GetSingleObject(string strSql, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                cmd.CommandType = CommandType.Text;

                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// 通过 Transact-SQL 语句得到 DataTable 实例
        /// </summary>
        /// <param name="strSqlCode">Transact-SQL 语句</param>
        /// <param name="strTableName">数据表名称</param>
        /// <param name="parameters">参数数组（可选）</param>
        /// <returns>DataTable 实例</returns>
        public DataTable GetDataTable(string strSqlCode, string strTableName, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(strSqlCode, conn))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable(strTableName);
                    sda.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// 通过存储过程得到 DataTable 实例
        /// </summary>
        /// <param name="strProcedureName">存储过程名称</param>
        /// <param name="inputParameters">存储过程参数数组</param>
        /// <returns>DataTable 实例</returns>
        public DataTable GetDataTable(string strProcedureName, SqlParameter[] inputParameters)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(strProcedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (inputParameters != null && inputParameters.Length > 0)
                {
                    foreach (SqlParameter param in inputParameters)
                    {
                        param.Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(param);
                    }
                }

                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    return dt;
                }
            }
        }
    }
}
