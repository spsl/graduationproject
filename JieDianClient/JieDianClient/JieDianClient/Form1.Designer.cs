namespace JieDianClient
{
    partial class Form1
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
            this.Btn_Chongzhi = new System.Windows.Forms.Button();
            this.Btn_DengLu = new System.Windows.Forms.Button();
            this.Btn_QuXiao = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.Tb_UserName = new System.Windows.Forms.TextBox();
            this.Mtb_MiMa = new System.Windows.Forms.MaskedTextBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.SuspendLayout();
            // 
            // Btn_Chongzhi
            // 
            this.Btn_Chongzhi.Location = new System.Drawing.Point(144, 126);
            this.Btn_Chongzhi.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_Chongzhi.Name = "Btn_Chongzhi";
            this.Btn_Chongzhi.Size = new System.Drawing.Size(74, 31);
            this.Btn_Chongzhi.TabIndex = 0;
            this.Btn_Chongzhi.Text = "重置";
            this.Btn_Chongzhi.UseVisualStyleBackColor = true;
            this.Btn_Chongzhi.Click += new System.EventHandler(this.Btn_Chongzhi_Click);
            // 
            // Btn_DengLu
            // 
            this.Btn_DengLu.Location = new System.Drawing.Point(36, 127);
            this.Btn_DengLu.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_DengLu.Name = "Btn_DengLu";
            this.Btn_DengLu.Size = new System.Drawing.Size(88, 31);
            this.Btn_DengLu.TabIndex = 1;
            this.Btn_DengLu.Text = "登陆";
            this.Btn_DengLu.UseVisualStyleBackColor = true;
            this.Btn_DengLu.Click += new System.EventHandler(this.Btn_DengLu_Click);
            // 
            // Btn_QuXiao
            // 
            this.Btn_QuXiao.Location = new System.Drawing.Point(245, 126);
            this.Btn_QuXiao.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_QuXiao.Name = "Btn_QuXiao";
            this.Btn_QuXiao.Size = new System.Drawing.Size(74, 33);
            this.Btn_QuXiao.TabIndex = 2;
            this.Btn_QuXiao.Text = "取消";
            this.Btn_QuXiao.UseVisualStyleBackColor = true;
            this.Btn_QuXiao.Click += new System.EventHandler(this.Btn_QuXiao_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "用户名:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 86);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "密码：";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // Tb_UserName
            // 
            this.Tb_UserName.Location = new System.Drawing.Point(94, 34);
            this.Tb_UserName.Margin = new System.Windows.Forms.Padding(2);
            this.Tb_UserName.Name = "Tb_UserName";
            this.Tb_UserName.Size = new System.Drawing.Size(152, 21);
            this.Tb_UserName.TabIndex = 5;
            this.Tb_UserName.Text = "sunsai";
            // 
            // Mtb_MiMa
            // 
            this.Mtb_MiMa.Location = new System.Drawing.Point(94, 78);
            this.Mtb_MiMa.Margin = new System.Windows.Forms.Padding(2);
            this.Mtb_MiMa.Name = "Mtb_MiMa";
            this.Mtb_MiMa.PasswordChar = '*';
            this.Mtb_MiMa.Size = new System.Drawing.Size(152, 21);
            this.Mtb_MiMa.TabIndex = 6;
            this.Mtb_MiMa.Text = "123";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(266, 86);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(53, 12);
            this.linkLabel1.TabIndex = 8;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "忘记密码";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(340, 203);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.Mtb_MiMa);
            this.Controls.Add(this.Tb_UserName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Btn_QuXiao);
            this.Controls.Add(this.Btn_DengLu);
            this.Controls.Add(this.Btn_Chongzhi);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "登陆窗口";
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_Chongzhi;
        private System.Windows.Forms.Button Btn_DengLu;
        private System.Windows.Forms.Button Btn_QuXiao;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.MaskedTextBox Mtb_MiMa;
        private System.Windows.Forms.TextBox Tb_UserName;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}

