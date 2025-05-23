namespace LabelViewUserSync
{
    partial class MainForm
    {
        private System.Windows.Forms.Button buttonAddUser;
        private System.Windows.Forms.Button buttonDeleteUser;
        private System.Windows.Forms.Button buttonResetLabelVersion;
        private System.Windows.Forms.Button buttonSync;
        private System.Windows.Forms.Button buttonUpdateUser;
        private System.Windows.Forms.Button buttonUpdateUserSettings;
        private System.Windows.Forms.CheckBox checkBoxMisc;
        private System.Windows.Forms.CheckBox checkBoxReceiving;
        private System.Windows.Forms.CheckBox checkBoxShipping;
        private System.Windows.Forms.CheckBox checkBoxStatusMisc;
        private System.Windows.Forms.CheckBox checkBoxStatusReceiving;
        private System.Windows.Forms.CheckBox checkBoxStatusShipping;
        private System.Windows.Forms.CheckBox checkBoxUpdateMisc;
        private System.Windows.Forms.CheckBox checkBoxUpdateReceiving;
        private System.Windows.Forms.CheckBox checkBoxUpdateShipping;
        private System.Windows.Forms.ComboBox comboBoxUpdateUser;
        private System.Windows.Forms.ComboBox comboBoxUsers;
        private System.Windows.Forms.GroupBox groupBoxNewUser;
        private System.Windows.Forms.GroupBox groupBoxUpdateUser;
        private System.Windows.Forms.TextBox textBoxFirstName;
        private System.Windows.Forms.TextBox textBoxLastName;
        private System.Windows.Forms.TextBox textBoxReport;
        private System.Windows.Forms.Label labelUserFolder;
        private System.Windows.Forms.ComboBox comboBoxOpenLabelUserFolder;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageOpenLabel;
        private System.Windows.Forms.TabPage tabPageUserSync;
        private System.Windows.Forms.Panel panelUserSync;
        private System.Windows.Forms.Panel panelOpenLbl;
        private System.Windows.Forms.Panel panelOpenExcel;
        private System.Windows.Forms.Label labelCustomerSearch;
        private System.Windows.Forms.ComboBox comboBoxCustomerFolders;
        private System.Windows.Forms.ListBox listBoxLblFiles;
        private System.Windows.Forms.Button buttonOpenLbl;
        private System.Windows.Forms.Label labelExcelFiles;
        private System.Windows.Forms.ListBox listBoxExcelFiles;
        private System.Windows.Forms.Button buttonOpenExcel;

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
            tabControlMain = new TabControl();
            tabPageOpenLabel = new TabPage();
            labelUserFolder = new Label();
            comboBoxOpenLabelUserFolder = new ComboBox();
            panelOpenLbl = new Panel();
            labelCustomerSearch = new Label();
            comboBoxCustomerFolders = new ComboBox();
            listBoxLblFiles = new ListBox();
            buttonOpenLbl = new Button();
            panelOpenExcel = new Panel();
            labelExcelFiles = new Label();
            listBoxExcelFiles = new ListBox();
            buttonOpenExcel = new Button();
            tabPageUserSync = new TabPage();
            panelUserSync = new Panel();
            buttonUpdateCoreFiles = new Button();
            groupBoxNewUser.SuspendLayout();
            groupBoxUpdateUser.SuspendLayout();
            tabControlMain.SuspendLayout();
            tabPageOpenLabel.SuspendLayout();
            panelOpenLbl.SuspendLayout();
            panelOpenExcel.SuspendLayout();
            tabPageUserSync.SuspendLayout();
            panelUserSync.SuspendLayout();
            SuspendLayout();
            // 
            // comboBoxUsers
            // 
            comboBoxUsers.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxUsers.Location = new Point(20, 20);
            comboBoxUsers.Name = "comboBoxUsers";
            comboBoxUsers.Size = new Size(614, 23);
            comboBoxUsers.TabIndex = 0;
            // 
            // buttonSync
            // 
            buttonSync.Location = new Point(640, 19);
            buttonSync.Name = "buttonSync";
            buttonSync.Size = new Size(120, 23);
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
            groupBoxNewUser.Location = new Point(166, 84);
            groupBoxNewUser.Name = "groupBoxNewUser";
            groupBoxNewUser.Size = new Size(468, 94);
            groupBoxNewUser.TabIndex = 2;
            groupBoxNewUser.TabStop = false;
            groupBoxNewUser.Text = "Create New User";
            // 
            // textBoxFirstName
            // 
            textBoxFirstName.Location = new Point(10, 25);
            textBoxFirstName.Name = "textBoxFirstName";
            textBoxFirstName.PlaceholderText = "First Name";
            textBoxFirstName.Size = new Size(220, 23);
            textBoxFirstName.TabIndex = 0;
            // 
            // textBoxLastName
            // 
            textBoxLastName.Location = new Point(236, 25);
            textBoxLastName.Name = "textBoxLastName";
            textBoxLastName.PlaceholderText = "Last Name";
            textBoxLastName.Size = new Size(220, 23);
            textBoxLastName.TabIndex = 1;
            // 
            // checkBoxShipping
            // 
            checkBoxShipping.Location = new Point(10, 60);
            checkBoxShipping.Name = "checkBoxShipping";
            checkBoxShipping.Size = new Size(120, 24);
            checkBoxShipping.TabIndex = 2;
            checkBoxShipping.Text = "Shipping";
            // 
            // checkBoxReceiving
            // 
            checkBoxReceiving.Location = new Point(140, 60);
            checkBoxReceiving.Name = "checkBoxReceiving";
            checkBoxReceiving.Size = new Size(120, 24);
            checkBoxReceiving.TabIndex = 3;
            checkBoxReceiving.Text = "Receiving";
            // 
            // buttonAddUser
            // 
            buttonAddUser.Location = new Point(356, 60);
            buttonAddUser.Name = "buttonAddUser";
            buttonAddUser.Size = new Size(100, 23);
            buttonAddUser.TabIndex = 5;
            buttonAddUser.Text = "Add User";
            buttonAddUser.Click += buttonAddUser_Click;
            // 
            // checkBoxMisc
            // 
            checkBoxMisc.Location = new Point(270, 60);
            checkBoxMisc.Name = "checkBoxMisc";
            checkBoxMisc.Size = new Size(120, 24);
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
            groupBoxUpdateUser.Location = new Point(20, 184);
            groupBoxUpdateUser.Name = "groupBoxUpdateUser";
            groupBoxUpdateUser.Size = new Size(740, 120);
            groupBoxUpdateUser.TabIndex = 3;
            groupBoxUpdateUser.TabStop = false;
            groupBoxUpdateUser.Text = "Update/Delete User";
            // 
            // comboBoxUpdateUser
            // 
            comboBoxUpdateUser.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxUpdateUser.Location = new Point(10, 25);
            comboBoxUpdateUser.Name = "comboBoxUpdateUser";
            comboBoxUpdateUser.Size = new Size(724, 23);
            comboBoxUpdateUser.TabIndex = 0;
            // 
            // checkBoxUpdateShipping
            // 
            checkBoxUpdateShipping.Location = new Point(10, 54);
            checkBoxUpdateShipping.Name = "checkBoxUpdateShipping";
            checkBoxUpdateShipping.Size = new Size(120, 24);
            checkBoxUpdateShipping.TabIndex = 1;
            checkBoxUpdateShipping.Text = "Shipping";
            // 
            // checkBoxUpdateReceiving
            // 
            checkBoxUpdateReceiving.Location = new Point(136, 54);
            checkBoxUpdateReceiving.Name = "checkBoxUpdateReceiving";
            checkBoxUpdateReceiving.Size = new Size(120, 24);
            checkBoxUpdateReceiving.TabIndex = 2;
            checkBoxUpdateReceiving.Text = "Receiving";
            // 
            // buttonUpdateUser
            // 
            buttonUpdateUser.Location = new Point(6, 91);
            buttonUpdateUser.Name = "buttonUpdateUser";
            buttonUpdateUser.Size = new Size(100, 23);
            buttonUpdateUser.TabIndex = 4;
            buttonUpdateUser.Text = "Update";
            buttonUpdateUser.Click += buttonUpdateUser_Click;
            // 
            // buttonDeleteUser
            // 
            buttonDeleteUser.Location = new Point(634, 91);
            buttonDeleteUser.Name = "buttonDeleteUser";
            buttonDeleteUser.Size = new Size(100, 23);
            buttonDeleteUser.TabIndex = 5;
            buttonDeleteUser.Text = "Delete";
            buttonDeleteUser.Click += buttonDeleteUser_Click;
            // 
            // checkBoxUpdateMisc
            // 
            checkBoxUpdateMisc.Location = new Point(262, 54);
            checkBoxUpdateMisc.Name = "checkBoxUpdateMisc";
            checkBoxUpdateMisc.Size = new Size(120, 24);
            checkBoxUpdateMisc.TabIndex = 3;
            checkBoxUpdateMisc.Text = "Misc";
            // 
            // textBoxReport
            // 
            textBoxReport.Location = new Point(20, 310);
            textBoxReport.Multiline = true;
            textBoxReport.Name = "textBoxReport";
            textBoxReport.ReadOnly = true;
            textBoxReport.ScrollBars = ScrollBars.Vertical;
            textBoxReport.Size = new Size(740, 279);
            textBoxReport.TabIndex = 4;
            // 
            // checkBoxStatusShipping
            // 
            checkBoxStatusShipping.Enabled = false;
            checkBoxStatusShipping.Location = new Point(20, 49);
            checkBoxStatusShipping.Name = "checkBoxStatusShipping";
            checkBoxStatusShipping.Size = new Size(120, 20);
            checkBoxStatusShipping.TabIndex = 5;
            checkBoxStatusShipping.Text = "Shipping";
            // 
            // checkBoxStatusReceiving
            // 
            checkBoxStatusReceiving.Enabled = false;
            checkBoxStatusReceiving.Location = new Point(146, 49);
            checkBoxStatusReceiving.Name = "checkBoxStatusReceiving";
            checkBoxStatusReceiving.Size = new Size(120, 20);
            checkBoxStatusReceiving.TabIndex = 6;
            checkBoxStatusReceiving.Text = "Receiving";
            // 
            // checkBoxStatusMisc
            // 
            checkBoxStatusMisc.Enabled = false;
            checkBoxStatusMisc.Location = new Point(272, 49);
            checkBoxStatusMisc.Name = "checkBoxStatusMisc";
            checkBoxStatusMisc.Size = new Size(120, 20);
            checkBoxStatusMisc.TabIndex = 7;
            checkBoxStatusMisc.Text = "Misc";
            // 
            // buttonUpdateUserSettings
            // 
            buttonUpdateUserSettings.Location = new Point(20, 595);
            buttonUpdateUserSettings.Name = "buttonUpdateUserSettings";
            buttonUpdateUserSettings.Size = new Size(740, 30);
            buttonUpdateUserSettings.TabIndex = 8;
            buttonUpdateUserSettings.Text = "Update Settings for Selected User";
            buttonUpdateUserSettings.Click += buttonUpdateUserSettings_Click;
            // 
            // buttonResetLabelVersion
            // 
            buttonResetLabelVersion.Location = new Point(20, 631);
            buttonResetLabelVersion.Name = "buttonResetLabelVersion";
            buttonResetLabelVersion.Size = new Size(370, 30);
            buttonResetLabelVersion.TabIndex = 9;
            buttonResetLabelVersion.Text = "Reset all Labels to Version 1.0";
            buttonResetLabelVersion.Click += buttonResetLabelVersion_Click;
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabPageOpenLabel);
            tabControlMain.Controls.Add(tabPageUserSync);
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Location = new Point(0, 0);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(800, 700);
            tabControlMain.TabIndex = 100;
            // 
            // tabPageOpenLabel
            // 
            tabPageOpenLabel.Controls.Add(labelUserFolder);
            tabPageOpenLabel.Controls.Add(comboBoxOpenLabelUserFolder);
            tabPageOpenLabel.Controls.Add(panelOpenLbl);
            tabPageOpenLabel.Controls.Add(panelOpenExcel);
            tabPageOpenLabel.Location = new Point(4, 24);
            tabPageOpenLabel.Name = "tabPageOpenLabel";
            tabPageOpenLabel.Padding = new Padding(3);
            tabPageOpenLabel.Size = new Size(792, 672);
            tabPageOpenLabel.TabIndex = 0;
            tabPageOpenLabel.Text = "Open Label";
            tabPageOpenLabel.UseVisualStyleBackColor = true;
            // 
            // labelUserFolder
            // 
            labelUserFolder.Location = new Point(10, 10);
            labelUserFolder.Name = "labelUserFolder";
            labelUserFolder.Size = new Size(80, 23);
            labelUserFolder.TabIndex = 0;
            labelUserFolder.Text = "User Folder:";
            // 
            // comboBoxOpenLabelUserFolder
            // 
            comboBoxOpenLabelUserFolder.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxOpenLabelUserFolder.Location = new Point(100, 10);
            comboBoxOpenLabelUserFolder.Name = "comboBoxOpenLabelUserFolder";
            comboBoxOpenLabelUserFolder.Size = new Size(680, 23);
            comboBoxOpenLabelUserFolder.TabIndex = 1;
            comboBoxOpenLabelUserFolder.SelectedIndexChanged += comboBoxOpenLabelUserFolder_SelectedIndexChanged;
            // 
            // panelOpenLbl
            // 
            panelOpenLbl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panelOpenLbl.BorderStyle = BorderStyle.FixedSingle;
            panelOpenLbl.Controls.Add(labelCustomerSearch);
            panelOpenLbl.Controls.Add(comboBoxCustomerFolders);
            panelOpenLbl.Controls.Add(listBoxLblFiles);
            panelOpenLbl.Controls.Add(buttonOpenLbl);
            panelOpenLbl.Location = new Point(10, 45);
            panelOpenLbl.Name = "panelOpenLbl";
            panelOpenLbl.Size = new Size(380, 617);
            panelOpenLbl.TabIndex = 2;
            // 
            // labelCustomerSearch
            // 
            labelCustomerSearch.Location = new Point(10, 10);
            labelCustomerSearch.Name = "labelCustomerSearch";
            labelCustomerSearch.Size = new Size(200, 20);
            labelCustomerSearch.TabIndex = 0;
            labelCustomerSearch.Text = "Customer (Shipping Folders):";
            // 
            // comboBoxCustomerFolders
            // 
            comboBoxCustomerFolders.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxCustomerFolders.Location = new Point(10, 35);
            comboBoxCustomerFolders.Name = "comboBoxCustomerFolders";
            comboBoxCustomerFolders.Size = new Size(350, 23);
            comboBoxCustomerFolders.TabIndex = 1;
            comboBoxCustomerFolders.SelectedIndexChanged += comboBoxCustomerFolders_SelectedIndexChanged;
            // 
            // listBoxLblFiles
            // 
            listBoxLblFiles.Location = new Point(10, 70);
            listBoxLblFiles.Name = "listBoxLblFiles";
            listBoxLblFiles.Size = new Size(350, 469);
            listBoxLblFiles.TabIndex = 2;
            listBoxLblFiles.DoubleClick += listBoxLblFiles_DoubleClick;
            // 
            // buttonOpenLbl
            // 
            buttonOpenLbl.Location = new Point(10, 560);
            buttonOpenLbl.Name = "buttonOpenLbl";
            buttonOpenLbl.Size = new Size(350, 30);
            buttonOpenLbl.TabIndex = 3;
            buttonOpenLbl.Text = "Open Selected Label";
            buttonOpenLbl.Click += buttonOpenLbl_Click;
            // 
            // panelOpenExcel
            // 
            panelOpenExcel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelOpenExcel.BorderStyle = BorderStyle.FixedSingle;
            panelOpenExcel.Controls.Add(labelExcelFiles);
            panelOpenExcel.Controls.Add(listBoxExcelFiles);
            panelOpenExcel.Controls.Add(buttonOpenExcel);
            panelOpenExcel.Location = new Point(400, 45);
            panelOpenExcel.Name = "panelOpenExcel";
            panelOpenExcel.Size = new Size(382, 617);
            panelOpenExcel.TabIndex = 3;
            // 
            // labelExcelFiles
            // 
            labelExcelFiles.Location = new Point(10, 10);
            labelExcelFiles.Name = "labelExcelFiles";
            labelExcelFiles.Size = new Size(200, 20);
            labelExcelFiles.TabIndex = 0;
            labelExcelFiles.Text = "Excel Files (Shipping):";
            // 
            // listBoxExcelFiles
            // 
            listBoxExcelFiles.Location = new Point(10, 35);
            listBoxExcelFiles.Name = "listBoxExcelFiles";
            listBoxExcelFiles.Size = new Size(350, 514);
            listBoxExcelFiles.TabIndex = 1;
            listBoxExcelFiles.DoubleClick += listBoxExcelFiles_DoubleClick;
            // 
            // buttonOpenExcel
            // 
            buttonOpenExcel.Location = new Point(10, 570);
            buttonOpenExcel.Name = "buttonOpenExcel";
            buttonOpenExcel.Size = new Size(350, 30);
            buttonOpenExcel.TabIndex = 2;
            buttonOpenExcel.Text = "Open Selected Excel";
            buttonOpenExcel.Click += buttonOpenExcel_Click;
            // 
            // tabPageUserSync
            // 
            tabPageUserSync.Controls.Add(panelUserSync);
            tabPageUserSync.Location = new Point(4, 24);
            tabPageUserSync.Name = "tabPageUserSync";
            tabPageUserSync.Padding = new Padding(3);
            tabPageUserSync.Size = new Size(792, 672);
            tabPageUserSync.TabIndex = 1;
            tabPageUserSync.Text = "User Sync";
            tabPageUserSync.UseVisualStyleBackColor = true;
            // 
            // panelUserSync
            // 
            panelUserSync.Controls.Add(buttonUpdateCoreFiles);
            panelUserSync.Controls.Add(comboBoxUsers);
            panelUserSync.Controls.Add(buttonSync);
            panelUserSync.Controls.Add(groupBoxNewUser);
            panelUserSync.Controls.Add(groupBoxUpdateUser);
            panelUserSync.Controls.Add(textBoxReport);
            panelUserSync.Controls.Add(checkBoxStatusShipping);
            panelUserSync.Controls.Add(buttonResetLabelVersion);
            panelUserSync.Controls.Add(checkBoxStatusReceiving);
            panelUserSync.Controls.Add(checkBoxStatusMisc);
            panelUserSync.Controls.Add(buttonUpdateUserSettings);
            panelUserSync.Dock = DockStyle.Fill;
            panelUserSync.Location = new Point(3, 3);
            panelUserSync.Name = "panelUserSync";
            panelUserSync.Size = new Size(786, 666);
            panelUserSync.TabIndex = 0;
            // 
            // buttonUpdateCoreFiles
            // 
            buttonUpdateCoreFiles.Location = new Point(390, 631);
            buttonUpdateCoreFiles.Name = "buttonUpdateCoreFiles";
            buttonUpdateCoreFiles.Size = new Size(370, 30);
            buttonUpdateCoreFiles.TabIndex = 10;
            buttonUpdateCoreFiles.Text = "Update Core Files for User";
            buttonUpdateCoreFiles.Click += buttonUpdateCoreFiles_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 700);
            Controls.Add(tabControlMain);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "LabelView User Sync";
            groupBoxNewUser.ResumeLayout(false);
            groupBoxNewUser.PerformLayout();
            groupBoxUpdateUser.ResumeLayout(false);
            tabControlMain.ResumeLayout(false);
            tabPageOpenLabel.ResumeLayout(false);
            panelOpenLbl.ResumeLayout(false);
            panelOpenExcel.ResumeLayout(false);
            tabPageUserSync.ResumeLayout(false);
            panelUserSync.ResumeLayout(false);
            panelUserSync.PerformLayout();
            ResumeLayout(false);
        }
        private Button buttonUpdateCoreFiles;
    }
}