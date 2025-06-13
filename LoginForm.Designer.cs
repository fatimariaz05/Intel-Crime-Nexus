namespace CISystem
{
    partial class LoginForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new System.Drawing.Size(1243, 779); // whatever your fixed size is

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.lbl_UN = new System.Windows.Forms.Label();
            this.lbl_PS = new System.Windows.Forms.Label();
            this.User_choice = new System.Windows.Forms.GroupBox();
            this.rdb_officer = new System.Windows.Forms.RadioButton();
            this.rdb_public = new System.Windows.Forms.RadioButton();
            this.btn_log = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            this.txb_username = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            this.sign_option = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            this.txb_password = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            this.User_choice.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_UN
            // 
            this.lbl_UN.AutoSize = true;
            this.lbl_UN.BackColor = System.Drawing.Color.Transparent;
            this.lbl_UN.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_UN.ForeColor = System.Drawing.Color.White;
            this.lbl_UN.Location = new System.Drawing.Point(741, 239);
            this.lbl_UN.Name = "lbl_UN";
            this.lbl_UN.Size = new System.Drawing.Size(107, 21);
            this.lbl_UN.TabIndex = 0;
            this.lbl_UN.Text = "Username:";
            this.lbl_UN.Click += new System.EventHandler(this.lbl_UN_Click);
            // 
            // lbl_PS
            // 
            this.lbl_PS.AutoSize = true;
            this.lbl_PS.BackColor = System.Drawing.Color.Transparent;
            this.lbl_PS.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_PS.ForeColor = System.Drawing.Color.White;
            this.lbl_PS.Location = new System.Drawing.Point(741, 353);
            this.lbl_PS.Name = "lbl_PS";
            this.lbl_PS.Size = new System.Drawing.Size(103, 21);
            this.lbl_PS.TabIndex = 2;
            this.lbl_PS.Text = "Password:";
            this.lbl_PS.Click += new System.EventHandler(this.lbl_PS_Click);
            // 
            // User_choice
            // 
            this.User_choice.BackColor = System.Drawing.Color.Transparent;
            this.User_choice.Controls.Add(this.rdb_officer);
            this.User_choice.Controls.Add(this.rdb_public);
            this.User_choice.Font = new System.Drawing.Font("Arial Narrow", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.User_choice.ForeColor = System.Drawing.Color.Snow;
            this.User_choice.Location = new System.Drawing.Point(745, 471);
            this.User_choice.Name = "User_choice";
            this.User_choice.Size = new System.Drawing.Size(350, 97);
            this.User_choice.TabIndex = 6;
            this.User_choice.TabStop = false;
            this.User_choice.Text = "Select User Type";
            this.User_choice.Enter += new System.EventHandler(this.User_choice_Enter);
            // 
            // rdb_officer
            // 
            this.rdb_officer.AutoSize = true;
            this.rdb_officer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdb_officer.Location = new System.Drawing.Point(203, 39);
            this.rdb_officer.Name = "rdb_officer";
            this.rdb_officer.Size = new System.Drawing.Size(91, 26);
            this.rdb_officer.TabIndex = 1;
            this.rdb_officer.TabStop = true;
            this.rdb_officer.Text = "Officer";
            this.rdb_officer.UseVisualStyleBackColor = true;
            this.rdb_officer.CheckedChanged += new System.EventHandler(this.rdb_officer_CheckedChanged);
            // 
            // rdb_public
            // 
            this.rdb_public.AutoSize = true;
            this.rdb_public.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdb_public.Location = new System.Drawing.Point(45, 39);
            this.rdb_public.Name = "rdb_public";
            this.rdb_public.Size = new System.Drawing.Size(86, 26);
            this.rdb_public.TabIndex = 0;
            this.rdb_public.TabStop = true;
            this.rdb_public.Text = "Public";
            this.rdb_public.UseVisualStyleBackColor = true;
            this.rdb_public.CheckedChanged += new System.EventHandler(this.rdb_public_CheckedChanged);
            // 
            // btn_log
            // 
            this.btn_log.BorderColor = System.Drawing.Color.Transparent;
            this.btn_log.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_log.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_log.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_log.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_log.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_log.ForeColor = System.Drawing.Color.White;
            this.btn_log.Location = new System.Drawing.Point(842, 592);
            this.btn_log.Name = "btn_log";
            this.btn_log.Size = new System.Drawing.Size(159, 44);
            this.btn_log.TabIndex = 9;
            this.btn_log.Text = "Login In";
            this.btn_log.Click += new System.EventHandler(this.siticoneButton1_Click);
            // 
            // txb_username
            // 
            this.txb_username.BorderColor = System.Drawing.Color.Transparent;
            this.txb_username.BorderThickness = 0;
            this.txb_username.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txb_username.DefaultText = "";
            this.txb_username.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txb_username.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txb_username.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_username.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_username.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_username.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold);
            this.txb_username.ForeColor = System.Drawing.Color.Black;
            this.txb_username.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_username.Location = new System.Drawing.Point(745, 272);
            this.txb_username.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txb_username.Name = "txb_username";
            this.txb_username.PasswordChar = '\0';
            this.txb_username.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txb_username.PlaceholderText = "";
            this.txb_username.SelectedText = "";
            this.txb_username.Size = new System.Drawing.Size(350, 49);
            this.txb_username.TabIndex = 11;
            this.txb_username.TextChanged += new System.EventHandler(this.siticoneTextBox1_TextChanged_1);
            // 
            // sign_option
            // 
            this.sign_option.BackColor = System.Drawing.Color.Transparent;
            this.sign_option.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.sign_option.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.sign_option.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.sign_option.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.sign_option.FillColor = System.Drawing.Color.Transparent;
            this.sign_option.Font = new System.Drawing.Font("Arial", 10.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sign_option.ForeColor = System.Drawing.Color.White;
            this.sign_option.Location = new System.Drawing.Point(790, 653);
            this.sign_option.Name = "sign_option";
            this.sign_option.Size = new System.Drawing.Size(279, 31);
            this.sign_option.TabIndex = 14;
            this.sign_option.Text = "Not Registered? Sign Up";
            this.sign_option.Click += new System.EventHandler(this.sign_option_Click);
            // 
            // txb_password
            // 
            this.txb_password.BorderColor = System.Drawing.Color.Transparent;
            this.txb_password.BorderThickness = 0;
            this.txb_password.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txb_password.DefaultText = "";
            this.txb_password.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txb_password.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txb_password.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_password.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_password.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_password.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold);
            this.txb_password.ForeColor = System.Drawing.Color.Black;
            this.txb_password.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_password.Location = new System.Drawing.Point(745, 386);
            this.txb_password.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txb_password.Name = "txb_password";
            this.txb_password.PasswordChar = '\0';
            this.txb_password.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txb_password.PlaceholderText = "";
            this.txb_password.SelectedText = "";
            this.txb_password.Size = new System.Drawing.Size(350, 49);
            this.txb_password.TabIndex = 13;
            this.txb_password.TextChanged += new System.EventHandler(this.tbx_password_TextChanged);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1221, 728);
            this.Controls.Add(this.sign_option);
            this.Controls.Add(this.txb_password);
            this.Controls.Add(this.txb_username);
            this.Controls.Add(this.btn_log);
            this.Controls.Add(this.User_choice);
            this.Controls.Add(this.lbl_PS);
            this.Controls.Add(this.lbl_UN);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.User_choice.ResumeLayout(false);
            this.User_choice.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_UN;
        private System.Windows.Forms.Label lbl_PS;
        private System.Windows.Forms.GroupBox User_choice;
        private System.Windows.Forms.RadioButton rdb_officer;
        private System.Windows.Forms.RadioButton rdb_public;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btn_log;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txb_username;
        private Siticone.Desktop.UI.WinForms.SiticoneButton sign_option;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txb_password;
    }
}