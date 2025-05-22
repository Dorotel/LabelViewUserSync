using System.Text.Json;
using System.Diagnostics;


namespace LabelViewUserSync
{
    public partial class MainForm : Form
    {
        private readonly string masterFolder = @"X:\Shipping\Labels - Labelview\Master Label Folder";
        private readonly string employeeFolderRoot = @"X:\Shipping\Labels - Labelview\Employee Folder";
        private string UsersJsonPath => Path.Combine(settingsFolder, "users.json");

        public MainForm()
        {
            InitializeComponent();
            var lvProcesses = Process.GetProcessesByName("LV");
            if (lvProcesses.Length > 0)
            {
                var result = MessageBox.Show(
                    "LabelView (LV.exe) is currently running. All instances will be closed. Please save your work before continuing.\n\nDo you want to close all LV.exe processes now?",
                    "Close LabelView",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    foreach (var proc in lvProcesses)
                    {
                        try
                        {
                            proc.Kill();
                            proc.WaitForExit(5000); // Wait up to 5 seconds for each process to exit
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Could not close process ID {proc.Id}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    // Optionally, close your app if user does not want to proceed
                    this.Close();
                    return;
                }
            }
            buttonUpdateUserSettings.Enabled = comboBoxUsers.SelectedItem is string && !string.IsNullOrWhiteSpace(comboBoxUsers.SelectedItem.ToString());



            // Ensure users.json exists
            if (!File.Exists(UsersJsonPath))
            {
                var userFolders = Directory.GetDirectories(employeeFolderRoot)
                    .Select(Path.GetFileName)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .OrderBy(n => n)
                    .ToList();

                var users = userFolders.Select(name => new UserSettings
                {
                    UserName = name,
                    UsesShipping = Directory.Exists(Path.Combine(employeeFolderRoot, name, "Shipping")),
                    UsesReceiving = Directory.Exists(Path.Combine(employeeFolderRoot, name, "Receiving")),
                    UsesMisc = Directory.Exists(Path.Combine(employeeFolderRoot, name, "Misc"))
                }).ToList();

                SaveAllUserSettings(users);
            }

            RefreshUserLists();
            comboBoxUsers.SelectedIndexChanged += comboBoxUsers_SelectedIndexChanged;
            comboBoxUpdateUser.SelectedIndexChanged += comboBoxUpdateUser_SelectedIndexChanged;
            UpdateStatusCheckboxes();
            UpdateUpdateUserCheckboxes();
        }

        private void SetUserDefaultPrinter(string userName, string printerName)
        {
            string iniPath = Path.Combine(employeeFolderRoot, userName, "LabelView Folders", "Settings", "User.ini");
            Directory.CreateDirectory(Path.GetDirectoryName(iniPath)!);

            List<string> lines = new List<string>();
            if (File.Exists(iniPath))
                lines = File.ReadAllLines(iniPath).ToList();

            bool found = false;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].TrimStart().StartsWith("Printer=", StringComparison.OrdinalIgnoreCase))
                {
                    lines[i] = $"Printer={printerName}";
                    found = true;
                    break;
                }
            }
            if (!found)
                lines.Add($"Printer={printerName}");

