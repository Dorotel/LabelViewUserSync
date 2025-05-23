using System.Text.Json;
using System.Diagnostics;
namespace LabelViewUserSync;
public partial class MainForm : Form
{
    private readonly string masterFolder = @"X:\Shipping\Labels - Labelview\Master Label Folder";
    private readonly string employeeFolderRoot = @"X:\Shipping\Labels - Labelview\Employee Folder";
    private readonly string settingsFolder = @"X:\Shipping\Labels - Labelview\SyncFiles";
    private string UsersJsonPath => Path.Combine(settingsFolder, "users.json");
    private static string LocalSettingsPath => Path.Combine(
       Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
       "LabelViewUserSync",
       "openlabelsettings.json"
   );
    private class OpenLabelSettings { public string? LastUserFolder { get; set; } }
    public class UserSettings { public string UserName { get; set; } = ""; public bool UsesShipping { get; set; } public bool UsesReceiving { get; set; } public bool UsesMisc { get; set; } }


    public MainForm()
    {
        InitializeComponent();

        buttonUpdateUserSettings.Enabled = comboBoxUsers.SelectedItem is string && !string.IsNullOrWhiteSpace(comboBoxUsers.SelectedItem.ToString());
        if (!File.Exists(UsersJsonPath))
        {
            var userFolders = Directory.GetDirectories(employeeFolderRoot)
                .Select(Path.GetFileName)
                .Where(n => !string.IsNullOrEmpty(n))
                .OrderBy(n => n)
                .ToList();

            var users = userFolders.Select(name => new UserSettings
            {
                UserName = name ?? string.Empty, // Ensure non-null value
                UsesShipping = name != null && Directory.Exists(Path.Combine(employeeFolderRoot, name, "Shipping")),
                UsesReceiving = name != null && Directory.Exists(Path.Combine(employeeFolderRoot, name, "Receiving")),
                UsesMisc = name != null && Directory.Exists(Path.Combine(employeeFolderRoot, name, "Misc"))
            }).ToList();
            SaveAllUserSettings(users);
        }
        PopulateOpenLabelTab();
        RefreshUserLists();
        comboBoxUsers.SelectedIndexChanged += comboBoxUsers_SelectedIndexChanged;
        comboBoxUpdateUser.SelectedIndexChanged += comboBoxUpdateUser_SelectedIndexChanged;
        UpdateStatusCheckboxes();
        UpdateUpdateUserCheckboxes();
    }
    private static string? ExtractLabelVersion(string fileName)
    {
        var match = System.Text.RegularExpressions.Regex.Match(
            Path.GetFileNameWithoutExtension(fileName),
            @"Ver\.?\s*(\d+(\.\d+)?)$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }
    private void SaveOpenLabelSettings(string? userFolder)
    {
        var dir = Path.GetDirectoryName(LocalSettingsPath)!;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        var settings = new OpenLabelSettings { LastUserFolder = userFolder };
        File.WriteAllText(LocalSettingsPath, JsonSerializer.Serialize(settings));
    }
    private string? LoadOpenLabelSettings()
    {
        if (!File.Exists(LocalSettingsPath)) return null;
        try { return JsonSerializer.Deserialize<OpenLabelSettings>(File.ReadAllText(LocalSettingsPath))?.LastUserFolder; }
        catch { return null; }
    }
    private void SaveAllUserSettings(List<UserSettings> users)
    {
        Directory.CreateDirectory(settingsFolder);
        File.WriteAllText(UsersJsonPath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));
    }
    private List<UserSettings> LoadAllUserSettings()
    {
        if (!File.Exists(UsersJsonPath)) return new List<UserSettings>();
        return JsonSerializer.Deserialize<List<UserSettings>>(File.ReadAllText(UsersJsonPath)) ?? new List<UserSettings>();
    }
    private void SaveUserSettings(string userName, bool usesShipping, bool usesReceiving, bool usesMisc)
    {
        var users = LoadAllUserSettings();
        var existing = users.FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            existing.UsesShipping = usesShipping;
            existing.UsesReceiving = usesReceiving;
            existing.UsesMisc = usesMisc;
        }
        else
        {
            users.Add(new UserSettings { UserName = userName, UsesShipping = usesShipping, UsesReceiving = usesReceiving, UsesMisc = usesMisc });
        }
        SaveAllUserSettings(users);
    }
    private UserSettings? LoadUserSettings(string userName)
    {
        var users = LoadAllUserSettings();
        return users.FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
    }
    private void RefreshUserLists()
    {
        var userFolders = Directory.GetDirectories(employeeFolderRoot).Select(Path.GetFileName).Where(n => !string.IsNullOrEmpty(n)).OrderBy(n => n).ToArray();
        comboBoxUsers.Items.Clear();
        comboBoxUsers.Items.AddRange(userFolders!);
        comboBoxUpdateUser.Items.Clear();
        comboBoxUpdateUser.Items.AddRange(userFolders!);
    }
    private void EnsureUserShortcut(string userFolder)
    {
        var shortcutName = "LabelView 2022 User Sync.lnk";
        var sourceShortcut = Path.Combine(employeeFolderRoot, shortcutName);
        var destShortcut = Path.Combine(userFolder, shortcutName);
        try { if (File.Exists(sourceShortcut) && !File.Exists(destShortcut)) File.Copy(sourceShortcut, destShortcut); }
        catch (Exception ex) { Debug.WriteLine($"Failed to copy shortcut: {ex.Message}"); }
    }
    private void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);
        foreach (var file in Directory.GetFiles(sourceDir)) File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)), true);
        foreach (var dir in Directory.GetDirectories(sourceDir)) CopyDirectory(dir, Path.Combine(destDir, Path.GetFileName(dir)));
    }
    private void comboBoxUsers_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateStatusCheckboxes();
        buttonUpdateUserSettings.Enabled = comboBoxUsers.SelectedItem is string userName && !string.IsNullOrWhiteSpace(userName);
    }
    private void comboBoxUpdateUser_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateUpdateUserCheckboxes();
    }
    private void UpdateStatusCheckboxes()
    {
        if (comboBoxUsers.SelectedItem is string userName && !string.IsNullOrWhiteSpace(userName))
        {
            var settings = LoadUserSettings(userName);
            checkBoxStatusShipping.Checked = settings?.UsesShipping ?? false;
            checkBoxStatusReceiving.Checked = settings?.UsesReceiving ?? false;
            checkBoxStatusMisc.Checked = settings?.UsesMisc ?? false;
        }
        else
        {
            checkBoxStatusShipping.Checked = false;
            checkBoxStatusReceiving.Checked = false;
            checkBoxStatusMisc.Checked = false;
        }
    }
    private void UpdateUpdateUserCheckboxes()
    {
        if (comboBoxUpdateUser.SelectedItem is string userName && !string.IsNullOrWhiteSpace(userName))
        {
            var settings = LoadUserSettings(userName);
            checkBoxUpdateShipping.Checked = settings?.UsesShipping ?? false;
            checkBoxUpdateReceiving.Checked = settings?.UsesReceiving ?? false;
            checkBoxUpdateMisc.Checked = settings?.UsesMisc ?? false;
        }
        else
        {
            checkBoxUpdateShipping.Checked = false;
            checkBoxUpdateReceiving.Checked = false;
            checkBoxUpdateMisc.Checked = false;
        }
    }
    private void DeleteUserSettings(string userName)
    {
        var users = LoadAllUserSettings();
        users.RemoveAll(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        SaveAllUserSettings(users);
    }
    private void buttonAddUser_Click(object sender, EventArgs e)
    {
        var firstName = textBoxFirstName.Text.Trim();
        var lastName = textBoxLastName.Text.Trim();
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            MessageBox.Show("First and Last Name are required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var userName = $"{firstName} {lastName}";
        var userFolder = Path.Combine(employeeFolderRoot, userName);
        if (Directory.Exists(userFolder))
        {
            MessageBox.Show("A user with this name already exists.", "Duplicate User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        Directory.CreateDirectory(userFolder);
        if (checkBoxShipping.Checked)
        {
            var source = Path.Combine(masterFolder, "Shipping");
            var dest = Path.Combine(userFolder, "Shipping");
            if (Directory.Exists(source)) CopyDirectory(source, dest);
        }
        if (checkBoxReceiving.Checked)
        {
            var source = Path.Combine(masterFolder, "Receiving");
            var dest = Path.Combine(userFolder, "Receiving");
            if (Directory.Exists(source)) CopyDirectory(source, dest);
        }
        if (checkBoxMisc.Checked)
        {
            var source = Path.Combine(masterFolder, "Misc");
            var dest = Path.Combine(userFolder, "Misc");
            if (Directory.Exists(source)) CopyDirectory(source, dest);
        }
        var labelViewSource = Path.Combine(masterFolder, "LabelView Folders");
        var labelViewDest = Path.Combine(userFolder, "LabelView Folders");
        if (Directory.Exists(labelViewSource)) CopyDirectory(labelViewSource, labelViewDest);
        var userIniDir = Path.Combine(userFolder, "LabelView Folders", "Settings");
        Directory.CreateDirectory(userIniDir);
        var userIniPath = Path.Combine(userIniDir, "User.ini");
        var defaultIniPath = Path.Combine(settingsFolder, "DefaultUsers.ini");
        var userFullName = $"{firstName} {lastName}";
        try
        {
            if (File.Exists(defaultIniPath))
            {
                var iniContents = File.ReadAllText(defaultIniPath);
                iniContents = iniContents.Replace("USERNAME", userFullName);
                File.WriteAllText(userIniPath, iniContents);
            }
            else
            {
                MessageBox.Show($"DefaultUsers.ini not found at:\n{defaultIniPath}", "INI File Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to create User.ini: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        SaveUserSettings(userName, checkBoxShipping.Checked, checkBoxReceiving.Checked, checkBoxMisc.Checked);
        SyncUserFolder(userFolder, masterFolder);
        MessageBox.Show("User created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        EnsureUserShortcut(userFolder);
        RefreshUserLists();
        textBoxFirstName.Text = string.Empty;
        textBoxLastName.Text = string.Empty;
        checkBoxShipping.Checked = false;
        checkBoxReceiving.Checked = false;
        checkBoxMisc.Checked = false;
    }
    private void buttonUpdateUser_Click(object sender, EventArgs e)
    {
        if (comboBoxUpdateUser.SelectedItem is null)
        {
            MessageBox.Show("Please select a user to update.", "No User Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var userName = comboBoxUpdateUser.SelectedItem.ToString()!;
        var userFolder = Path.Combine(employeeFolderRoot, userName);
        if (!checkBoxUpdateShipping.Checked)
        {
            var shippingPath = Path.Combine(userFolder, "Shipping");
            if (Directory.Exists(shippingPath)) Directory.Delete(shippingPath, true);
        }
        else
        {
            var source = Path.Combine(masterFolder, "Shipping");
            var dest = Path.Combine(userFolder, "Shipping");
            if (Directory.Exists(source)) CopyDirectory(source, dest);
        }
        if (!checkBoxUpdateReceiving.Checked)
        {
            var receivingPath = Path.Combine(userFolder, "Receiving");
            if (Directory.Exists(receivingPath)) Directory.Delete(receivingPath, true);
        }
        else
        {
            var source = Path.Combine(masterFolder, "Receiving");
            var dest = Path.Combine(userFolder, "Receiving");
            if (Directory.Exists(source)) CopyDirectory(source, dest);
        }
        if (!checkBoxUpdateMisc.Checked)
        {
            var miscPath = Path.Combine(userFolder, "Misc");
            if (Directory.Exists(miscPath)) Directory.Delete(miscPath, true);
        }
        else
        {
            var source = Path.Combine(masterFolder, "Misc");
            var dest = Path.Combine(userFolder, "Misc");
            if (Directory.Exists(source)) CopyDirectory(source, dest);
        }
        SaveUserSettings(userName, checkBoxUpdateShipping.Checked, checkBoxUpdateReceiving.Checked, checkBoxUpdateMisc.Checked);
        SyncUserFolder(userFolder, masterFolder);
        MessageBox.Show("User updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    private void buttonUpdateUserSettings_Click(object sender, EventArgs e)
    {
        if (comboBoxUsers.SelectedItem is not string userName)
        {
            MessageBox.Show("Please select a user first.", "No User Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var csIniPath = @"C:\ProgramData\Teklynx\LABELVIEW\Cs.ini";
        var newUserPath = $@"X:\Shipping\Labels - Labelview\Employee Folder\{userName}\LabelView Folders\Settings\";
        var userIniPath = Path.Combine(employeeFolderRoot, userName, "LabelView Folders", "Settings", "User.ini");
        string? userPrinter = null;
        if (File.Exists(userIniPath))
            foreach (var line in File.ReadAllLines(userIniPath))
                if (line.TrimStart().StartsWith("Printer=", StringComparison.OrdinalIgnoreCase))
                {
                    userPrinter = line.Substring(line.IndexOf('=') + 1).Trim();
                    break;
                }

        // Preview changes to Cs.ini
        List<string> lines = new();
        if (File.Exists(csIniPath))
            lines = File.ReadAllLines(csIniPath).ToList();

        string? oldUserLine = lines.FirstOrDefault(l => l.TrimStart().StartsWith("User=", StringComparison.OrdinalIgnoreCase));
        string? oldPrinterLine = lines.FirstOrDefault(l => l.TrimStart().StartsWith("Printer=", StringComparison.OrdinalIgnoreCase));
        string preview =
            $"The following changes will be made to Cs.ini:\r\n\r\n" +
            $"User line:\r\n" +
            $"  Old: {oldUserLine ?? "(none)"}\r\n" +
            $"  New: User={newUserPath}\r\n";
        if (!string.IsNullOrWhiteSpace(userPrinter))
        {
            preview += $"Printer line:\r\n" +
                       $"  Old: {oldPrinterLine ?? "(none)"}\r\n" +
                       $"  New: Printer={userPrinter}\r\n";
        }
        preview += "\r\nDo you want to proceed with updating Cs.ini?";

        var result = MessageBox.Show(preview, "Cs.ini Update Preview", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes)
        {
            textBoxReport.Text = "Cs.ini update canceled by user.";
            return;
        }

        try
        {
            bool userLineFound = false;
            for (var i = 0; i < lines.Count; i++)
                if (lines[i].TrimStart().StartsWith("User=", StringComparison.OrdinalIgnoreCase))
                {
                    lines[i] = $"User={newUserPath}";
                    userLineFound = true;
                    break;
                }
            if (!userLineFound) lines.Add($"User={newUserPath}");

            if (!string.IsNullOrWhiteSpace(userPrinter))
            {
                bool printerLineFound = false;
                for (var i = 0; i < lines.Count; i++)
                    if (lines[i].TrimStart().StartsWith("Printer=", StringComparison.OrdinalIgnoreCase))
                    {
                        lines[i] = $"Printer={userPrinter}";
                        printerLineFound = true;
                        break;
                    }
                if (!printerLineFound) lines.Add($"Printer={userPrinter}");
            }
            File.WriteAllLines(csIniPath, lines);

            textBoxReport.Text =
                "Cs.ini updated successfully.\r\n\r\n" +
                $"User line set to: User={newUserPath}\r\n" +
                (!string.IsNullOrWhiteSpace(userPrinter) ? $"Printer line set to: Printer={userPrinter}\r\n" : "");
            MessageBox.Show("Cs.ini updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            textBoxReport.Text = $"Failed to update Cs.ini: {ex.Message}";
            MessageBox.Show($"Failed to update Cs.ini: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private void buttonSync_Click(object sender, EventArgs e)
    {
        // Check if Shift key is held down
        bool shiftHeld = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;

        if (shiftHeld)
        {
            // Run for all users in comboBoxUsers
            var userNames = comboBoxUsers.Items.Cast<string>().Where(u => !string.IsNullOrWhiteSpace(u)).ToList();
            if (userNames.Count == 0)
            {
                MessageBox.Show("No users found.", "No Users", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int successCount = 0;
            var errors = new List<string>();
            foreach (var name in userNames)
            {
                var userFolderPath = Path.Combine(employeeFolderRoot, name); // Renamed variable to avoid conflict

                // Prompt for each user
                var promptMsg = $"Sync files for user '{name}'?\r\n\r\nUser folder:\r\n{userFolderPath}\r\n\r\nProceed?";
                var userResult = MessageBox.Show(promptMsg, $"Sync User: {name}", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (userResult != DialogResult.Yes)
                {
                    errors.Add($"{name}: Skipped by user.");
                    continue;
                }

                try
                {
                    var syncedFiles = SyncUserFolder(userFolderPath, masterFolder); // Updated variable name here
                    successCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"{name}: {ex.Message}");
                }
            }
            string msg = $"Sync completed for {successCount} user(s).";
            if (errors.Count > 0)
                msg += "\r\nSome errors occurred:\r\n" + string.Join("\r\n", errors);
            MessageBox.Show(msg, "Batch Sync Complete", MessageBoxButtons.OK, errors.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            return;
        }

        // Default: single user
        if (comboBoxUsers.SelectedItem is null)
        {
            MessageBox.Show("Please select a user to sync.", "No User Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var userName = comboBoxUsers.SelectedItem.ToString()!;
        var userFolder = Path.Combine(employeeFolderRoot, userName);
        var filesToSync = PreviewSyncUserFolder(userFolder, masterFolder);
        if (filesToSync.Count == 0)
        {
            textBoxReport.Text = "No files needed syncing.";
            MessageBox.Show("No files need to be updated.", "Sync Preview", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var previewMsg = "The following files will be overwritten or created:\r\n\r\n" + string.Join("\r\n", filesToSync) + "\r\n\r\nDo you want to proceed with syncing?";
        var result = MessageBox.Show(previewMsg, "Sync Preview", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            var syncedFiles = SyncUserFolder(userFolder, masterFolder);
            textBoxReport.Text = syncedFiles.Count == 0
                ? "No files needed syncing."
                : "Synced files:\r\n" + string.Join("\r\n", syncedFiles);
        }
        else
        {
            textBoxReport.Text = "Sync canceled by user.";
        }
    }
    private void buttonDeleteUser_Click(object sender, EventArgs e)
    {
        if (comboBoxUpdateUser.SelectedItem is null)
        {
            MessageBox.Show("Please select a user to delete.", "No User Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var userName = comboBoxUpdateUser.SelectedItem.ToString()!;
        var userFolder = Path.Combine(employeeFolderRoot, userName);
        var result = MessageBox.Show($"Are you sure you want to delete user '{userName}' and all their files?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (result == DialogResult.Yes)
        {
            if (Directory.Exists(userFolder)) Directory.Delete(userFolder, true);
            DeleteUserSettings(userName);
            RefreshUserLists();
            checkBoxUpdateShipping.Checked = false;
            checkBoxUpdateReceiving.Checked = false;
            checkBoxUpdateMisc.Checked = false;
            MessageBox.Show("User deleted.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
    private void comboBoxOpenLabelUserFolder_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (comboBoxOpenLabelUserFolder.SelectedItem is not string userFolder) return;
        SaveOpenLabelSettings(userFolder);

        // Update customer folders as before
        var shippingRoot = Path.Combine(employeeFolderRoot, userFolder, "Shipping");
        comboBoxCustomerFolders.Items.Clear();
        if (Directory.Exists(shippingRoot))
        {
            var customerFolders = Directory.GetDirectories(shippingRoot)
                .Select(Path.GetFileName)
                .Where(folder => !string.IsNullOrEmpty(folder))
                .ToArray();
            comboBoxCustomerFolders.Items.AddRange(customerFolders.Where(f => f != null).Cast<object>().ToArray());
            if (comboBoxCustomerFolders.Items.Count > 0) comboBoxCustomerFolders.SelectedIndex = 0;
        }
        else { listBoxLblFiles.Items.Clear(); }

        // Update Excel files to use the new Data folder
        listBoxExcelFiles.Items.Clear();
        var dataDir = Path.Combine(employeeFolderRoot, userFolder, "LabelView Folders", "Data");
        if (Directory.Exists(dataDir))
        {
            var excelFiles = Directory.GetFiles(dataDir, "*.xls*", SearchOption.AllDirectories)
                .Select(f => Path.GetRelativePath(dataDir, f)).OrderBy(f => f).ToArray();
            listBoxExcelFiles.Items.AddRange(excelFiles);
        }
    }
    private void comboBoxCustomerFolders_SelectedIndexChanged(object? sender, EventArgs e)
    {
        listBoxLblFiles.Items.Clear();
        if (comboBoxOpenLabelUserFolder.SelectedItem is not string userFolder) return;
        if (comboBoxCustomerFolders.SelectedItem is not string customerFolder) return;
        var shippingPath = Path.Combine(employeeFolderRoot, userFolder, "Shipping", customerFolder);
        if (!Directory.Exists(shippingPath)) return;
        var lblFiles = Directory.GetFiles(shippingPath, "*.lbl", SearchOption.AllDirectories)
            .Where(f => !f.Split(Path.DirectorySeparatorChar).Contains("Backups") && !f.Split(Path.AltDirectorySeparatorChar).Contains("Backups"))
            .ToArray();
        foreach (var file in lblFiles)
            listBoxLblFiles.Items.Add(Path.GetRelativePath(shippingPath, file));
    }
    private void buttonResetLabelVersion_Click(object sender, EventArgs e)
    {
        var prompt = "Enter PIN to reset all label file versions to 1.0.\n(Hint: Creator's PC PIN)";
        var input = Microsoft.VisualBasic.Interaction.InputBox(prompt, "PIN Required", "");
        if (input == null || input != "0831")
        {
            MessageBox.Show("Incorrect PIN. Operation canceled.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var confirm = MessageBox.Show("This will rename all .lbl files in both the master and all employee folders to end with 'Ver. 1.0'.\n\nAre you sure you want to proceed?", "Confirm Version Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm == DialogResult.Yes)
        {
            Cursor.Current = Cursors.WaitCursor;
            try { ResetAllLabelFileVersionsTo1_0(); MessageBox.Show("All label file versions have been reset to 1.0.", "Operation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            catch (Exception ex) { MessageBox.Show($"Error during version reset:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { Cursor.Current = Cursors.Default; }
        }
    }
    private void listBoxLblFiles_DoubleClick(object? sender, EventArgs e) => OpenSelectedLblFile();
    private void buttonOpenLbl_Click(object? sender, EventArgs e) => OpenSelectedLblFile();
    private void OpenSelectedLblFile()
    {
        if (comboBoxCustomerFolders.SelectedItem is not string customerFolder) return;
        if (listBoxLblFiles.SelectedItem is not string relPath) return;
        var shippingPath = Path.Combine(masterFolder, "Shipping", customerFolder);
        var fullPath = Path.Combine(shippingPath, relPath);
        if (File.Exists(fullPath))
        {
            Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
        }
    }
    private void listBoxExcelFiles_DoubleClick(object? sender, EventArgs e) => OpenSelectedExcelFile();
    private void buttonOpenExcel_Click(object? sender, EventArgs e) => OpenSelectedExcelFile();
    private void OpenSelectedExcelFile()
    {
        if (comboBoxOpenLabelUserFolder.SelectedItem is not string userFolder) return;
        if (listBoxExcelFiles.SelectedItem is not string relPath) return;
        var dataDir = Path.Combine(employeeFolderRoot, userFolder, "LabelView Folders", "Data");
        var fullPath = Path.Combine(dataDir, relPath);
        if (File.Exists(fullPath))
        {
            var msg =
                "Please let John Koll know you are editing this Excel file so he can sync your changes with other users.\r\n\r\n" +
                "This application cannot be open at the same time as Excel. The application will close if you continue.\r\n\r\n" +
                "Do you want to continue and open the Excel file?";
            var result = MessageBox.Show(
                msg,
                "Excel File Edit Notice",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning
            );
            if (result == DialogResult.OK)
            {
                Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
                Close();
            }
            // else: do nothing, user cancelled
        }
    }
    private void PopulateOpenLabelTab()
    {
        var userFolders = Directory.GetDirectories(employeeFolderRoot)
            .Select(Path.GetFileName)
            .Where(n => !string.IsNullOrEmpty(n))
            .OrderBy(n => n)
            .ToArray();
        comboBoxOpenLabelUserFolder.Items.Clear();
        comboBoxOpenLabelUserFolder.Items.AddRange(userFolders.Cast<object>().ToArray());
        var lastUser = LoadOpenLabelSettings();
        if (lastUser != null && comboBoxOpenLabelUserFolder.Items.Contains(lastUser)) comboBoxOpenLabelUserFolder.SelectedItem = lastUser;
        else if (comboBoxOpenLabelUserFolder.Items.Count > 0) comboBoxOpenLabelUserFolder.SelectedIndex = 0;
    }
    private List<string> SyncUserFolder(string userFolder, string masterFolder)
    {
        var syncedFiles = new List<string>();
        var errors = new List<string>();
        var userName = Path.GetFileName(userFolder);
        var settings = LoadUserSettings(userName ?? "");
        var keepShipping = settings?.UsesShipping ?? true;
        var keepReceiving = settings?.UsesReceiving ?? true;
        var keepMisc = settings?.UsesMisc ?? true;

        // 1. Sync as before (with backup on overwrite)
        foreach (var folder in new[] { "Shipping", "Receiving", "Misc" })
        {
            var keep = folder switch { "Shipping" => keepShipping, "Receiving" => keepReceiving, "Misc" => keepMisc, _ => false };
            if (!keep) continue;
            var masterSubFolder = Path.Combine(masterFolder, folder);
            if (!Directory.Exists(masterSubFolder)) continue;
            foreach (var masterFile in Directory.GetFiles(masterSubFolder, "*", SearchOption.AllDirectories))
            {
                if (string.Equals(Path.GetFileName(masterFile), "thumbs.db", StringComparison.OrdinalIgnoreCase)) continue;
                var relativePath = Path.GetRelativePath(masterFolder, masterFile);

                // Skip "Still Needs Conversion" in Shipping as before
                if (folder == "Shipping" && (
                    relativePath.StartsWith("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                    relativePath.StartsWith("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion" + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                    relativePath.Equals("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase) ||
                    relativePath.Equals("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase) ||
                    relativePath.StartsWith("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase) ||
                    relativePath.StartsWith("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase)))
                    continue;

                var userFile = Path.Combine(userFolder, relativePath);

                bool needsSync = false;
                string? reason = null;
                string? masterVer = null;
                string? userVer = null;

                if (folder == "Shipping" && Path.GetExtension(masterFile).Equals(".lbl", StringComparison.OrdinalIgnoreCase))
                {
                    masterVer = ExtractLabelVersion(masterFile);
                    userVer = File.Exists(userFile) ? ExtractLabelVersion(userFile) : null;
                    if (!File.Exists(userFile) || !string.Equals(masterVer, userVer, StringComparison.OrdinalIgnoreCase))
                    {
                        needsSync = true;
                        reason = $"version: {userVer ?? "none"} → {masterVer ?? "none"}";
                    }
                }
                else
                {
                    if (!File.Exists(userFile))
                    {
                        needsSync = true;
                        reason = "new file";
                    }
                    else if (new FileInfo(masterFile).Length != new FileInfo(userFile).Length)
                    {
                        var masterInfo = new FileInfo(masterFile);
                        var userInfo = new FileInfo(userFile);
                        needsSync = true;
                        reason = $"size: {userInfo.Length} → {masterInfo.Length}, date: {userInfo.LastWriteTime} → {masterInfo.LastWriteTime}";
                    }
                }

                if (!needsSync) continue;

                try
                {
                    // Backup old file if it exists
                    if (File.Exists(userFile))
                    {
                        var userFileDir = Path.GetDirectoryName(userFile)!;
                        var backupDir = Path.Combine(userFileDir, "Backups");
                        Directory.CreateDirectory(backupDir);
                        var backupFileName = Path.GetFileName(userFile);
                        var backupFilePath = Path.Combine(backupDir, backupFileName);

                        // If backup file exists, append timestamp
                        if (File.Exists(backupFilePath))
                        {
                            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                            var name = Path.GetFileNameWithoutExtension(backupFileName);
                            var ext = Path.GetExtension(backupFileName);
                            backupFilePath = Path.Combine(backupDir, $"{name}_{timestamp}{ext}");
                        }
                        File.Copy(userFile, backupFilePath, true);
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(userFile)!);
                    File.Copy(masterFile, userFile, true);

                    if (folder == "Shipping" && Path.GetExtension(masterFile).Equals(".lbl", StringComparison.OrdinalIgnoreCase))
                        syncedFiles.Add($"{relativePath} (version: {userVer ?? "none"} → {masterVer ?? "none"})");
                    else
                        syncedFiles.Add($"{relativePath} ({reason})");
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to sync '{relativePath}': {ex.Message}");
                }
            }
        }

        // 2. Remove (move to Backups) any extra .lbl files in user folder that are not in master
        var masterLblFiles = new HashSet<string>(
            Directory.Exists(masterFolder)
                ? Directory.GetFiles(masterFolder, "*.lbl", SearchOption.AllDirectories)
                    .Select(f => Path.GetRelativePath(masterFolder, f))
                : Enumerable.Empty<string>(),
            StringComparer.OrdinalIgnoreCase);

        foreach (var folder in new[] { "Shipping", "Receiving", "Misc" })
        {
            var userSubFolder = Path.Combine(userFolder, folder);
            if (!Directory.Exists(userSubFolder)) continue;
            foreach (var userLblFile in Directory.GetFiles(userSubFolder, "*.lbl", SearchOption.AllDirectories))
            {
                var relPath = Path.GetRelativePath(userFolder, userLblFile);
                if (!masterLblFiles.Contains(relPath))
                {
                    try
                    {
                        var dir = Path.GetDirectoryName(userLblFile)!;
                        var backupDir = Path.Combine(dir, "Backups");
                        Directory.CreateDirectory(backupDir);
                        var backupFileName = Path.GetFileName(userLblFile);
                        var backupFilePath = Path.Combine(backupDir, backupFileName);

                        // If backup file exists, append timestamp
                        if (File.Exists(backupFilePath))
                        {
                            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                            var name = Path.GetFileNameWithoutExtension(backupFileName);
                            var ext = Path.GetExtension(backupFileName);
                            backupFilePath = Path.Combine(backupDir, $"{name}_{timestamp}{ext}");
                        }
                        File.Move(userLblFile, backupFilePath);
                        syncedFiles.Add($"{relPath} (moved to Backups: not in master)");
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Failed to move extra file '{relPath}': {ex.Message}");
                    }
                }
            }
        }

        if (errors.Count > 0) MessageBox.Show(string.Join(Environment.NewLine, errors), "Sync Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        EnsureUserShortcut(userFolder);
        return syncedFiles;
    }
    private List<string> PreviewSyncUserFolder(string userFolder, string masterFolder)
    {
        var filesToSync = new List<string>();
        var userName = Path.GetFileName(userFolder);
        var settings = LoadUserSettings(userName ?? "");
        var keepShipping = settings?.UsesShipping ?? true;
        var keepReceiving = settings?.UsesReceiving ?? true;
        var keepMisc = settings?.UsesMisc ?? true;
        foreach (var folder in new[] { "Shipping", "Receiving", "Misc" })
        {
            var keep = folder switch { "Shipping" => keepShipping, "Receiving" => keepReceiving, "Misc" => keepMisc, _ => false };
            if (!keep) continue;
            var masterSubFolder = Path.Combine(masterFolder, folder);
            if (!Directory.Exists(masterSubFolder)) continue;
            foreach (var masterFile in Directory.GetFiles(masterSubFolder, "*", SearchOption.AllDirectories))
            {
                if (string.Equals(Path.GetFileName(masterFile), "thumbs.db", StringComparison.OrdinalIgnoreCase)) continue;
                var relativePath = Path.GetRelativePath(masterFolder, masterFile);
                if (folder == "Shipping" && (relativePath.StartsWith("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) || relativePath.StartsWith("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion" + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) || relativePath.Equals("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase) || relativePath.Equals("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase) || relativePath.StartsWith("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase) || relativePath.StartsWith("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase))) continue;
                var userFile = Path.Combine(userFolder, relativePath);
                string? reason = null;
                if (!File.Exists(userFile)) { reason = "new file"; }
                else if (new FileInfo(masterFile).Length != new FileInfo(userFile).Length)
                {
                    var masterInfo = new FileInfo(masterFile);
                    var userInfo = new FileInfo(userFile);
                    reason = $"size: {userInfo.Length} → {masterInfo.Length}, date: {userInfo.LastWriteTime} → {masterInfo.LastWriteTime}";
                }
                if (reason != null) filesToSync.Add($"{relativePath} ({reason})");
            }
        }
        return filesToSync;
    }
    private void ResetAllLabelFileVersionsTo1_0()
    {
        void RenameShippingLblFilesInDirectory(string rootDir, List<string> openSkipped, ref int versionSkipped, ref int successCount)
        {
            var shippingDir = Path.Combine(rootDir, "Shipping");
            if (!Directory.Exists(shippingDir)) return;
            foreach (var file in Directory.GetFiles(shippingDir, "*.lbl", SearchOption.AllDirectories))
            {
                var dir = Path.GetDirectoryName(file)!;
                var name = Path.GetFileNameWithoutExtension(file);
                var ext = Path.GetExtension(file);
                var verMatch = System.Text.RegularExpressions.Regex.Match(name, @"\s*Ver\.?\s*(\d+)(\.\d+)?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (verMatch.Success)
                {
                    if (double.TryParse($"{verMatch.Groups[1].Value}{verMatch.Groups[2].Value}", out double verNum) && verNum >= 1.0)
                    {
                        versionSkipped++;
                        continue;
                    }
                }
                var newName = System.Text.RegularExpressions.Regex.Replace(name, @"\s*Ver\.?\s*\d+(\.\d+)?$", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                newName = $"{newName} Ver. 1.0{ext}";
                var newPath = Path.Combine(dir, newName);
                if (File.Exists(newPath)) continue;
                try
                {
                    using (var fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
                    File.Move(file, newPath);
                    successCount++;
                }
                catch (IOException) { openSkipped.Add(file); }
                catch (UnauthorizedAccessException) { openSkipped.Add(file); }
            }
            foreach (var bakFile in Directory.GetFiles(shippingDir, "*.bak", SearchOption.AllDirectories))
            {
                try { File.Delete(bakFile); }
                catch { }
            }
        }
        void RemoveVerFromReceivingAndMisc(string rootDir, List<string> openSkipped, ref int verRemovedCount)
        {
            foreach (var folder in new[] { "Receiving", "Misc" })
            {
                var targetDir = Path.Combine(rootDir, folder);
                if (!Directory.Exists(targetDir)) continue;
                foreach (var file in Directory.GetFiles(targetDir, "*.lbl", SearchOption.AllDirectories))
                {
                    var dir = Path.GetDirectoryName(file)!;
                    var name = Path.GetFileNameWithoutExtension(file);
                    var ext = Path.GetExtension(file);
                    var verMatch = System.Text.RegularExpressions.Regex.Match(name, @"\s*Ver\.?\s*(\d+)(\.\d+)?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (!verMatch.Success) continue;
                    var newName = System.Text.RegularExpressions.Regex.Replace(name, @"\s*Ver\.?\s*\d+(\.\d+)?$", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    var newPath = Path.Combine(dir, newName + ext);
                    if (File.Exists(newPath)) continue;
                    try
                    {
                        using (var fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
                        File.Move(file, newPath);
                        verRemovedCount++;
                    }
                    catch (IOException) { openSkipped.Add(file); }
                    catch (UnauthorizedAccessException) { openSkipped.Add(file); }
                }
                foreach (var bakFile in Directory.GetFiles(targetDir, "*.bak", SearchOption.AllDirectories))
                {
                    try { File.Delete(bakFile); }
                    catch { }
                }
            }
        }
        var openSkipped = new List<string>();
        int versionSkipped = 0;
        int successCount = 0;
        int verRemovedCount = 0;
        RenameShippingLblFilesInDirectory(masterFolder, openSkipped, ref versionSkipped, ref successCount);
        RemoveVerFromReceivingAndMisc(masterFolder, openSkipped, ref verRemovedCount);
        if (Directory.Exists(employeeFolderRoot))
        {
            foreach (var userDir in Directory.GetDirectories(employeeFolderRoot))
            {
                RenameShippingLblFilesInDirectory(userDir, openSkipped, ref versionSkipped, ref successCount);
                RemoveVerFromReceivingAndMisc(userDir, openSkipped, ref verRemovedCount);
            }
        }
        var report = new System.Text.StringBuilder();
        report.AppendLine($"Total Shipping files successfully renamed: {successCount}");
        report.AppendLine($"Total Shipping files skipped (already Ver 1.0 or newer): {versionSkipped}");
        report.AppendLine($"Total Receiving/Misc files with 'Ver' removed: {verRemovedCount}");
        report.AppendLine();
        report.AppendLine("Files skipped because they are open or access denied:");
        if (openSkipped.Count == 0) report.AppendLine("  (none)");
        else report.AppendLine(string.Join("\r\n", openSkipped));
        textBoxReport.Text = report.ToString();
    }

    private void CopyLabelViewFoldersAndUpdateDataLinks(string userFolder)
    {
        var masterLabelView = Path.Combine(masterFolder, "LabelView Folders");
        var userLabelView = Path.Combine(userFolder, "LabelView Folders");
        var filesToCopy = new List<string>();
        var errors = new List<string>();

        // Preview files to be copied/overwritten
        if (Directory.Exists(masterLabelView))
        {
            foreach (var masterFile in Directory.GetFiles(masterLabelView, "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(masterLabelView, masterFile);
                var userFile = Path.Combine(userLabelView, relativePath);

                string? reason = null;
                if (!File.Exists(userFile))
                {
                    reason = "new file";
                }
                else if (new FileInfo(masterFile).Length != new FileInfo(userFile).Length)
                {
                    var masterInfo = new FileInfo(masterFile);
                    var userInfo = new FileInfo(userFile);
                    reason = $"size: {userInfo.Length} → {masterInfo.Length}, date: {userInfo.LastWriteTime} → {masterInfo.LastWriteTime}";
                }
                else
                {
                    continue;
                }
                filesToCopy.Add($"{relativePath} ({reason})");
            }
        }

        // Prompt user if any files will be copied/overwritten
        if (filesToCopy.Count == 0)
        {
            textBoxReport.Text = "No core files needed updating.";
            MessageBox.Show("No core files need to be updated.", "Core Files Update Preview", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var previewMsg = "The following files will be overwritten or created:\r\n\r\n" +
                         string.Join("\r\n", filesToCopy) +
                         "\r\n\r\nDo you want to proceed with updating core files?";
        var result = MessageBox.Show(previewMsg, "Core Files Update Preview", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes)
        {
            textBoxReport.Text = "Core files update canceled by user.";
            return;
        }

        // Copy all files from Master LabelView Folders to User's LabelView Folders (overwrite all)
        if (Directory.Exists(masterLabelView))
        {
            try
            {
                CopyDirectory(masterLabelView, userLabelView);
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to copy LabelView Folders: {ex.Message}");
            }
        }

        // Edit all .udl files in DataLink folder
        var userName = Path.GetFileName(userFolder);
        var dataLinkDir = Path.Combine(userLabelView, "DataLink");
        if (Directory.Exists(dataLinkDir))
        {
            foreach (var udlFile in Directory.GetFiles(dataLinkDir, "*.udl", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    var lines = File.ReadAllLines(udlFile);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        // Replace any user folder in UNC path with the current userName
                        lines[i] = System.Text.RegularExpressions.Regex.Replace(
                            lines[i],
                            @"(\\\\MTMANU-FS01\\Expo Drive\\Shipping\\Labels - Labelview\\Employee Folder\\)[^\\]+",
                            $"\\\\MTMANU-FS01\\Expo Drive\\Shipping\\Labels - Labelview\\Employee Folder\\{userName}",
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase
                        );
                    }
                    File.WriteAllLines(udlFile, lines);
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to update {Path.GetFileName(udlFile)}: {ex.Message}");
                }
            }
        }

        // Update User.ini default directories
        var userIniPath = Path.Combine(userLabelView, "Settings", "User.ini");
        if (File.Exists(userIniPath))
        {
            try
            {
                var lines = File.ReadAllLines(userIniPath);
                for (int i = 0; i < lines.Length; i++)
                {
                    // Replace any Employee Folder user segment in UNC paths with the current userName
                    lines[i] = System.Text.RegularExpressions.Regex.Replace(
                        lines[i],
                        @"(\\\\MTMANU-FS01\\Expo Drive\\Shipping\\Labels - Labelview\\Employee Folder\\)[^\\]+",
                        $"\\\\MTMANU-FS01\\Expo Drive\\Shipping\\Labels - Labelview\\Employee Folder\\{userName}",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    );
                }
                File.WriteAllLines(userIniPath, lines);
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to update User.ini: {ex.Message}");
            }
        }

        // Fill textBoxReport
        if (errors.Count > 0)
        {
            textBoxReport.Text = "Some errors occurred:\r\n" + string.Join("\r\n", errors);
            MessageBox.Show(string.Join(Environment.NewLine, errors), "Core Files Update Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        else
        {
            textBoxReport.Text = filesToCopy.Count == 0
                ? "No core files needed updating."
                : "Core files updated:\r\n" + string.Join("\r\n", filesToCopy);
            MessageBox.Show("Core files and DataLinks updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void buttonUpdateCoreFiles_Click(object sender, EventArgs e)
    {
        // Check if Shift key is held down
        bool shiftHeld = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;

        if (shiftHeld)
        {
            // Run for all users in comboBoxUsers
            var userNames = comboBoxUsers.Items.Cast<string>().Where(u => !string.IsNullOrWhiteSpace(u)).ToList();
            if (userNames.Count == 0)
            {
                MessageBox.Show("No users found.", "No Users", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int successCount = 0;
            var errors = new List<string>();
            foreach (var name in userNames)
            {
                var userFolder = Path.Combine(employeeFolderRoot, name);

                // Prompt for each user
                var promptMsg = $"Update core files and DataLinks for user '{name}'?\r\n\r\nUser folder:\r\n{userFolder}\r\n\r\nProceed?";
                var userResult = MessageBox.Show(promptMsg, $"Update User: {name}", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (userResult != DialogResult.Yes)
                {
                    errors.Add($"{name}: Skipped by user.");
                    continue;
                }

                try
                {
                    CopyLabelViewFoldersAndUpdateDataLinks(userFolder);
                    successCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"{name}: {ex.Message}");
                }
            }
            string msg = $"Core files and DataLinks updated for {successCount} user(s).";
            if (errors.Count > 0)
                msg += "\r\nSome errors occurred:\r\n" + string.Join("\r\n", errors);
            MessageBox.Show(msg, "Batch Update Complete", MessageBoxButtons.OK, errors.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            return;
        }

        // Default: single user
        if (comboBoxUsers.SelectedItem is not string userName || string.IsNullOrWhiteSpace(userName))
        {
            MessageBox.Show("Please select a user first.", "No User Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var singleUserFolder = Path.Combine(employeeFolderRoot, userName);

        try
        {
            CopyLabelViewFoldersAndUpdateDataLinks(singleUserFolder);
            MessageBox.Show("Core files and DataLinks updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating core files: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}