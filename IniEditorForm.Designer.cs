namespace LabelViewUserSync
{
    partial class IniEditorForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageInitialize;
        private System.Windows.Forms.TabPage tabPagePath;
        private System.Windows.Forms.TabPage tabPageOther;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.ToolTip toolTip;

        // Controls for [Initialize]
        private System.Windows.Forms.TextBox textBoxWizardsUsed;
        private System.Windows.Forms.CheckBox checkBoxDatabase;
        private System.Windows.Forms.CheckBox checkBoxLabelUnit;
        private System.Windows.Forms.CheckBox checkBoxAnimateUI;
        private System.Windows.Forms.TextBox textBoxViewColor;
        private System.Windows.Forms.TextBox textBoxPrinter;
        private System.Windows.Forms.CheckBox checkBoxLoadCOM;
        private System.Windows.Forms.CheckBox checkBoxDisplayPrintMode;
        private System.Windows.Forms.ComboBox comboBoxPrintDialogStartTab;
        private System.Windows.Forms.CheckBox checkBoxUpdateDateDuringPrinting;
        private System.Windows.Forms.CheckBox checkBoxPrintKeepDlgOpen;
        private System.Windows.Forms.CheckBox checkBoxBackup;
        private System.Windows.Forms.CheckBox checkBoxDrawOnMove;

        // Controls for [Path] (just a few as example)
        private System.Windows.Forms.TextBox textBoxDocument;
        private System.Windows.Forms.TextBox textBoxImage;
        private System.Windows.Forms.TextBox textBoxOdbc;

        // Controls for [FILLER]
        private System.Windows.Forms.TextBox textBoxDlgColorFrom;
        private System.Windows.Forms.TextBox textBoxDlgColorTo;

        // Controls for [Formule]
        private System.Windows.Forms.TextBox textBoxEuro;

        // Controls for [RULEBAR]
        private System.Windows.Forms.TextBox textBoxRuleBarColor;
        private System.Windows.Forms.TextBox textBoxRuleBarTextColor;

        // Controls for [DIALOGS]
        private System.Windows.Forms.TextBox textBoxFont;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageInitialize = new System.Windows.Forms.TabPage();
            this.tabPagePath = new System.Windows.Forms.TabPage();
            this.tabPageOther = new System.Windows.Forms.TabPage();
            this.buttonSave = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);

            // [Initialize] controls
            this.textBoxWizardsUsed = new System.Windows.Forms.TextBox();
            this.checkBoxDatabase = new System.Windows.Forms.CheckBox();
            this.checkBoxLabelUnit = new System.Windows.Forms.CheckBox();
            this.checkBoxAnimateUI = new System.Windows.Forms.CheckBox();
            this.textBoxViewColor = new System.Windows.Forms.TextBox();
            this.textBoxPrinter = new System.Windows.Forms.TextBox();
            this.checkBoxLoadCOM = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayPrintMode = new System.Windows.Forms.CheckBox();
            this.comboBoxPrintDialogStartTab = new System.Windows.Forms.ComboBox();
            this.checkBoxUpdateDateDuringPrinting = new System.Windows.Forms.CheckBox();
            this.checkBoxPrintKeepDlgOpen = new System.Windows.Forms.CheckBox();
            this.checkBoxBackup = new System.Windows.Forms.CheckBox();
            this.checkBoxDrawOnMove = new System.Windows.Forms.CheckBox();

            // [Path] controls (examples)
            this.textBoxDocument = new System.Windows.Forms.TextBox();
            this.textBoxImage = new System.Windows.Forms.TextBox();
            this.textBoxOdbc = new System.Windows.Forms.TextBox();

            // [FILLER]
            this.textBoxDlgColorFrom = new System.Windows.Forms.TextBox();
            this.textBoxDlgColorTo = new System.Windows.Forms.TextBox();

            // [Formule]
            this.textBoxEuro = new System.Windows.Forms.TextBox();

            // [RULEBAR]
            this.textBoxRuleBarColor = new System.Windows.Forms.TextBox();
            this.textBoxRuleBarTextColor = new System.Windows.Forms.TextBox();

            // [DIALOGS]
            this.textBoxFont = new System.Windows.Forms.TextBox();

            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageInitialize);
            this.tabControl.Controls.Add(this.tabPagePath);
            this.tabControl.Controls.Add(this.tabPageOther);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl.Size = new System.Drawing.Size(584, 400);

            // 
            // tabPageInitialize
            // 
            this.tabPageInitialize.Text = "Initialize";
            this.tabPageInitialize.Controls.Add(this.textBoxWizardsUsed);
            this.tabPageInitialize.Controls.Add(this.checkBoxDatabase);
            this.tabPageInitialize.Controls.Add(this.checkBoxLabelUnit);
            this.tabPageInitialize.Controls.Add(this.checkBoxAnimateUI);
            this.tabPageInitialize.Controls.Add(this.textBoxViewColor);
            this.tabPageInitialize.Controls.Add(this.textBoxPrinter);
            this.tabPageInitialize.Controls.Add(this.checkBoxLoadCOM);
            this.tabPageInitialize.Controls.Add(this.checkBoxDisplayPrintMode);
            this.tabPageInitialize.Controls.Add(this.comboBoxPrintDialogStartTab);
            this.tabPageInitialize.Controls.Add(this.checkBoxUpdateDateDuringPrinting);
            this.tabPageInitialize.Controls.Add(this.checkBoxPrintKeepDlgOpen);
            this.tabPageInitialize.Controls.Add(this.checkBoxBackup);
            this.tabPageInitialize.Controls.Add(this.checkBoxDrawOnMove);

            // 
            // tabPagePath
            // 
            this.tabPagePath.Text = "Path";
            this.tabPagePath.Controls.Add(this.textBoxDocument);
            this.tabPagePath.Controls.Add(this.textBoxImage);
            this.tabPagePath.Controls.Add(this.textBoxOdbc);

            // 
            // tabPageOther
            // 
            this.tabPageOther.Text = "Other";
            this.tabPageOther.Controls.Add(this.textBoxDlgColorFrom);
            this.tabPageOther.Controls.Add(this.textBoxDlgColorTo);
            this.tabPageOther.Controls.Add(this.textBoxEuro);
            this.tabPageOther.Controls.Add(this.textBoxRuleBarColor);
            this.tabPageOther.Controls.Add(this.textBoxRuleBarTextColor);
            this.tabPageOther.Controls.Add(this.textBoxFont);

            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(497, 420);
            this.buttonSave.Size = new System.Drawing.Size(75, 30);
            this.buttonSave.Text = "Save";
            this.buttonSave.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);

            // 
            // Controls layout and tooltips for [Initialize]
            // 
            int y = 20;
            int labelWidth = 120;
            int controlWidth = 200;

            AddLabeledControl(this.tabPageInitialize, "WizardsUsed:", this.textBoxWizardsUsed, "Comma-separated list of wizards used.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageInitialize, "Database:", this.checkBoxDatabase, "Enable (1) or disable (0) database support.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageInitialize, "LabelUnit:", this.checkBoxLabelUnit, "Enable (1) or disable (0) label units.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageInitialize, "AnimateUI:", this.checkBoxAnimateUI, "Enable (1) or disable (0) UI animation.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageInitialize, "ViewColor:", this.textBoxViewColor, "UI view color (integer RGB).", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageInitialize, "Printer:", this.textBoxPrinter, "Default printer name and address.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageInitialize, "LoadCOM:", this.checkBoxLoadCOM, "Enable (1) or disable (0) COM loading.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageInitialize, "DisplayPrintMode:", this.checkBoxDisplayPrintMode, "Show print mode dialog (1) or not (0).", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageInitialize, "PrintDialogStartTab:", this.comboBoxPrintDialogStartTab, "Tab to show first in print dialog.", ref y, labelWidth, controlWidth);
            this.comboBoxPrintDialogStartTab.Items.AddRange(new object[] { "1", "2", "3" });
            AddLabeledControl(this.tabPageInitialize, "UpdateDateDuringPrinting:", this.checkBoxUpdateDateDuringPrinting, "Update date field during printing.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageInitialize, "PrintKeepDlgOpen:", this.checkBoxPrintKeepDlgOpen, "Keep print dialog open after printing.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageInitialize, "Backup:", this.checkBoxBackup, "Enable (1) or disable (0) backup.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageInitialize, "DrawOnMove:", this.checkBoxDrawOnMove, "Enable (1) or disable (0) draw on move.", ref y, labelWidth, controlWidth);

            // [Path] controls (examples)
            y = 20;
            AddLabeledControl(this.tabPagePath, "Document:", this.textBoxDocument, "Path to document folder.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPagePath, "Image:", this.textBoxImage, "Path to images folder.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPagePath, "Odbc:", this.textBoxOdbc, "Path to ODBC folder.", ref y, labelWidth, controlWidth);

            // [Other] controls
            y = 20;
            AddLabeledControl(this.tabPageOther, "DlgColorFrom:", this.textBoxDlgColorFrom, "Dialog gradient start color.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageOther, "DlgColorTo:", this.textBoxDlgColorTo, "Dialog gradient end color.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageOther, "Euro:", this.textBoxEuro, "Euro conversion rate.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageOther, "RuleBar Color:", this.textBoxRuleBarColor, "Rule bar background color.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageOther, "RuleBar TextColor:", this.textBoxRuleBarTextColor, "Rule bar text color.", ref y, labelWidth, controlWidth);
            AddLabeledControl(this.tabPageOther, "Font:", this.textBoxFont, "Dialog font settings.", ref y, labelWidth, controlWidth);

            // 
            // IniEditorForm
            // 
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.buttonSave);
            this.Name = "IniEditorForm";
            this.Text = "Edit User.ini";
            this.ResumeLayout(false);
        }

        private void AddLabeledControl(System.Windows.Forms.Control parent, string labelText, System.Windows.Forms.Control control, string tooltip, ref int y, int labelWidth, int controlWidth)
        {
            var label = new System.Windows.Forms.Label();
            label.Text = labelText;
            label.Location = new System.Drawing.Point(10, y + 3);
            label.Size = new System.Drawing.Size(labelWidth, 20);
            control.Location = new System.Drawing.Point(10 + labelWidth, y);
            control.Size = new System.Drawing.Size(controlWidth, 20);
            parent.Controls.Add(label);
            parent.Controls.Add(control);
            toolTip.SetToolTip(control, tooltip);
            y += 28;
        }
    }
}