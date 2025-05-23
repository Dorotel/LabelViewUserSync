namespace LabelViewUserSync
{
    partial class MainForm
    {
        private System.Windows.Forms.ComboBox comboBoxUsers;
        private System.Windows.Forms.Button buttonSync;
        private System.Windows.Forms.GroupBox groupBoxNewUser;
        private System.Windows.Forms.TextBox textBoxFirstName;
        private System.Windows.Forms.TextBox textBoxLastName;
        private System.Windows.Forms.CheckBox checkBoxShipping;
        private System.Windows.Forms.CheckBox checkBoxReceiving;
        private System.Windows.Forms.Button buttonAddUser;
        private System.Windows.Forms.GroupBox groupBoxUpdateUser;
        private System.Windows.Forms.ComboBox comboBoxUpdateUser;
        private System.Windows.Forms.CheckBox checkBoxUpdateShipping;
        private System.Windows.Forms.CheckBox checkBoxUpdateReceiving;
        private System.Windows.Forms.Button buttonUpdateUser;
        private System.Windows.Forms.Button buttonDeleteUser;
        private System.Windows.Forms.TextBox textBoxReport;
        private System.Windows.Forms.CheckBox checkBoxMisc;
        private System.Windows.Forms.CheckBox checkBoxUpdateMisc;
        private System.Windows.Forms.CheckBox checkBoxStatusShipping;
        private System.Windows.Forms.CheckBox checkBoxStatusReceiving;
        private System.Windows.Forms.CheckBox checkBoxStatusMisc;
        private System.Windows.Forms.Button buttonUpdateUserSettings;




        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            comboBoxUsers = new ComboBox();
            buttonSync = new Button();
            groupBoxNewUser = new GroupBox();
            textBoxFirstName = new TextBox();
            textBoxLastName = new TextBox();
            checkBoxShipping = new CheckBox();
            checkBoxReceiving = new CheckBox();
            buttonAddUser = new Button();
            checkBoxMisc = new CheckBox();
            groupBoxUpdateUser = new GroupBox();
            comboBoxUpdateUser = new ComboBox();
            checkBoxUpdateShipping = new CheckBox();
            checkBoxUpdateReceiving = new CheckBox();
            buttonUpdateUser = new Button();
            buttonDeleteUser = new Button();
            checkBoxUpdateMisc = new CheckBox();
            textBoxReport = new TextBox();
            checkBoxStatusShipping = new CheckBox();
            checkBoxStatusReceiving = new CheckBox();
            checkBoxStatusMisc = new CheckBox();
            buttonUpdateUserSettings = new Button();
            buttonResetLabelVersion = new Button();
            groupBoxNewUser.SuspendLayout();
            groupBoxUpdateUser.SuspendLayout();
            SuspendLayout();
            // 
            // comboBoxUsers
            // 
            comboBoxUsers.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxUsers.Location = new Point(20, 20);
            comboBoxUsers.Name = "comboBoxUsers";
            comboBoxUsers.Size = new Size(250, 23);
            comboBoxUsers.TabIndex = 0;
            // 
            // buttonSync
            // 
            buttonSync.Location = new Point(280, 20);
            buttonSync.Name = "buttonSync";
            buttonSync.Size = new Size(100, 23);
            buttonSync.TabIndex = 1;
            buttonSync.Text = "Sync";
            buttonSync.Click += buttonSync_Click;
            // 
            // groupBoxNewUser
            // 
            groupBoxNewUser.Controls.Add(textBoxFirstName);
            groupBoxNewUser.Controls.Add(textBoxLastName);
            groupBoxNewUser.Controls.Add(checkBoxShipping);
            groupBoxNewUser.Controls.Add(checkBoxReceiving);
            groupBoxNewUser.Controls.Add(buttonAddUser);
            groupBoxNewUser.Controls.Add(checkBoxMisc);
            groupBoxNewUser.Location = new Point(20, 80);
            groupBoxNewUser.Name = "groupBoxNewUser";
            groupBoxNewUser.Size = new Size(360, 120);
            groupBoxNewUser.TabIndex = 2;
            groupBoxNewUser.TabStop = false;
            groupBoxNewUser.Text = "Create New User";
            // 
            // textBoxFirstName
            // 
            textBoxFirstName.Location = new Point(10, 25);
            textBoxFirstName.Name = "textBoxFirstName";
            textBoxFirstName.PlaceholderText = "First Name";
            textBoxFirstName.Size = new Size(166, 23);
            textBoxFirstName.TabIndex = 0;
            // 
            // textBoxLastName
            // 
            textBoxLastName.Location = new Point(188, 25);
            textBoxLastName.Name = "textBoxLastName";
            textBoxLastName.PlaceholderText = "Last Name";
            textBoxLastName.Size = new Size(166, 23);
            textBoxLastName.TabIndex = 1;
            // 
            // checkBoxShipping
            // 
            checkBoxShipping.Location = new Point(10, 54);
            checkBoxShipping.Name = "checkBoxShipping";
            checkBoxShipping.Size = new Size(104, 24);
            checkBoxShipping.TabIndex = 2;
            checkBoxShipping.Text = "Shipping";
            // 
            // checkBoxReceiving
            // 
            checkBoxReceiving.Location = new Point(147, 54);
            checkBoxReceiving.Name = "checkBoxReceiving";
            checkBoxReceiving.Size = new Size(104, 24);
            checkBoxReceiving.TabIndex = 3;
            checkBoxReceiving.Text = "Receiving";
            // 
            // buttonAddUser
            // 
            buttonAddUser.Location = new Point(6, 84);
            buttonAddUser.Name = "buttonAddUser";
            buttonAddUser.Size = new Size(348, 29);
            buttonAddUser.TabIndex = 5;
            buttonAddUser.Text = "Add User";
            buttonAddUser.Click += buttonAddUser_Click;
            // 
            // checkBoxMisc
            // 
            checkBoxMisc.Location = new Point(284, 54);
            checkBoxMisc.Name = "checkBoxMisc";
            checkBoxMisc.Size = new Size(70, 24);
            checkBoxMisc.TabIndex = 4;
            checkBoxMisc.Text = "Misc";
            // 
            // groupBoxUpdateUser
            // 
            groupBoxUpdateUser.Controls.Add(comboBoxUpdateUser);
            groupBoxUpdateUser.Controls.Add(checkBoxUpdateShipping);
            groupBoxUpdateUser.Controls.Add(checkBoxUpdateReceiving);
            groupBoxUpdateUser.Controls.Add(buttonUpdateUser);
            groupBoxUpdateUser.Controls.Add(buttonDeleteUser);
            groupBoxUpdateUser.Controls.Add(checkBoxUpdateMisc);
            groupBoxUpdateUser.Location = new Point(20, 210);
            groupBoxUpdateUser.Name = "groupBoxUpdateUser";
            groupBoxUpdateUser.Size = new Size(360, 113);
            groupBoxUpdateUser.TabIndex = 3;
            groupBoxUpdateUser.TabStop = false;
            groupBoxUpdateUser.Text = "Update/Delete User";
            // 
            // comboBoxUpdateUser
            // 
            comboBoxUpdateUser.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxUpdateUser.Location = new Point(6, 22);
            comboBoxUpdateUser.Name = "comboBoxUpdateUser";
            comboBoxUpdateUser.Size = new Size(344, 23);
            comboBoxUpdateUser.TabIndex = 0;
            // 
            // checkBoxUpdateShipping
            // 
            checkBoxUpdateShipping.Location = new Point(10, 51);
            checkBoxUpdateShipping.Name = "checkBoxUpdateShipping";
            checkBoxUpdateShipping.Size = new Size(104, 24);
            checkBoxUpdateShipping.TabIndex = 1;
            checkBoxUpdateShipping.Text = "Shipping";
            // 
            // checkBoxUpdateReceiving
            // 
            checkBoxUpdateReceiving.Location = new Point(145, 51);
            checkBoxUpdateReceiving.Name = "checkBoxUpdateReceiving";
            checkBoxUpdateReceiving.Size = new Size(104, 24);
            checkBoxUpdateReceiving.TabIndex = 2;
            checkBoxUpdateReceiving.Text = "Receiving";
            // 
            // buttonUpdateUser
            // 
            buttonUpdateUser.Location = new Point(6, 81);
            buttonUpdateUser.Name = "buttonUpdateUser";
            buttonUpdateUser.Size = new Size(70, 23);
            buttonUpdateUser.TabIndex = 4;
            buttonUpdateUser.Text = "Update";
            buttonUpdateUser.Click += buttonUpdateUser_Click;
            // 
            // buttonDeleteUser
            // 
            buttonDeleteUser.Location = new Point(284, 81);
            buttonDeleteUser.Name = "buttonDeleteUser";
            buttonDeleteUser.Size = new Size(70, 23);
            buttonDeleteUser.TabIndex = 5;
            buttonDeleteUser.Text = "Delete";
            buttonDeleteUser.Click += buttonDeleteUser_Click;
            // 
            // checkBoxUpdateMisc
            // 
            checkBoxUpdateMisc.Location = new Point(284, 51);
            checkBoxUpdateMisc.Name = "checkBoxUpdateMisc";
            checkBoxUpdateMisc.Size = new Size(70, 24);
            checkBoxUpdateMisc.TabIndex = 3;
            checkBoxUpdateMisc.Text = "Misc";
            // 
            // textBoxReport
            // 
            textBoxReport.Location = new Point(20, 329);
            textBoxReport.Multiline = true;
            textBoxReport.Name = "textBoxReport";
            textBoxReport.ReadOnly = true;
            textBoxReport.ScrollBars = ScrollBars.Vertical;
            textBoxReport.Size = new Size(360, 111);
            textBoxReport.TabIndex = 4;
            // 
            // checkBoxStatusShipping
            // 
            checkBoxStatusShipping.Enabled = false;
            checkBoxStatusShipping.Location = new Point(30, 49);
            checkBoxStatusShipping.Name = "checkBoxStatusShipping";
            checkBoxStatusShipping.Size = new Size(104, 20);
            checkBoxStatusShipping.TabIndex = 5;
            checkBoxStatusShipping.Text = "Shipping";
            // 
            // checkBoxStatusReceiving
            // 
            checkBoxStatusReceiving.Enabled = false;
            checkBoxStatusReceiving.Location = new Point(160, 49);
            checkBoxStatusReceiving.Name = "checkBoxStatusReceiving";
            checkBoxStatusReceiving.Size = new Size(104, 20);
            checkBoxStatusReceiving.TabIndex = 6;
            checkBoxStatusReceiving.Text = "Receiving";
            // 
            // checkBoxStatusMisc
            // 
            checkBoxStatusMisc.Enabled = false;
            checkBoxStatusMisc.Location = new Point(304, 49);
            checkBoxStatusMisc.Name = "checkBoxStatusMisc";
            checkBoxStatusMisc.Size = new Size(70, 20);
            checkBoxStatusMisc.TabIndex = 7;
            checkBoxStatusMisc.Text = "Misc";
            // 
            // buttonUpdateUserSettings
            // 
            buttonUpdateUserSettings.Location = new Point(20, 446);
            buttonUpdateUserSettings.Name = "buttonUpdateUserSettings";
            buttonUpdateUserSettings.Size = new Size(360, 30);
            buttonUpdateUserSettings.TabIndex = 0;
            buttonUpdateUserSettings.Text = "Update Settings for Selected User";
            buttonUpdateUserSettings.Click += buttonUpdateUserSettings_Click;
            // 
            // buttonResetLabelVersion
            // 
            buttonResetLabelVersion.Location = new Point(20, 478);
            buttonResetLabelVersion.Name = "buttonResetLabelVersion";
            buttonResetLabelVersion.Size = new Size(360, 30);
            buttonResetLabelVersion.TabIndex = 8;
            buttonResetLabelVersion.Text = "Reset all Labels to Version 1.0";
            buttonResetLabelVersion.Click += buttonResetLabelVersion_Click;
            // 
            // MainForm
            // 
            ClientSize = new Size(400, 513);
            Controls.Add(buttonResetLabelVersion);
            Controls.Add(buttonUpdateUserSettings);
            Controls.Add(comboBoxUsers);
            Controls.Add(buttonSync);
            Controls.Add(groupBoxNewUser);
            Controls.Add(groupBoxUpdateUser);
            Controls.Add(textBoxReport);
            Controls.Add(checkBoxStatusShipping);
            Controls.Add(checkBoxStatusReceiving);
            Controls.Add(checkBoxStatusMisc);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "LabelView User Sync";
            groupBoxNewUser.ResumeLayout(false);
            groupBoxNewUser.PerformLayout();
            groupBoxUpdateUser.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        private Button buttonResetLabelVersion;
    }
}