            File.WriteAllLines(iniPath, lines);
        }
        private List<UserSettings> LoadAllUserSettings()
        {
            if (!File.Exists(UsersJsonPath))
                return new List<UserSettings>();
            return JsonSerializer.Deserialize<List<UserSettings>>(File.ReadAllText(UsersJsonPath)) ?? new List<UserSettings>();
        }
        private void EnsureUserShortcut(string userFolder)
        {
            string shortcutName = "LabelView 2022 User Sync.lnk";
            string sourceShortcut = Path.Combine(employeeFolderRoot, shortcutName);
            string destShortcut = Path.Combine(userFolder, shortcutName);

            try
            {
                if (File.Exists(sourceShortcut) && !File.Exists(destShortcut))
                {
                    File.Copy(sourceShortcut, destShortcut);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to copy shortcut: {ex.Message}");
            }
        }
        private void SaveAllUserSettings(List<UserSettings> users)
        {
            Directory.CreateDirectory(settingsFolder);
            File.WriteAllText(UsersJsonPath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));
        }
        private string? GetUserDefaultPrinter(string userName)
        {
            string iniPath = Path.Combine(employeeFolderRoot, userName, "LabelView Folders", "Settings", "User.ini");
            if (!File.Exists(iniPath))
                return null;

            foreach (var line in File.ReadAllLines(iniPath))
            {
                if (line.TrimStart().StartsWith("Printer=", StringComparison.OrdinalIgnoreCase))
                {
                    var printerName = line.Substring(line.IndexOf('=') + 1).Trim();
                    if (!string.IsNullOrEmpty(printerName))
                        return printerName;
                }
            }
            return null;
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
                users.Add(new UserSettings
                {
                    UserName = userName,
                    UsesShipping = usesShipping,
                    UsesReceiving = usesReceiving,
                    UsesMisc = usesMisc
                });
            }
            SaveAllUserSettings(users);
        }

        private UserSettings? LoadUserSettings(string userName)
        {
            var users = LoadAllUserSettings();
            return users.FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        private void comboBoxUsers_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateStatusCheckboxes();

            // Enable the button only if a user is selected
            buttonUpdateUserSettings.Enabled = comboBoxUsers.SelectedItem is string && !string.IsNullOrWhiteSpace(comboBoxUsers.SelectedItem.ToString());
        }

        private void comboBoxUpdateUser_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateUpdateUserCheckboxes();
        }

        private void UpdateStatusCheckboxes()
        {
            if (comboBoxUsers.SelectedItem is string userName)
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
            if (comboBoxUpdateUser.SelectedItem is string userName)
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

        public class UserSettings
        {
            public string UserName { get; set; } = "";
            public bool UsesShipping { get; set; }
            public bool UsesReceiving { get; set; }
            public bool UsesMisc { get; set; } // Add this property
        }

        // Add these helper methods inside MainForm
        private readonly string settingsFolder = @"X:\Shipping\Labels - Labelview\SyncFiles";

        private List<string> SyncUserFolder(string userFolder, string masterFolder)
        {
            var syncedFiles = new List<string>();
            var errors = new List<string>();

            // Load user settings to determine which folders should exist
            string userName = Path.GetFileName(userFolder);
            var settings = LoadUserSettings(userName ?? "");
            bool keepShipping = settings?.UsesShipping ?? true;
            bool keepReceiving = settings?.UsesReceiving ?? true;
            bool keepMisc = settings?.UsesMisc ?? true;

            // Remove folders that are not checked in user settings
            if (!keepShipping)
            {
                string shippingPath = Path.Combine(userFolder, "Shipping");
                if (Directory.Exists(shippingPath))
                    Directory.Delete(shippingPath, true);
            }
            if (!keepReceiving)
            {
                string receivingPath = Path.Combine(userFolder, "Receiving");
                if (Directory.Exists(receivingPath))
                    Directory.Delete(receivingPath, true);
            }
            if (!keepMisc)
            {
                string miscPath = Path.Combine(userFolder, "Misc");
                if (Directory.Exists(miscPath))
                    Directory.Delete(miscPath, true);
            }

            // Only process files in the three main folders
            foreach (var folder in new[] { "Shipping", "Receiving", "Misc" })
            {
                bool keep = folder switch
                {
                    "Shipping" => keepShipping,
                    "Receiving" => keepReceiving,
                    "Misc" => keepMisc,
                    _ => false
                };
                if (!keep) continue;

                string masterSubFolder = Path.Combine(masterFolder, folder);
                if (!Directory.Exists(masterSubFolder))
                    continue;

                foreach (var masterFile in Directory.GetFiles(masterSubFolder, "*", SearchOption.AllDirectories))
                {
                    // Exclude thumbs.db
                    if (string.Equals(Path.GetFileName(masterFile), "thumbs.db", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var relativePath = Path.GetRelativePath(masterFolder, masterFile);

                    // Exclude "Still Needs Conversion" in Shipping
                    if (folder == "Shipping" &&
                        (relativePath.StartsWith("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                         relativePath.StartsWith("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion" + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                         relativePath.Equals("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase) ||
                         relativePath.Equals("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase) ||
                         relativePath.StartsWith("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase) ||
                         relativePath.StartsWith("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    var userFile = Path.Combine(userFolder, relativePath);

                    try
                    {
                        string reason;
                        if (!File.Exists(userFile))
                        {
                            reason = "new file";
                        }
                        else if (new FileInfo(masterFile).Length != new FileInfo(userFile).Length)
                        {
                            var masterInfo = new FileInfo(masterFile);
                            var userInfo = new FileInfo(userFile);
                            reason = $"size: {userInfo.Length} → {masterInfo.Length}, " +
                                     $"date: {userInfo.LastWriteTime} → {masterInfo.LastWriteTime}";
                        }
                        else
                        {
                            continue;
                        }

                        Directory.CreateDirectory(Path.GetDirectoryName(userFile)!);
                        File.Copy(masterFile, userFile, true);
                        syncedFiles.Add($"{relativePath} ({reason})");
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Failed to sync '{relativePath}': {ex.Message}");
                    }
                }
            }

            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join(Environment.NewLine, errors), "Sync Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            EnsureUserShortcut(userFolder);

            return syncedFiles;
        }
        private List<string> PreviewSyncUserFolder(string userFolder, string masterFolder)
        {
            var filesToSync = new List<string>();

            string userName = Path.GetFileName(userFolder);
            var settings = LoadUserSettings(userName ?? "");
            bool keepShipping = settings?.UsesShipping ?? true;
            bool keepReceiving = settings?.UsesReceiving ?? true;
            bool keepMisc = settings?.UsesMisc ?? true;

            foreach (var folder in new[] { "Shipping", "Receiving", "Misc" })
            {
                bool keep = folder switch
                {
                    "Shipping" => keepShipping,
                    "Receiving" => keepReceiving,
                    "Misc" => keepMisc,
                    _ => false
                };
                if (!keep) continue;

                string masterSubFolder = Path.Combine(masterFolder, folder);
                if (!Directory.Exists(masterSubFolder))
                    continue;

                foreach (var masterFile in Directory.GetFiles(masterSubFolder, "*", SearchOption.AllDirectories))
                {
                    // Exclude thumbs.db
                    if (string.Equals(Path.GetFileName(masterFile), "thumbs.db", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var relativePath = Path.GetRelativePath(masterFolder, masterFile);

                    // Exclude "Still Needs Conversion" in Shipping
                    if (folder == "Shipping" &&
                        (relativePath.StartsWith("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                         relativePath.StartsWith("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion" + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                         relativePath.Equals("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase) ||
                         relativePath.Equals("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase) ||
                         relativePath.StartsWith("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase) ||
                         relativePath.StartsWith("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion", StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    var userFile = Path.Combine(userFolder, relativePath);

                    string reason = null;
                    if (!File.Exists(userFile))
                    {
                        reason = "new file";
                    }
                    else if (new FileInfo(masterFile).Length != new FileInfo(userFile).Length)
                    {
                        var masterInfo = new FileInfo(masterFile);
                        var userInfo = new FileInfo(userFile);
                        reason = $"size: {userInfo.Length} → {masterInfo.Length}, " +
                                 $"date: {userInfo.LastWriteTime} → {masterInfo.LastWriteTime}";
                    }

                    if (reason != null)
                    {
                        filesToSync.Add($"{relativePath} ({reason})");
                    }
                }
            }

            return filesToSync;
        }

        // 2. Add a new user
        private void buttonAddUser_Click(object sender, EventArgs e)
        {
            string firstName = textBoxFirstName.Text.Trim();
            string lastName = textBoxLastName.Text.Trim();
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("First and Last Name are required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string userName = $"{firstName} {lastName}";
            string userFolder = Path.Combine(employeeFolderRoot, userName);

            if (Directory.Exists(userFolder))
            {
                MessageBox.Show("A user with this name already exists.", "Duplicate User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Directory.CreateDirectory(userFolder);

            // Copy selected folders
            if (checkBoxShipping.Checked)
            {
                string source = Path.Combine(masterFolder, "Shipping");
                string dest = Path.Combine(userFolder, "Shipping");
                if (Directory.Exists(source))
                    CopyDirectory(source, dest);
            }
            if (checkBoxReceiving.Checked)
            {
                string source = Path.Combine(masterFolder, "Receiving");
                string dest = Path.Combine(userFolder, "Receiving");
                if (Directory.Exists(source))
                    CopyDirectory(source, dest);
            }
            if (checkBoxMisc.Checked)
            {
                string source = Path.Combine(masterFolder, "Misc");
                string dest = Path.Combine(userFolder, "Misc");
                if (Directory.Exists(source))
                    CopyDirectory(source, dest);
            }

            // Copy LabelView Folders for new user
            string labelViewSource = Path.Combine(masterFolder, "LabelView Folders");
            string labelViewDest = Path.Combine(userFolder, "LabelView Folders");
            if (Directory.Exists(labelViewSource))
                CopyDirectory(labelViewSource, labelViewDest);

            // Create/overwrite User.ini with contents of DefaultUsers.ini, replacing USERNAME
            string userIniDir = Path.Combine(userFolder, "LabelView Folders", "Settings");
            Directory.CreateDirectory(userIniDir);

            string userIniPath = Path.Combine(userIniDir, "User.ini");
            string defaultIniPath = Path.Combine(settingsFolder, "DefaultUsers.ini");
            string userFullName = $"{firstName} {lastName}";

            try
            {
                if (File.Exists(defaultIniPath))
                {
                    string iniContents = File.ReadAllText(defaultIniPath);
                    iniContents = iniContents.Replace("USERNAME", userFullName);
                    File.WriteAllText(userIniPath, iniContents);
                }
                else
                {
                    MessageBox.Show($"DefaultUsers.ini not found at:\n{defaultIniPath}", "INI File Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Optionally, return here if you want to abort user creation
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create User.ini: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveUserSettings(userName, checkBoxShipping.Checked, checkBoxReceiving.Checked, checkBoxMisc.Checked);

            // Sync after adding (does NOT sync LabelView Folders)
            SyncUserFolder(userFolder, masterFolder);
            MessageBox.Show("User created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            EnsureUserShortcut(userFolder);
            RefreshUserLists();

            // Reset add-user fields and checkboxes
            textBoxFirstName.Text = string.Empty;
            textBoxLastName.Text = string.Empty;
            checkBoxShipping.Checked = false;
            checkBoxReceiving.Checked = false;
            checkBoxMisc.Checked = false;
        }


        // 3. Update user (change folders)
        private void buttonUpdateUser_Click(object sender, EventArgs e)
        {
            if (comboBoxUpdateUser.SelectedItem is null)
            {
                MessageBox.Show("Please select a user to update.", "No User Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string userName = comboBoxUpdateUser.SelectedItem.ToString()!;
            string userFolder = Path.Combine(employeeFolderRoot, userName);

            // Remove unchecked folders
            if (!checkBoxUpdateShipping.Checked)
            {
                string shippingPath = Path.Combine(userFolder, "Shipping");
                if (Directory.Exists(shippingPath))
                    Directory.Delete(shippingPath, true);
            }
            else
            {
                string source = Path.Combine(masterFolder, "Shipping");
                string dest = Path.Combine(userFolder, "Shipping");
                if (Directory.Exists(source))
                    CopyDirectory(source, dest);
            }

            if (!checkBoxUpdateReceiving.Checked)
            {
                string receivingPath = Path.Combine(userFolder, "Receiving");
                if (Directory.Exists(receivingPath))
                    Directory.Delete(receivingPath, true);
            }
            else
            {
                string source = Path.Combine(masterFolder, "Receiving");
                string dest = Path.Combine(userFolder, "Receiving");
                if (Directory.Exists(source))
                    CopyDirectory(source, dest);
            }

            if (!checkBoxUpdateMisc.Checked)
            {
                string miscPath = Path.Combine(userFolder, "Misc");
                if (Directory.Exists(miscPath))
                    Directory.Delete(miscPath, true);
            }
            else
            {
                string source = Path.Combine(masterFolder, "Misc");
                string dest = Path.Combine(userFolder, "Misc");
                if (Directory.Exists(source))
                    CopyDirectory(source, dest);
            }

            SaveUserSettings(userName, checkBoxUpdateShipping.Checked, checkBoxUpdateReceiving.Checked, checkBoxUpdateMisc.Checked);

            // Sync after updating
            SyncUserFolder(userFolder, masterFolder);

            MessageBox.Show("User updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Helper: Refresh user lists in both combo boxes
        private void RefreshUserLists()
        {
            var userFolders = Directory.GetDirectories(employeeFolderRoot)
                .Select(Path.GetFileName)
                .Where(n => !string.IsNullOrEmpty(n))
                .OrderBy(n => n)
                .ToArray();
            comboBoxUsers.Items.Clear();
            comboBoxUsers.Items.AddRange(userFolders!);
            comboBoxUpdateUser.Items.Clear();
            comboBoxUpdateUser.Items.AddRange(userFolders!);
        }

        private void buttonUpdateUserSettings_Click(object sender, EventArgs e)
        {
            if (comboBoxUsers.SelectedItem is not string userName)
            {
                MessageBox.Show("Please select a user first.", "No User Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string csIniPath = @"C:\ProgramData\Teklynx\LABELVIEW\Cs.ini";
            string newUserPath = $@"X:\Shipping\Labels - Labelview\Employee Folder\{userName}\LabelView Folders\Settings\";

            // 1. Get Printer= value from user's User.ini
            string userIniPath = Path.Combine(employeeFolderRoot, userName, "LabelView Folders", "Settings", "User.ini");
            string? userPrinter = null;
            if (File.Exists(userIniPath))
            {
                foreach (var line in File.ReadAllLines(userIniPath))
                {
                    if (line.TrimStart().StartsWith("Printer=", StringComparison.OrdinalIgnoreCase))
                    {
                        userPrinter = line.Substring(line.IndexOf('=') + 1).Trim();
                        break;
                    }
                }
            }

            try
            {
                List<string> lines = new List<string>();
                if (File.Exists(csIniPath))
                {
                    lines = File.ReadAllLines(csIniPath).ToList();
                }

                // Update or add User= line
                bool userLineFound = false;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].TrimStart().StartsWith("User=", StringComparison.OrdinalIgnoreCase))
                    {
                        lines[i] = $"User={newUserPath}";
                        userLineFound = true;
                        break;
                    }
                }
                if (!userLineFound)
                {
                    lines.Add($"User={newUserPath}");
                }

                // Update or add Printer= line
                if (!string.IsNullOrWhiteSpace(userPrinter))
                {
                    bool printerLineFound = false;
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].TrimStart().StartsWith("Printer=", StringComparison.OrdinalIgnoreCase))
                        {
                            lines[i] = $"Printer={userPrinter}";
                            printerLineFound = true;
                            break;
                        }
                    }
                    if (!printerLineFound)
                    {
                        lines.Add($"Printer={userPrinter}");
                    }
                }

                File.WriteAllLines(csIniPath, lines);
                MessageBox.Show("Cs.ini updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update Cs.ini: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 1. Sync selected user's folder with master
        private void buttonSync_Click(object sender, EventArgs e)
        {
            if (comboBoxUsers.SelectedItem is null)
            {
                MessageBox.Show("Please select a user to sync.", "No User Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string userName = comboBoxUsers.SelectedItem.ToString()!;
            string userFolder = Path.Combine(employeeFolderRoot, userName);

            // Preview what will change
            var filesToSync = PreviewSyncUserFolder(userFolder, masterFolder);

            if (filesToSync.Count == 0)
            {
                textBoxReport.Text = "No files needed syncing.";
                MessageBox.Show("No files need to be updated.", "Sync Preview", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var previewMsg = "The following files will be overwritten or created:\r\n\r\n" +
                             string.Join("\r\n", filesToSync) +
                             "\r\n\r\nDo you want to proceed with syncing?";
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

        // 4. Delete user
        private void buttonDeleteUser_Click(object sender, EventArgs e)
        {
            if (comboBoxUpdateUser.SelectedItem is null)
            {
                MessageBox.Show("Please select a user to delete.", "No User Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string userName = comboBoxUpdateUser.SelectedItem.ToString()!;
            string userFolder = Path.Combine(employeeFolderRoot, userName);

            var result = MessageBox.Show($"Are you sure you want to delete user '{userName}' and all their files?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                if (Directory.Exists(userFolder))
                    Directory.Delete(userFolder, true);

                // Remove user from users.json
                DeleteUserSettings(userName);

                RefreshUserLists();

                // Uncheck only the update pane checkboxes
                checkBoxUpdateShipping.Checked = false;
                checkBoxUpdateReceiving.Checked = false;
                checkBoxUpdateMisc.Checked = false;

                MessageBox.Show("User deleted.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Helper: Copy directory recursively
        private void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);
            foreach (var file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)), true);
            foreach (var dir in Directory.GetDirectories(sourceDir))
                CopyDirectory(dir, Path.Combine(destDir, Path.GetFileName(dir)));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IniEditorForm iniEditor = new IniEditorForm(Path.Combine(settingsFolder, "DefaultUsers.ini"));
            iniEditor.ShowDialog();
        }
    }
}
