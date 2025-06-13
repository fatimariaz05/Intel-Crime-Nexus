namespace CISystem
{
    partial class SignUp
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

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignUp));
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.backtologin = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_sign = new Siticone.Desktop.UI.WinForms.SiticoneButton();
            this.label6 = new System.Windows.Forms.Label();
            this.txb_pass = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txb_username = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txb_address = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txb_email = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txb_desig = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            this.lbl_name = new System.Windows.Forms.Label();
            this.txb_name = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            this.gender_select = new Siticone.Desktop.UI.WinForms.SiticoneComboBox();
            this.re_enter_pass = new System.Windows.Forms.Label();
            this.lblUsernameStatus = new System.Windows.Forms.Label();
            this.txb_re_pass = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
            this.guna2Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BackColor = System.Drawing.Color.Black;
            this.guna2Panel1.Controls.Add(this.backtologin);
            this.guna2Panel1.Controls.Add(this.label1);
            this.guna2Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.guna2Panel1.Location = new System.Drawing.Point(0, 0);
            this.guna2Panel1.Margin = new System.Windows.Forms.Padding(4);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(1225, 77);
            this.guna2Panel1.TabIndex = 4;
            // 
            // backtologin
            // 
            this.backtologin.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.backtologin.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.backtologin.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.backtologin.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.backtologin.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.backtologin.ForeColor = System.Drawing.Color.White;
            this.backtologin.Location = new System.Drawing.Point(13, 24);
            this.backtologin.Margin = new System.Windows.Forms.Padding(4);
            this.backtologin.Name = "backtologin";
            this.backtologin.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.backtologin.Size = new System.Drawing.Size(146, 36);
            this.backtologin.TabIndex = 12;
            this.backtologin.Text = "Back To Login";
            this.backtologin.Click += new System.EventHandler(this.backtologin_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(329, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 26);
            this.label1.TabIndex = 11;
            this.label1.Text = "Sign Up";
            // 
            // btn_sign
            // 
            this.btn_sign.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_sign.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_sign.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_sign.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_sign.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_sign.ForeColor = System.Drawing.Color.White;
            this.btn_sign.Location = new System.Drawing.Point(288, 543);
            this.btn_sign.Name = "btn_sign";
            this.btn_sign.Size = new System.Drawing.Size(189, 45);
            this.btn_sign.TabIndex = 53;
            this.btn_sign.Text = "Register";
            this.btn_sign.Click += new System.EventHandler(this.btn_sign_Click_1);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(415, 348);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(160, 24);
            this.label6.TabIndex = 52;
            this.label6.Text = "Enter Password";
            // 
            // txb_pass
            // 
            this.txb_pass.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txb_pass.DefaultText = "";
            this.txb_pass.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txb_pass.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txb_pass.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_pass.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_pass.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_pass.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txb_pass.ForeColor = System.Drawing.Color.Black;
            this.txb_pass.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_pass.Location = new System.Drawing.Point(419, 375);
            this.txb_pass.Name = "txb_pass";
            this.txb_pass.PasswordChar = '\0';
            this.txb_pass.PlaceholderForeColor = System.Drawing.Color.Black;
            this.txb_pass.PlaceholderText = "";
            this.txb_pass.SelectedText = "";
            this.txb_pass.Size = new System.Drawing.Size(290, 36);
            this.txb_pass.TabIndex = 51;
            this.txb_pass.TextChanged += new System.EventHandler(this.txb_pass_TextChanged_1);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(415, 253);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(162, 24);
            this.label7.TabIndex = 50;
            this.label7.Text = "Enter Username";
            // 
            // txb_username
            // 
            this.txb_username.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txb_username.DefaultText = "";
            this.txb_username.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txb_username.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txb_username.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_username.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_username.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_username.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txb_username.ForeColor = System.Drawing.Color.Black;
            this.txb_username.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_username.Location = new System.Drawing.Point(419, 280);
            this.txb_username.Name = "txb_username";
            this.txb_username.PasswordChar = '\0';
            this.txb_username.PlaceholderForeColor = System.Drawing.Color.Black;
            this.txb_username.PlaceholderText = "";
            this.txb_username.SelectedText = "";
            this.txb_username.Size = new System.Drawing.Size(290, 36);
            this.txb_username.TabIndex = 49;
            this.txb_username.TextChanged += new System.EventHandler(this.txb_username_TextChanged_1);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(415, 163);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(158, 24);
            this.label5.TabIndex = 48;
            this.label5.Text = "Choose Gender";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(38, 441);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(144, 24);
            this.label4.TabIndex = 47;
            this.label4.Text = "Enter Address";
            // 
            // txb_address
            // 
            this.txb_address.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txb_address.DefaultText = "";
            this.txb_address.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txb_address.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txb_address.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_address.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_address.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_address.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txb_address.ForeColor = System.Drawing.Color.Black;
            this.txb_address.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_address.Location = new System.Drawing.Point(42, 468);
            this.txb_address.Name = "txb_address";
            this.txb_address.PasswordChar = '\0';
            this.txb_address.PlaceholderForeColor = System.Drawing.Color.Black;
            this.txb_address.PlaceholderText = "";
            this.txb_address.SelectedText = "";
            this.txb_address.Size = new System.Drawing.Size(301, 36);
            this.txb_address.TabIndex = 46;
            this.txb_address.TextChanged += new System.EventHandler(this.txb_address_TextChanged_1);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(38, 348);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 24);
            this.label3.TabIndex = 45;
            this.label3.Text = "Enter Email";
            // 
            // txb_email
            // 
            this.txb_email.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txb_email.DefaultText = "";
            this.txb_email.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txb_email.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txb_email.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_email.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_email.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_email.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txb_email.ForeColor = System.Drawing.Color.Black;
            this.txb_email.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_email.Location = new System.Drawing.Point(42, 375);
            this.txb_email.Name = "txb_email";
            this.txb_email.PasswordChar = '\0';
            this.txb_email.PlaceholderForeColor = System.Drawing.Color.Black;
            this.txb_email.PlaceholderText = "";
            this.txb_email.SelectedText = "";
            this.txb_email.Size = new System.Drawing.Size(301, 36);
            this.txb_email.TabIndex = 44;
            this.txb_email.TextChanged += new System.EventHandler(this.txb_email_TextChanged_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(38, 253);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 24);
            this.label2.TabIndex = 43;
            this.label2.Text = "Enter Designation";
            // 
            // txb_desig
            // 
            this.txb_desig.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txb_desig.DefaultText = "";
            this.txb_desig.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txb_desig.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txb_desig.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_desig.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_desig.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_desig.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txb_desig.ForeColor = System.Drawing.Color.Black;
            this.txb_desig.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_desig.Location = new System.Drawing.Point(42, 280);
            this.txb_desig.Name = "txb_desig";
            this.txb_desig.PasswordChar = '\0';
            this.txb_desig.PlaceholderForeColor = System.Drawing.Color.Black;
            this.txb_desig.PlaceholderText = "";
            this.txb_desig.SelectedText = "";
            this.txb_desig.Size = new System.Drawing.Size(301, 36);
            this.txb_desig.TabIndex = 42;
            this.txb_desig.TextChanged += new System.EventHandler(this.txb_desig_TextChanged_1);
            // 
            // lbl_name
            // 
            this.lbl_name.AutoSize = true;
            this.lbl_name.BackColor = System.Drawing.Color.Transparent;
            this.lbl_name.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_name.ForeColor = System.Drawing.Color.White;
            this.lbl_name.Location = new System.Drawing.Point(38, 163);
            this.lbl_name.Name = "lbl_name";
            this.lbl_name.Size = new System.Drawing.Size(160, 24);
            this.lbl_name.TabIndex = 41;
            this.lbl_name.Text = "Enter Full Name";
            // 
            // txb_name
            // 
            this.txb_name.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txb_name.DefaultText = "";
            this.txb_name.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txb_name.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txb_name.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_name.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_name.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_name.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txb_name.ForeColor = System.Drawing.Color.Black;
            this.txb_name.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_name.Location = new System.Drawing.Point(42, 190);
            this.txb_name.Name = "txb_name";
            this.txb_name.PasswordChar = '\0';
            this.txb_name.PlaceholderForeColor = System.Drawing.Color.Black;
            this.txb_name.PlaceholderText = "";
            this.txb_name.SelectedText = "";
            this.txb_name.Size = new System.Drawing.Size(301, 36);
            this.txb_name.TabIndex = 40;
            this.txb_name.TextChanged += new System.EventHandler(this.txb_name_TextChanged_1);
            // 
            // gender_select
            // 
            this.gender_select.BackColor = System.Drawing.Color.Transparent;
            this.gender_select.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.gender_select.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gender_select.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.gender_select.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.gender_select.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gender_select.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.gender_select.ItemHeight = 30;
            this.gender_select.Items.AddRange(new object[] {
            "Select Gender",
            "Male",
            "Female",
            "Unknown"});
            this.gender_select.Location = new System.Drawing.Point(419, 190);
            this.gender_select.Name = "gender_select";
            this.gender_select.Size = new System.Drawing.Size(290, 36);
            this.gender_select.StartIndex = 0;
            this.gender_select.TabIndex = 39;
            this.gender_select.SelectedIndexChanged += new System.EventHandler(this.gender_select_SelectedIndexChanged_1);
            // 
            // re_enter_pass
            // 
            this.re_enter_pass.AutoSize = true;
            this.re_enter_pass.BackColor = System.Drawing.Color.Transparent;
            this.re_enter_pass.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.re_enter_pass.ForeColor = System.Drawing.Color.White;
            this.re_enter_pass.Location = new System.Drawing.Point(415, 441);
            this.re_enter_pass.Name = "re_enter_pass";
            this.re_enter_pass.Size = new System.Drawing.Size(190, 24);
            this.re_enter_pass.TabIndex = 55;
            this.re_enter_pass.Text = "Re-enter Password";
            // 
            // lblUsernameStatus
            // 
            this.lblUsernameStatus.AutoSize = true;
            this.lblUsernameStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblUsernameStatus.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsernameStatus.ForeColor = System.Drawing.Color.Red;
            this.lblUsernameStatus.Location = new System.Drawing.Point(387, 322);
            this.lblUsernameStatus.Name = "lblUsernameStatus";
            this.lblUsernameStatus.Size = new System.Drawing.Size(0, 19);
            this.lblUsernameStatus.TabIndex = 56;
            this.lblUsernameStatus.Click += new System.EventHandler(this.lblUsernameStatus_Click);
            // 
            // txb_re_pass
            // 
            this.txb_re_pass.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txb_re_pass.DefaultText = "";
            this.txb_re_pass.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txb_re_pass.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txb_re_pass.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_re_pass.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txb_re_pass.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_re_pass.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txb_re_pass.ForeColor = System.Drawing.Color.Black;
            this.txb_re_pass.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txb_re_pass.Location = new System.Drawing.Point(420, 468);
            this.txb_re_pass.Name = "txb_re_pass";
            this.txb_re_pass.PasswordChar = '\0';
            this.txb_re_pass.PlaceholderForeColor = System.Drawing.Color.Black;
            this.txb_re_pass.PlaceholderText = "";
            this.txb_re_pass.SelectedText = "";
            this.txb_re_pass.Size = new System.Drawing.Size(289, 36);
            this.txb_re_pass.TabIndex = 57;
            this.txb_re_pass.TextChanged += new System.EventHandler(this.txb_re_pass_TextChanged);
            // 
            // SignUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1225, 732);
            this.Controls.Add(this.txb_re_pass);
            this.Controls.Add(this.lblUsernameStatus);
            this.Controls.Add(this.re_enter_pass);
            this.Controls.Add(this.btn_sign);
            this.Controls.Add(this.guna2Panel1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txb_email);
            this.Controls.Add(this.txb_pass);
            this.Controls.Add(this.gender_select);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txb_name);
            this.Controls.Add(this.txb_username);
            this.Controls.Add(this.lbl_name);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txb_desig);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txb_address);
            this.Controls.Add(this.label3);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SignUp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.SignUp_Load);
            this.guna2Panel1.ResumeLayout(false);
            this.guna2Panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private System.Windows.Forms.Label label1;
        private Siticone.Desktop.UI.WinForms.SiticoneButton backtologin;
        private Siticone.Desktop.UI.WinForms.SiticoneButton btn_sign;
        private System.Windows.Forms.Label label6;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txb_pass;
        private System.Windows.Forms.Label label7;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txb_username;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txb_address;
        private System.Windows.Forms.Label label3;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txb_email;
        private System.Windows.Forms.Label label2;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txb_desig;
        private System.Windows.Forms.Label lbl_name;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txb_name;
        private Siticone.Desktop.UI.WinForms.SiticoneComboBox gender_select;
        private System.Windows.Forms.Label re_enter_pass;
        private System.Windows.Forms.Label lblUsernameStatus;
        private Siticone.Desktop.UI.WinForms.SiticoneTextBox txb_re_pass;
    }
}