
namespace LabelViewUserSync
{
    public partial class IniEditorForm : Form
    {
        private readonly string iniPath;
        private Dictionary<string, Dictionary<string, string>> iniData = new();

        public IniEditorForm(string iniPath)
        {
            InitializeComponent();
            this.iniPath = iniPath;
            LoadIni();
        }

        private void LoadIni()
        {
            iniData = ParseIniFile(iniPath);

            // [Initialize]
            var init = GetSection("Initialize");
            textBoxWizardsUsed.Text = GetValue(init, "WizardsUsed");
            checkBoxDatabase.Checked = GetValue(init, "Database") == "1";
            checkBoxLabelUnit.Checked = GetValue(init, "LabelUnit") == "1";
            checkBoxAnimateUI.Checked = GetValue(init, "AnimateUI") == "1";
            textBoxViewColor.Text = GetValue(init, "ViewColor");
            textBoxPrinter.Text = GetValue(init, "Printer");
            checkBoxLoadCOM.Checked = GetValue(init, "LoadCOM") == "1";
            checkBoxDisplayPrintMode.Checked = GetValue(init, "DisplayPrintMode") == "1";
            comboBoxPrintDialogStartTab.Text = GetValue(init, "PrintDialogStartTab");
            checkBoxUpdateDateDuringPrinting.Checked = GetValue(init, "UpdateDateDuringPrinting") == "1";
            checkBoxPrintKeepDlgOpen.Checked = GetValue(init, "PrintKeepDlgOpen") == "1";
            checkBoxBackup.Checked = GetValue(init, "Backup") == "1";
            checkBoxDrawOnMove.Checked = GetValue(init, "DrawOnMove") == "1";

            // [Path]
            var path = GetSection("Path");
            textBoxDocument.Text = GetValue(path, "Document");
            textBoxImage.Text = GetValue(path, "Image");
            textBoxOdbc.Text = GetValue(path, "Odbc");

            // [FILLER]
            var filler = GetSection("FILLER");
            textBoxDlgColorFrom.Text = GetValue(filler, "DlgColorFrom");
            textBoxDlgColorTo.Text = GetValue(filler, "DlgColorTo");

            // [Formule]
            var formule = GetSection("Formule");
            textBoxEuro.Text = GetValue(formule, "Euro");

            // [RULEBAR]
            var rulebar = GetSection("RULEBAR");
            textBoxRuleBarColor.Text = GetValue(rulebar, "Color");
            textBoxRuleBarTextColor.Text = GetValue(rulebar, "TextColor");

            // [DIALOGS]
            var dialogs = GetSection("DIALOGS");
            textBoxFont.Text = GetValue(dialogs, "Font");
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // [Initialize]
            SetValue("Initialize", "WizardsUsed", textBoxWizardsUsed.Text);
            SetValue("Initialize", "Database", checkBoxDatabase.Checked ? "1" : "0");
            SetValue("Initialize", "LabelUnit", checkBoxLabelUnit.Checked ? "1" : "0");
            SetValue("Initialize", "AnimateUI", checkBoxAnimateUI.Checked ? "1" : "0");
            SetValue("Initialize", "ViewColor", textBoxViewColor.Text);
            SetValue("Initialize", "Printer", textBoxPrinter.Text);
            SetValue("Initialize", "LoadCOM", checkBoxLoadCOM.Checked ? "1" : "0");
            SetValue("Initialize", "DisplayPrintMode", checkBoxDisplayPrintMode.Checked ? "1" : "0");
            SetValue("Initialize", "PrintDialogStartTab", comboBoxPrintDialogStartTab.Text);
            SetValue("Initialize", "UpdateDateDuringPrinting", checkBoxUpdateDateDuringPrinting.Checked ? "1" : "0");
            SetValue("Initialize", "PrintKeepDlgOpen", checkBoxPrintKeepDlgOpen.Checked ? "1" : "0");
            SetValue("Initialize", "Backup", checkBoxBackup.Checked ? "1" : "0");
            SetValue("Initialize", "DrawOnMove", checkBoxDrawOnMove.Checked ? "1" : "0");

            // [Path]
            SetValue("Path", "Document", textBoxDocument.Text);
            SetValue("Path", "Image", textBoxImage.Text);
            SetValue("Path", "Odbc", textBoxOdbc.Text);

            // [FILLER]
            SetValue("FILLER", "DlgColorFrom", textBoxDlgColorFrom.Text);
            SetValue("FILLER", "DlgColorTo", textBoxDlgColorTo.Text);

            // [Formule]
            SetValue("Formule", "Euro", textBoxEuro.Text);

            // [RULEBAR]
            SetValue("RULEBAR", "Color", textBoxRuleBarColor.Text);
            SetValue("RULEBAR", "TextColor", textBoxRuleBarTextColor.Text);

            // [DIALOGS]
            SetValue("DIALOGS", "Font", textBoxFont.Text);

            try
            {
                File.WriteAllLines(iniPath, SerializeIni(iniData));
                MessageBox.Show("Saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- INI Helpers ---

        private Dictionary<string, Dictionary<string, string>> ParseIniFile(string path)
        {
            var data = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(path)) return data;
            string? section = null;
            foreach (var rawLine in File.ReadAllLines(path))
            {
                var line = rawLine.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith(";")) continue;
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    section = line.Substring(1, line.Length - 2).Trim();
                    if (!data.ContainsKey(section))
                        data[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                else if (section != null && line.Contains("="))
                {
                    var idx = line.IndexOf('=');
                    var key = line.Substring(0, idx).Trim();
                    var value = line.Substring(idx + 1).Trim();
                    data[section][key] = value;
                }
            }
            return data;
        }

        private Dictionary<string, string> GetSection(string section)
        {
            if (!iniData.ContainsKey(section))
                iniData[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            return iniData[section];
        }

        private string GetValue(Dictionary<string, string> section, string key)
        {
            return section.TryGetValue(key, out var value) ? value : "";
        }

        private void SetValue(string section, string key, string value)
        {
            if (!iniData.ContainsKey(section))
                iniData[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            iniData[section][key] = value;
        }

        private IEnumerable<string> SerializeIni(Dictionary<string, Dictionary<string, string>> data)
        {
            foreach (var section in data)
            {
                yield return $"[{section.Key}]";
                foreach (var kv in section.Value)
                    yield return $"{kv.Key}={kv.Value}";
                yield return "";
            }
        }
    }
}