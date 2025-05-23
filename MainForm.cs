using System.Text.Json;
using System.Diagnostics;


namespace LabelViewUserSync;

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
                    try
                    {
                        proc.Kill();
                        proc.WaitForExit(5000); // Wait up to 5 seconds for each process to exit
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not close process ID {proc.Id}: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
            }
            else
            {
                // Optionally, close your app if user does not want to proceed
                Close();
                return;
            }
        }

        buttonUpdateUserSettings.Enabled = comboBoxUsers.SelectedItem is string &&
                                           !string.IsNullOrWhiteSpace(comboBoxUsers.SelectedItem.ToString());


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

    private void ResetAllLabelFileVersionsTo1_0()
    {
        // Helper to rename .lbl files in a directory tree for Shipping
        void RenameShippingLblFilesInDirectory(
            string rootDir,
            List<string> openSkipped,
            ref int versionSkipped,
            ref int successCount)
        {
            var shippingDir = Path.Combine(rootDir, "Shipping");
            if (!Directory.Exists(shippingDir))
                return;

            foreach (var file in Directory.GetFiles(shippingDir, "*.lbl", SearchOption.AllDirectories))
            {
                var dir = Path.GetDirectoryName(file)!;
                var name = Path.GetFileNameWithoutExtension(file);
                var ext = Path.GetExtension(file);

                // Check for "Ver. X.X" or "Ver X.X" at the end (with or without dot)
                var verMatch = System.Text.RegularExpressions.Regex.Match(
                    name, @"\s*Ver\.?\s*(\d+)(\.\d+)?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (verMatch.Success)
                {
                    // If version is 1.0 or higher, skip and count
                    if (double.TryParse($"{verMatch.Groups[1].Value}{verMatch.Groups[2].Value}", out double verNum) && verNum >= 1.0)
                    {
                        versionSkipped++;
                        continue;
                    }
                }

                // Remove any existing "Ver. X.X" at the end (optional, for true reset)
                var newName = System.Text.RegularExpressions.Regex.Replace(
                    name, @"\s*Ver\.?\s*\d+(\.\d+)?$", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                newName = $"{newName} Ver. 1.0{ext}";
                var newPath = Path.Combine(dir, newName);

                // Avoid overwriting existing files
                if (File.Exists(newPath)) continue;

                // Try to move, skip if file is in use or access denied
                try
                {
                    using (var fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        // If we can open for read/write with no sharing, it's not in use
                    }
                    File.Move(file, newPath);
                    successCount++;
                }
                catch (IOException)
                {
                    openSkipped.Add(file);
                }
                catch (UnauthorizedAccessException)
                {
                    openSkipped.Add(file);
                }
            }
            // Delete .bak files in Shipping
            foreach (var bakFile in Directory.GetFiles(shippingDir, "*.bak", SearchOption.AllDirectories))
            {
                try
                {
                    File.Delete(bakFile);
                }
                catch
                {
                    // Ignore errors for .bak deletes
                }
            }
        }

        // Helper to remove "Ver" part from Receiving and Misc files and delete .bak files
        void RemoveVerFromReceivingAndMisc(
            string rootDir,
            List<string> openSkipped,
            ref int verRemovedCount)
        {
            foreach (var folder in new[] { "Receiving", "Misc" })
            {
                var targetDir = Path.Combine(rootDir, folder);
                if (!Directory.Exists(targetDir))
                    continue;

                foreach (var file in Directory.GetFiles(targetDir, "*.lbl", SearchOption.AllDirectories))
                {
                    var dir = Path.GetDirectoryName(file)!;
                    var name = Path.GetFileNameWithoutExtension(file);
                    var ext = Path.GetExtension(file);

                    // Check for "Ver. X.X" or "Ver X.X" at the end (with or without dot)
                    var verMatch = System.Text.RegularExpressions.Regex.Match(
                        name, @"\s*Ver\.?\s*(\d+)(\.\d+)?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                    if (!verMatch.Success)
                        continue; // No Ver part, nothing to remove

                    // Remove the "Ver" part
                    var newName = System.Text.RegularExpressions.Regex.Replace(
                        name, @"\s*Ver\.?\s*\d+(\.\d+)?$", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                    var newPath = Path.Combine(dir, newName + ext);

                    // Avoid overwriting existing files
                    if (File.Exists(newPath)) continue;

                    try
                    {
                        using (var fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            // If we can open for read/write with no sharing, it's not in use
                        }
                        File.Move(file, newPath);
                        verRemovedCount++;
                    }
                    catch (IOException)
                    {
                        openSkipped.Add(file);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        openSkipped.Add(file);
                    }
                }
                // Delete .bak files in Receiving/Misc
                foreach (var bakFile in Directory.GetFiles(targetDir, "*.bak", SearchOption.AllDirectories))
                {
                    try
                    {
                        File.Delete(bakFile);
                    }
                    catch
                    {
                        // Ignore errors for .bak deletes
                    }
                }
            }
        }

        var openSkipped = new List<string>();
        int versionSkipped = 0;
        int successCount = 0;
        int verRemovedCount = 0;

        // Master folder
        RenameShippingLblFilesInDirectory(masterFolder, openSkipped, ref versionSkipped, ref successCount);
        RemoveVerFromReceivingAndMisc(masterFolder, openSkipped, ref verRemovedCount);

        // All employee folders
        if (Directory.Exists(employeeFolderRoot))
        {
            foreach (var userDir in Directory.GetDirectories(employeeFolderRoot))
            {
                RenameShippingLblFilesInDirectory(userDir, openSkipped, ref versionSkipped, ref successCount);
                RemoveVerFromReceivingAndMisc(userDir, openSkipped, ref verRemovedCount);
            }
        }

        // Build report
        var report = new System.Text.StringBuilder();
        report.AppendLine($"Total Shipping files successfully renamed: {successCount}");
        report.AppendLine($"Total Shipping files skipped (already Ver 1.0 or newer): {versionSkipped}");
        report.AppendLine($"Total Receiving/Misc files with 'Ver' removed: {verRemovedCount}");
        report.AppendLine();
        report.AppendLine("Files skipped because they are open or access denied:");
        if (openSkipped.Count == 0)
            report.AppendLine("  (none)");
        else
            report.AppendLine(string.Join("\r\n", openSkipped));

        textBoxReport.Text = report.ToString();
    }

    private List<UserSettings> LoadAllUserSettings()
    {
        if (!File.Exists(UsersJsonPath))
            return new List<UserSettings>();
        return JsonSerializer.Deserialize<List<UserSettings>>(File.ReadAllText(UsersJsonPath)) ??
               new List<UserSettings>();
    }

    private void EnsureUserShortcut(string userFolder)
    {
        var shortcutName = "LabelView 2022 User Sync.lnk";
        var sourceShortcut = Path.Combine(employeeFolderRoot, shortcutName);
        var destShortcut = Path.Combine(userFolder, shortcutName);

        try
        {
            if (File.Exists(sourceShortcut) && !File.Exists(destShortcut)) File.Copy(sourceShortcut, destShortcut);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to copy shortcut: {ex.Message}");
        }
    }

    private void SaveAllUserSettings(List<UserSettings> users)
    {
        Directory.CreateDirectory(settingsFolder);
        File.WriteAllText(UsersJsonPath,
            JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));
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
        buttonUpdateUserSettings.Enabled = comboBoxUsers.SelectedItem is string &&
                                           !string.IsNullOrWhiteSpace(comboBoxUsers.SelectedItem.ToString());
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
        var userName = Path.GetFileName(userFolder);
        var settings = LoadUserSettings(userName ?? "");
        var keepShipping = settings?.UsesShipping ?? true;
        var keepReceiving = settings?.UsesReceiving ?? true;
        var keepMisc = settings?.UsesMisc ?? true;

        // Remove folders that are not checked in user settings
        if (!keepShipping)
        {
            var shippingPath = Path.Combine(userFolder, "Shipping");
            if (Directory.Exists(shippingPath))
                Directory.Delete(shippingPath, true);
        }

        if (!keepReceiving)
        {
            var receivingPath = Path.Combine(userFolder, "Receiving");
            if (Directory.Exists(receivingPath))
                Directory.Delete(receivingPath, true);
        }

        if (!keepMisc)
        {
            var miscPath = Path.Combine(userFolder, "Misc");
            if (Directory.Exists(miscPath))
                Directory.Delete(miscPath, true);
        }

        // Only process files in the three main folders
        foreach (var folder in new[] { "Shipping", "Receiving", "Misc" })
        {
            var keep = folder switch
            {
                "Shipping" => keepShipping,
                "Receiving" => keepReceiving,
                "Misc" => keepMisc,
                _ => false
            };
            if (!keep) continue;

            var masterSubFolder = Path.Combine(masterFolder, folder);
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
                    (relativePath.StartsWith(
                         "Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion" +
                         Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                     relativePath.StartsWith(
                         "Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion" +
                         Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                     relativePath.Equals("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion",
                         StringComparison.OrdinalIgnoreCase) ||
                     relativePath.Equals("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion",
                         StringComparison.OrdinalIgnoreCase) ||
                     relativePath.StartsWith("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion",
                         StringComparison.OrdinalIgnoreCase) ||
                     relativePath.StartsWith("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion",
                         StringComparison.OrdinalIgnoreCase)))
                    continue;

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
            MessageBox.Show(string.Join(Environment.NewLine, errors), "Sync Errors", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
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
            var keep = folder switch
            {
                "Shipping" => keepShipping,
                "Receiving" => keepReceiving,
                "Misc" => keepMisc,
                _ => false
            };
            if (!keep) continue;

            var masterSubFolder = Path.Combine(masterFolder, folder);
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
                    (relativePath.StartsWith(
                         "Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion" +
                         Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                     relativePath.StartsWith(
                         "Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion" +
                         Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                     relativePath.Equals("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion",
                         StringComparison.OrdinalIgnoreCase) ||
                     relativePath.Equals("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion",
                         StringComparison.OrdinalIgnoreCase) ||
                     relativePath.StartsWith("Shipping" + Path.DirectorySeparatorChar + "Still Needs Conversion",
                         StringComparison.OrdinalIgnoreCase) ||
                     relativePath.StartsWith("Shipping" + Path.AltDirectorySeparatorChar + "Still Needs Conversion",
                         StringComparison.OrdinalIgnoreCase)))
                    continue;

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

                if (reason != null) filesToSync.Add($"{relativePath} ({reason})");
            }
        }

        return filesToSync;
    }

    // 2. Add a new user
    private void buttonAddUser_Click(object sender, EventArgs e)
    {
        var firstName = textBoxFirstName.Text.Trim();
        var lastName = textBoxLastName.Text.Trim();
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            MessageBox.Show("First and Last Name are required.", "Input Error", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var userName = $"{firstName} {lastName}";
        var userFolder = Path.Combine(employeeFolderRoot, userName);

        if (Directory.Exists(userFolder))
        {
            MessageBox.Show("A user with this name already exists.", "Duplicate User", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        Directory.CreateDirectory(userFolder);

        // Copy selected folders
        if (checkBoxShipping.Checked)
        {
            var source = Path.Combine(masterFolder, "Shipping");
            var dest = Path.Combine(userFolder, "Shipping");
            if (Directory.Exists(source))
                CopyDirectory(source, dest);
        }

        if (checkBoxReceiving.Checked)
        {
            var source = Path.Combine(masterFolder, "Receiving");
            var dest = Path.Combine(userFolder, "Receiving");
            if (Directory.Exists(source))
                CopyDirectory(source, dest);
        }

        if (checkBoxMisc.Checked)
        {
            var source = Path.Combine(masterFolder, "Misc");
            var dest = Path.Combine(userFolder, "Misc");
            if (Directory.Exists(source))
                CopyDirectory(source, dest);
        }

        // Copy LabelView Folders for new user
        var labelViewSource = Path.Combine(masterFolder, "LabelView Folders");
        var labelViewDest = Path.Combine(userFolder, "LabelView Folders");
        if (Directory.Exists(labelViewSource))
            CopyDirectory(labelViewSource, labelViewDest);

        // Create/overwrite User.ini with contents of DefaultUsers.ini, replacing USERNAME
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
                MessageBox.Show($"DefaultUsers.ini not found at:\n{defaultIniPath}", "INI File Missing",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Optionally, return here if you want to abort user creation
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to create User.ini: {ex.Message}", "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
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
            MessageBox.Show("Please select a user to update.", "No User Selected", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var userName = comboBoxUpdateUser.SelectedItem.ToString()!;
        var userFolder = Path.Combine(employeeFolderRoot, userName);

        // Remove unchecked folders
        if (!checkBoxUpdateShipping.Checked)
        {
            var shippingPath = Path.Combine(userFolder, "Shipping");
            if (Directory.Exists(shippingPath))
                Directory.Delete(shippingPath, true);
        }
        else
        {
            var source = Path.Combine(masterFolder, "Shipping");
            var dest = Path.Combine(userFolder, "Shipping");
            if (Directory.Exists(source))
                CopyDirectory(source, dest);
        }

        if (!checkBoxUpdateReceiving.Checked)
        {
            var receivingPath = Path.Combine(userFolder, "Receiving");
            if (Directory.Exists(receivingPath))
                Directory.Delete(receivingPath, true);
        }
        else
        {
            var source = Path.Combine(masterFolder, "Receiving");
            var dest = Path.Combine(userFolder, "Receiving");
            if (Directory.Exists(source))
                CopyDirectory(source, dest);
        }

        if (!checkBoxUpdateMisc.Checked)
        {
            var miscPath = Path.Combine(userFolder, "Misc");
            if (Directory.Exists(miscPath))
                Directory.Delete(miscPath, true);
        }
        else
        {
            var source = Path.Combine(masterFolder, "Misc");
            var dest = Path.Combine(userFolder, "Misc");
            if (Directory.Exists(source))
                CopyDirectory(source, dest);
        }

        SaveUserSettings(userName, checkBoxUpdateShipping.Checked, checkBoxUpdateReceiving.Checked,
            checkBoxUpdateMisc.Checked);

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
            MessageBox.Show("Please select a user first.", "No User Selected", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var csIniPath = @"C:\ProgramData\Teklynx\LABELVIEW\Cs.ini";
        var newUserPath = $@"X:\Shipping\Labels - Labelview\Employee Folder\{userName}\LabelView Folders\Settings\";

        // 1. Get Printer= value from user's User.ini
        var userIniPath = Path.Combine(employeeFolderRoot, userName, "LabelView Folders", "Settings", "User.ini");
        string? userPrinter = null;
        if (File.Exists(userIniPath))
            foreach (var line in File.ReadAllLines(userIniPath))
                if (line.TrimStart().StartsWith("Printer=", StringComparison.OrdinalIgnoreCase))
                {
                    userPrinter = line.Substring(line.IndexOf('=') + 1).Trim();
                    break;
                }

        try
        {
            List<string> lines = new();
            if (File.Exists(csIniPath)) lines = File.ReadAllLines(csIniPath).ToList();

            // Update or add User= line
            var userLineFound = false;
            for (var i = 0; i < lines.Count; i++)
                if (lines[i].TrimStart().StartsWith("User=", StringComparison.OrdinalIgnoreCase))
                {
                    lines[i] = $"User={newUserPath}";
                    userLineFound = true;
                    break;
                }

            if (!userLineFound) lines.Add($"User={newUserPath}");

            // Update or add Printer= line
            if (!string.IsNullOrWhiteSpace(userPrinter))
            {
                var printerLineFound = false;
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
            MessageBox.Show("Cs.ini updated successfully.", "Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update Cs.ini: {ex.Message}", "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    // 1. Sync selected user's folder with master
    private void buttonSync_Click(object sender, EventArgs e)
    {
        if (comboBoxUsers.SelectedItem is null)
        {
            MessageBox.Show("Please select a user to sync.", "No User Selected", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var userName = comboBoxUsers.SelectedItem.ToString()!;
        var userFolder = Path.Combine(employeeFolderRoot, userName);

        // Preview what will change
        var filesToSync = PreviewSyncUserFolder(userFolder, masterFolder);

        if (filesToSync.Count == 0)
        {
            textBoxReport.Text = "No files needed syncing.";
            MessageBox.Show("No files need to be updated.", "Sync Preview", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
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
            MessageBox.Show("Please select a user to delete.", "No User Selected", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var userName = comboBoxUpdateUser.SelectedItem.ToString()!;
        var userFolder = Path.Combine(employeeFolderRoot, userName);

        var result = MessageBox.Show($"Are you sure you want to delete user '{userName}' and all their files?",
            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

    private void buttonResetLabelVersion_Click(object sender, EventArgs e)
    {
        // Prompt for PIN
        var prompt = "Enter PIN to reset all label file versions to 1.0.\n(Hint: Creator's PC PIN)";
        var input = Microsoft.VisualBasic.Interaction.InputBox(prompt, "PIN Required", "");

        if (input == null || input != "0831")
        {
            MessageBox.Show("Incorrect PIN. Operation canceled.", "Access Denied", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            "This will rename all .lbl files in both the master and all employee folders to end with 'Ver. 1.0'.\n\nAre you sure you want to proceed?",
            "Confirm Version Reset",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm == DialogResult.Yes)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                ResetAllLabelFileVersionsTo1_0();
                MessageBox.Show("All label file versions have been reset to 1.0.", "Operation Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during version reset:\n{ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}