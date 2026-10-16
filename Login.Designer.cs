namespace ERP
{
    partial class Login
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            errInfo = new ErrorProvider(components);
            txtPwd = new TextBox();
            txtCode = new TextBox();
            picLogin = new PictureBox();
            picReset = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)errInfo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picLogin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picReset).BeginInit();
            SuspendLayout();
            // 
            // errInfo
            // 
            errInfo.ContainerControl = this;
            // 
            // txtPwd
            // 
            txtPwd.BorderStyle = BorderStyle.None;
            txtPwd.Font = new Font("宋体", 16F, FontStyle.Regular, GraphicsUnit.Point, 134);
            txtPwd.Location = new Point(105, 384);
            txtPwd.Margin = new Padding(5);
            txtPwd.MaxLength = 20;
            txtPwd.Multiline = true;
            txtPwd.Name = "txtPwd";
            txtPwd.PasswordChar = '*';
            txtPwd.PlaceholderText = "密码";
            txtPwd.Size = new Size(427, 55);
            txtPwd.TabIndex = 2;
            txtPwd.KeyDown += txtPwd_KeyDown;
            // 
            // txtCode
            // 
            txtCode.BorderStyle = BorderStyle.None;
            txtCode.Font = new Font("宋体", 16F, FontStyle.Regular, GraphicsUnit.Point, 134);
            txtCode.Location = new Point(106, 297);
            txtCode.Margin = new Padding(5);
            txtCode.MaxLength = 10;
            txtCode.Multiline = true;
            txtCode.Name = "txtCode";
            txtCode.PlaceholderText = "用户名/手机号码";
            txtCode.Size = new Size(426, 57);
            txtCode.TabIndex = 1;
            txtCode.TextChanged += txtCode_TextChanged;
            txtCode.KeyDown += txtCode_KeyDown;
            // 
            // picLogin
            // 
            picLogin.BackColor = Color.Transparent;
            picLogin.BackgroundImage = (Image)resources.GetObject("picLogin.BackgroundImage");
            picLogin.BackgroundImageLayout = ImageLayout.Zoom;
            picLogin.Cursor = Cursors.Hand;
            picLogin.Location = new Point(92, 472);
            picLogin.Margin = new Padding(5);
            picLogin.Name = "picLogin";
            picLogin.Size = new Size(440, 60);
            picLogin.TabIndex = 4;
            picLogin.TabStop = false;
            picLogin.Click += picLogin_Click;
            // 
            // picReset
            // 
            picReset.BackColor = Color.Transparent;
            picReset.BackgroundImage = Properties.Resources.重置按钮;
            picReset.BackgroundImageLayout = ImageLayout.Stretch;
            picReset.Cursor = Cursors.Hand;
            picReset.Location = new Point(92, 558);
            picReset.Margin = new Padding(5);
            picReset.Name = "picReset";
            picReset.Size = new Size(440, 60);
            picReset.TabIndex = 5;
            picReset.TabStop = false;
            picReset.Click += picReset_Click;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(1315, 768);
            Controls.Add(picReset);
            Controls.Add(picLogin);
            Controls.Add(txtCode);
            Controls.Add(txtPwd);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Login";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "用户登录";
            Load += Login_Load;
            ((System.ComponentModel.ISupportInitialize)errInfo).EndInit();
            ((System.ComponentModel.ISupportInitialize)picLogin).EndInit();
            ((System.ComponentModel.ISupportInitialize)picReset).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ErrorProvider errInfo;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.PictureBox picLogin;
        private System.Windows.Forms.PictureBox picReset;
    }
}

