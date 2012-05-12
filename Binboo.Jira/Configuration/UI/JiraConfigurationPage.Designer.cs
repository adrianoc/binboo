namespace ConfigurationDialog
{
	partial class JiraConfigurationPage
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtServerURL = new System.Windows.Forms.TextBox();
			this.lblServerURL = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblRetypePassword = new System.Windows.Forms.Label();
			this.txtRetypePassword = new System.Windows.Forms.TextBox();
			this.lblPassword = new System.Windows.Forms.Label();
			this.lblUser = new System.Windows.Forms.Label();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.txtUser = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtServerURL
			// 
			this.txtServerURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtServerURL.Location = new System.Drawing.Point(73, 46);
			this.txtServerURL.Name = "txtServerURL";
			this.txtServerURL.Size = new System.Drawing.Size(178, 20);
			this.txtServerURL.TabIndex = 2;
			// 
			// lblServerURL
			// 
			this.lblServerURL.AutoSize = true;
			this.lblServerURL.Location = new System.Drawing.Point(3, 49);
			this.lblServerURL.Name = "lblServerURL";
			this.lblServerURL.Size = new System.Drawing.Size(63, 13);
			this.lblServerURL.TabIndex = 3;
			this.lblServerURL.Text = "Server URL";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.lblRetypePassword);
			this.groupBox1.Controls.Add(this.txtRetypePassword);
			this.groupBox1.Controls.Add(this.lblPassword);
			this.groupBox1.Controls.Add(this.lblUser);
			this.groupBox1.Controls.Add(this.txtPassword);
			this.groupBox1.Controls.Add(this.txtUser);
			this.groupBox1.Location = new System.Drawing.Point(7, 103);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(285, 158);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Autentication";
			// 
			// lblRetypePassword
			// 
			this.lblRetypePassword.AutoSize = true;
			this.lblRetypePassword.Location = new System.Drawing.Point(18, 116);
			this.lblRetypePassword.Name = "lblRetypePassword";
			this.lblRetypePassword.Size = new System.Drawing.Size(41, 13);
			this.lblRetypePassword.TabIndex = 11;
			this.lblRetypePassword.Text = "Retype";
			// 
			// txtRetypePassword
			// 
			this.txtRetypePassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtRetypePassword.Location = new System.Drawing.Point(66, 113);
			this.txtRetypePassword.Name = "txtRetypePassword";
			this.txtRetypePassword.PasswordChar = '*';
			this.txtRetypePassword.Size = new System.Drawing.Size(178, 20);
			this.txtRetypePassword.TabIndex = 10;
			// 
			// lblPassword
			// 
			this.lblPassword.AutoSize = true;
			this.lblPassword.Location = new System.Drawing.Point(7, 81);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(53, 13);
			this.lblPassword.TabIndex = 9;
			this.lblPassword.Text = "Password";
			// 
			// lblUser
			// 
			this.lblUser.AutoSize = true;
			this.lblUser.Location = new System.Drawing.Point(31, 45);
			this.lblUser.Name = "lblUser";
			this.lblUser.Size = new System.Drawing.Size(29, 13);
			this.lblUser.TabIndex = 8;
			this.lblUser.Text = "User";
			// 
			// txtPassword
			// 
			this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtPassword.Location = new System.Drawing.Point(66, 78);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(178, 20);
			this.txtPassword.TabIndex = 7;
			// 
			// txtUser
			// 
			this.txtUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtUser.Location = new System.Drawing.Point(66, 42);
			this.txtUser.Name = "txtUser";
			this.txtUser.Size = new System.Drawing.Size(178, 20);
			this.txtUser.TabIndex = 6;
			// 
			// JiraConfigurationPage
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.lblServerURL);
			this.Controls.Add(this.txtServerURL);
			this.Name = "JiraConfigurationPage";
			this.Size = new System.Drawing.Size(278, 304);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtServerURL;
		private System.Windows.Forms.Label lblServerURL;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.Label lblUser;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.TextBox txtUser;
		private System.Windows.Forms.Label lblRetypePassword;
		private System.Windows.Forms.TextBox txtRetypePassword;
	}
}
