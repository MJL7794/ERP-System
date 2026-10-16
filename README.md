1. 请自行配置 app.config 中的 sql 数据库连接字符串，才能运行本项目
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <connectionStrings>
    <add name="ERPConn" 
         connectionString="Server=YOUR_SERVER;Database=db_ERP;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;" 
         providerName="Microsoft.Data.SqlClient"/>
  </connectionStrings>
</configuration>
2.目前还在学习中，项目还存在一些不足 (包括部分模块还存在 sql 注入风险，管理分类模块还在待完善等问题)
3.以提供 db_ERP.sql 创建数据库，可以选择 ERP.ini 来连接