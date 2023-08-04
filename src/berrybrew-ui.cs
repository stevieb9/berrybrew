using BerryBrew;
using BerryBrew.PerlInstance;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

public class BBUI : System.Windows.Forms.Form {
    private Berrybrew bb = new Berrybrew();

    private dynamic Conf;
    
    private System.Windows.Forms.NotifyIcon trayIcon;
    private System.Windows.Forms.ContextMenu contextMenu;
    private System.Windows.Forms.MenuItem rightClickExit;

    private Label currentPerlLabel;

    private Button perlOpenButton;
    private Button perlOffButton;

    private ComboBox perlSwitchSelect;
    private Button perlSwitchButton;

    private ComboBox perlInstallSelect;
    private Button perlInstallButton;

    private ComboBox perlUseSelect;
    private Button perlUseButton;

    private ComboBox perlRemoveSelect;
    private Button perlRemoveButton;

    private ComboBox perlCloneSelect;
    private Button perlCloneButton;

    private Button perlFetchButton;

    private CheckBox fileAssocCheckBox;
    private CheckBox warnOrphansCheckBox;
    private CheckBox debugCheckBox;
    private CheckBox powershellCheckBox;
    private CheckBox windowsHomedirCheckBox;

    private System.ComponentModel.IContainer components;

    [STAThread]
   static void Main() {
        Application.Run(new BBUI());
    }

    public BBUI() {

        Conf = bb.JsonParse("ui");

        ClientSize = new System.Drawing.Size(
            (int) Conf["ui_object"]["client_size"][0], 
            (int) Conf["ui_object"]["client_size"][1]
        );
        Text = "berrybrew UI";
       
        components = new System.ComponentModel.Container();
        contextMenu = new System.Windows.Forms.ContextMenu();
        rightClickExit = new System.Windows.Forms.MenuItem();

        contextMenu.MenuItems.AddRange(
            new System.Windows.Forms.MenuItem[] { rightClickExit }
        );

        rightClickExit.Index = 0;
        rightClickExit.Text = "Exit";
        rightClickExit.Click += new System.EventHandler(rightClickExit_Click);

        trayIcon = new System.Windows.Forms.NotifyIcon(components);

        string iconPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        string iconFile = System.IO.Directory.GetParent(iconPath) + @"\inc\berrybrew.ico";

        trayIcon.Icon = new Icon(iconFile);
        trayIcon.ContextMenu = contextMenu;
        trayIcon.Text = Conf["ui_object"]["trayicon_text"];
        trayIcon.Visible = true;
        trayIcon.Click += new System.EventHandler(trayIcon_Click);

        InitializeComponents();

        Name = "Form";
        Load += new System.EventHandler(Form1_Load);
        ResumeLayout(false);

        FormClosing += new FormClosingEventHandler(Form1_FormClosing);
    }

    protected override void Dispose(bool disposing) {
        if (disposing)
            if (components != null)
                components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponents() {
        InitializeCurrentPerlLabel();

        InitializePerlOpenButton();
        InitializePerlOffButton();

        InitializePerlSwitchSelect();
        InitializePerlSwitchButton();

        InitializePerlInstallSelect();
        InitializePerlInstallButton();

        InitializePerlUseSelect();
        InitializePerlUseButton();

        InitializePerlRemoveSelect();
        InitializePerlRemoveButton();

        InitializePerlCloneSelect();
        InitializePerlCloneButton();

        InitializePerlFetchButton();

        InitializeFileAssocCheckBox();
        InitializeWarnOrphansCheckBox();
        InitializeUsePowershellCheckbox();
        InitializeDebugCheckBox();
        InitializeWindowsHomedirCheckBox();
    }

    // Label - Current Perl
    private void InitializeCurrentPerlLabel() {
        string name = "currentPerl";
        var data = Conf["label"][name];

        currentPerlLabel = new System.Windows.Forms.Label();
        SuspendLayout();

        currentPerlLabel.AutoSize = data["autosize"];
        currentPerlLabel.Location = new System.Drawing.Point(
            (int) data["location"][0], 
            (int) data["location"][1]
        );
        currentPerlLabel.Name = name;
        currentPerlLabel.Size = new System.Drawing.Size(
            (int) data["size"][0],
            (int) data["size"][1]
        );
        currentPerlLabel.TabIndex = data["tabindex"];
        currentPerlLabel.Font = new Font(Font, FontStyle.Bold);

        Controls.Add(currentPerlLabel);
        Name = data["name"];
        ResumeLayout(false);
        PerformLayout();              
    }
    private void CurrentPerlLabel_Redraw() {
         currentPerlLabel.Text = "Current Perl: ";

         string perlInUse = bb.PerlOp.PerlInUse().Name;

         if (perlInUse == null) {
             perlInUse = "Not configured";
         }

         currentPerlLabel.Text = currentPerlLabel.Text += perlInUse;
    }
   
    // Checkbox - File association
    private void InitializeFileAssocCheckBox() {
        string name = "fileAssoc";
        var data = Conf["checkbox"][name];

        fileAssocCheckBox = new System.Windows.Forms.CheckBox();

        fileAssocCheckBox.Text = data["text"];
        fileAssocCheckBox.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        fileAssocCheckBox.Width = data["width"];
        fileAssocCheckBox.AutoSize = data["autosize"];
        fileAssocCheckBox.Checked = FileAssocManaged() ? true : false;
        fileAssocCheckBox.CheckedChanged += new System.EventHandler(fileAssocCheckedChanged);
        Controls.Add(fileAssocCheckBox);
    }
    private void FileAssocCheckBox_Redraw() {
        fileAssocCheckBox.Checked = FileAssocManaged() ? true : false;
    }
    private bool FileAssocManaged() {
        string assoc = bb.Options("file_assoc", null, true);
        return assoc == "berrybrewPerl" ? true : false;
    }
    private void fileAssocCheckedChanged(object Sender, EventArgs e) {
        if (fileAssocCheckBox.Checked) {
            if (String.IsNullOrEmpty(bb.PerlOp.PerlInUse().Name)) {
                System.Windows.Forms.MessageBox.Show("No berrybrew Perl in use. Can't set file association.");
                fileAssocCheckBox.Checked = false;
            }
            else {
                Console.WriteLine("Setting file assoc");
                bb.FileAssoc("set");
            }
        }
        else {
            Console.WriteLine("Unsetting file assoc");
            bb.FileAssoc("unset");
        }
    }

    // Checkbox - Warn orphans
    private void InitializeWarnOrphansCheckBox() {
        string name = "warnOrphans";
        var data = Conf["checkbox"][name];

        warnOrphansCheckBox = new System.Windows.Forms.CheckBox();
        warnOrphansCheckBox.Text = data["text"];
        warnOrphansCheckBox.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        warnOrphansCheckBox.Width = data["width"];
        warnOrphansCheckBox.AutoSize = data["autosize"];
        warnOrphansCheckBox.Checked = WarnOrphans() ? true : false;
        warnOrphansCheckBox.CheckedChanged += new System.EventHandler(warnOrphansCheckedChanged);
        Controls.Add(warnOrphansCheckBox);
    }
    private bool WarnOrphans() {
        string assoc = bb.Options("warn_orphans", null, true);
        return assoc == "true" ? true : false;
    }
    private void warnOrphansCheckedChanged(object Sender, EventArgs e) {
        if (warnOrphansCheckBox.Checked) {
            Console.WriteLine("Setting warn_orphans");
            bb.Options("warn_orphans", "true", true);
        }
        else {
            Console.WriteLine("Unsetting warn_orphans");
            bb.Options("warn_orphans", "false", true);
        }
    }
    private void WarnOrphansCheckBox_Redraw() {
        warnOrphansCheckBox.Checked = WarnOrphans() ? true : false;
    }

    // Checkbox - Debug
    private void InitializeDebugCheckBox() {
        string name = "debug";
        var data = Conf["checkbox"][name];
        
        debugCheckBox = new System.Windows.Forms.CheckBox();
        debugCheckBox.Text = data["text"];
        debugCheckBox.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        debugCheckBox.Width = data["width"];
        debugCheckBox.AutoSize = data["autosize"];
        debugCheckBox.Checked = bb.Options("debug", null, true) == "true" ? true : false;
        debugCheckBox.CheckedChanged += new System.EventHandler(debugCheckedChanged);
        Controls.Add(debugCheckBox);
    }
    private void debugCheckedChanged(object Sender, EventArgs e) {
        if (debugCheckBox.Checked) {
            Console.WriteLine("Setting debug");
            bb.Options("debug", "true", true);
        }
        else {
            Console.WriteLine("Unsetting debug");
            bb.Options("debug", "false", true);
        }
    }
    private void DebugCheckBox_Redraw() {
        debugCheckBox.Checked = bb.Options("debug", null, true) == "true" ? true : false;
    }

    // Checkbox - Powershell
    private void InitializeUsePowershellCheckbox() {
        string name = "powershell";
        var data = Conf["checkbox"][name];
        
        powershellCheckBox = new System.Windows.Forms.CheckBox();
        powershellCheckBox.Text = data["text"];
        powershellCheckBox.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        powershellCheckBox.Width = data["width"];
        powershellCheckBox.AutoSize = data["autosize"];
        powershellCheckBox.Checked = bb.Options("shell", null, true) == "powershell" ? true : false;
        powershellCheckBox.CheckedChanged += new System.EventHandler(powershellCheckedChanged);
        Controls.Add(powershellCheckBox);
    }
    private void powershellCheckedChanged(object Sender, EventArgs e) {
        if (powershellCheckBox.Checked) {
            Console.WriteLine("Setting powershell");
            bb.Options("shell", "powershell", true);
        }
        else {
            Console.WriteLine("Unsetting powershell");
            bb.Options("shell", "cmd", true);
        }
    }
    private void PowershellCheckBox_Redraw() {
        powershellCheckBox.Checked = bb.Options("shell", null, true) == "powershell" ? true : false;
    }

    // Checkbox - Windows homedir
    private void InitializeWindowsHomedirCheckBox() {
        string name = "windowsHomedir";
        var data = Conf["checkbox"][name];
        
        windowsHomedirCheckBox = new System.Windows.Forms.CheckBox();
        windowsHomedirCheckBox.Text = data["text"];
        windowsHomedirCheckBox.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        windowsHomedirCheckBox.Width = data["width"];
        windowsHomedirCheckBox.AutoSize = data["autosize"];
        windowsHomedirCheckBox.Checked = bb.Options("windows_homedir", null, true) == "true" ? true : false;
        windowsHomedirCheckBox.CheckedChanged += new System.EventHandler(windowsHomedirCheckedChanged);
        Controls.Add(windowsHomedirCheckBox);
    }
    private void windowsHomedirCheckedChanged(object Sender, EventArgs e) {
        if (windowsHomedirCheckBox.Checked) {
            Console.WriteLine("Setting windows_homedir");
            bb.Options("windows_homedir", "true", true);
        }
        else {
            Console.WriteLine("Unsetting windows_homedir");
            bb.Options("windows_homedir", "false", true);
        }
    }
    private void WindowsHomedirCheckBox_Redraw() {
        windowsHomedirCheckBox.Checked = bb.Options("windows_homedir", null, true) == "true" ? true : false;
    }

    // Button - Open Perl
    private void InitializePerlOpenButton() {
        string name = "perlOpen";
        var data = Conf["button"][name];
        
        perlOpenButton = new System.Windows.Forms.Button();

        perlOpenButton.Name = data["name"];
        perlOpenButton.Text = data["text"];
        perlOpenButton.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        perlOpenButton.Size = new System.Drawing.Size(
            (int) data["size"][0],
            (int) data["size"][1]
        );
        perlOpenButton.TabIndex = data["tabindex"];
        perlOpenButton.UseVisualStyleBackColor = true;

        perlOpenButton.Click += new System.EventHandler(openPerlButton_Click);
    }
    private void openPerlButton_Click(object Sender, EventArgs e) {
        // MessageBox.Show(((Button)Sender).Name + " was pressed!");
        string perlInUse = bb.PerlOp.PerlInUse().Name;

        if (perlInUse == null) {
            System.Windows.Forms.MessageBox.Show("No Perl currently in use!");
            return;
        }

        bb.UseCompile(perlInUse, true);
        WindowState = FormWindowState.Minimized;
        Hide();
        DrawComponents();
    }

    // Button - Off
    private void InitializePerlOffButton() {
        string name = "perlOff";
        var data = Conf["button"][name];
        
        perlOffButton = new System.Windows.Forms.Button();

        perlOffButton.Name = data["name"];
        perlOffButton.Text = data["text"];
        perlOffButton.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        perlOffButton.Size = new System.Drawing.Size(
            (int) data["size"][0],
            (int) data["size"][1]
        );
        perlOffButton.TabIndex = data["tabindex"];
        perlOffButton.UseVisualStyleBackColor = true;

        perlOffButton.Click += new System.EventHandler(offPerlButton_Click);
    }
    private void offPerlButton_Click(object Sender, EventArgs e) {
        string perlInUse = bb.PerlOp.PerlInUse().Name;

        if (perlInUse == null) {
            System.Windows.Forms.MessageBox.Show("No Perl currently in use!");
            return;
        }

        bb.Off();
        WindowState = FormWindowState.Minimized;
        Hide();
        DrawComponents();
    }   

    // Button - Install
    private void InitializePerlInstallButton() {
        string name = "perlInstall";
        var data = Conf["button"][name];
        
        perlInstallButton = new System.Windows.Forms.Button();

        perlInstallButton.Name = data["name"];
        perlInstallButton.Text = data["text"];
        perlInstallButton.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        perlInstallButton.Size = new System.Drawing.Size(
            (int) data["size"][0],
            (int) data["size"][1]
        );
        perlInstallButton.TabIndex = data["tabindex"];
        perlInstallButton.UseVisualStyleBackColor = true;

        perlInstallButton.Click += new System.EventHandler(installPerlButton_Click);
    }
    private void installPerlButton_Click(object Sender, EventArgs e) {
        if (perlInstallSelect.Text == "") {
            System.Windows.Forms.MessageBox.Show("No Perl selected to install!");
            return;
        }

        string perlName = perlInstallSelect.Text;
        bb.Install(perlName);
        DrawComponents();
    }

    // Button - Switch
    private void InitializePerlSwitchButton() {
        string name = "perlSwitch";
        var data = Conf["button"][name];
        
        perlSwitchButton = new System.Windows.Forms.Button();

        perlSwitchButton.Name = data["name"];
        perlSwitchButton.Text = data["text"];
        perlSwitchButton.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        perlSwitchButton.Size = new System.Drawing.Size(
            (int) data["size"][0],
            (int) data["size"][1]
        );
        perlSwitchButton.TabIndex = data["tabindex"];
        perlSwitchButton.UseVisualStyleBackColor = true;

        perlSwitchButton.Click += new System.EventHandler(switchPerlButton_Click);
    }
    private void switchPerlButton_Click(object Sender, EventArgs e) {
        if (perlSwitchSelect.Text == "") {
            System.Windows.Forms.MessageBox.Show("No Perl selected to switch to!");
            return;
        }

        string newPerl = perlSwitchSelect.Text;
        bb.Switch(newPerl);
        WindowState = FormWindowState.Minimized;
        Hide();
        Application.Restart();
        Environment.Exit(0);
    }

    // Button - Use
    private void InitializePerlUseButton() {
        string name = "perlUse";
        var data = Conf["button"][name];
        
        perlUseButton = new System.Windows.Forms.Button();

        perlUseButton.Name = data["name"];
        perlUseButton.Text = data["text"];
        perlUseButton.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        perlUseButton.Size = new System.Drawing.Size(
            (int) data["size"][0],
            (int) data["size"][1]
        );
        perlUseButton.TabIndex = data["tabindex"];
        perlUseButton.UseVisualStyleBackColor = true;

        perlUseButton.Click += new System.EventHandler(usePerlButton_Click);
    }
    private void usePerlButton_Click(object Sender, EventArgs e) {
        if (perlUseSelect.Text == "") {
            System.Windows.Forms.MessageBox.Show("No Perl selected to use!");
            return;
        }

        string perlName = perlUseSelect.Text;
        bb.UseCompile(perlName, true);
        DrawComponents();
    }

    // Button - Remove
    private void InitializePerlRemoveButton() {
        string name = "perlRemove";
        var data = Conf["button"][name];
        
        perlRemoveButton = new System.Windows.Forms.Button();

        perlRemoveButton.Name = data["name"];
        perlRemoveButton.Text = data["text"];
        perlRemoveButton.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        perlRemoveButton.Size = new System.Drawing.Size(
            (int) data["size"][0],
            (int) data["size"][1]
        );
        perlRemoveButton.TabIndex = data["tabindex"];
        perlRemoveButton.UseVisualStyleBackColor = true;

        perlRemoveButton.Click += new System.EventHandler(removePerlButton_Click);
    }
    private void removePerlButton_Click(object Sender, EventArgs e) {
        if (perlRemoveSelect.Text == "") {
            System.Windows.Forms.MessageBox.Show("No Perl selected to remove!");
            return;
        }

        string removePerl = perlRemoveSelect.Text;
        bb.PerlOp.PerlRemove(removePerl);
        DrawComponents();
    }

    // Button - Clone
    private void InitializePerlCloneButton() {
        string name = "perlClone";
        var data = Conf["button"][name];
        
        perlCloneButton = new System.Windows.Forms.Button();

        perlCloneButton.Name = data["name"];
        perlCloneButton.Text = data["text"];
        perlCloneButton.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        perlCloneButton.Size = new System.Drawing.Size(
            (int) data["size"][0],
            (int) data["size"][1]
        );
        perlCloneButton.TabIndex = data["tabindex"];
        perlCloneButton.UseVisualStyleBackColor = true;

        perlCloneButton.Click += new System.EventHandler(clonePerlButton_Click);
    }
    private void clonePerlButton_Click(object Sender, EventArgs e) {
        if (perlCloneSelect.Text == "") {
            System.Windows.Forms.MessageBox.Show("No Perl selected to clone!");
            return;
        }

        string clonePerl = perlCloneSelect.Text;
        string clonePerlName = Microsoft.VisualBasic.Interaction.InputBox(
            "Name of cloned Perl",
            "berrybrew Clone",
            "",
            150,
            150
        );

        bb.Clone(clonePerl, clonePerlName);
        DrawComponents();
        MessageBox.Show(String.Format("Successfully cloned Perl {0} to {1}", clonePerl, clonePerlName));
    }

    // Button - Fetch
    private void InitializePerlFetchButton() {
        string name = "perlFetch";
        var data = Conf["button"][name];
        
        perlFetchButton = new System.Windows.Forms.Button();

        perlFetchButton.Name = data["name"];
        perlFetchButton.Text = data["text"];
        perlFetchButton.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        perlFetchButton.Size = new System.Drawing.Size(
            (int) data["size"][0],
            (int) data["size"][1]
        );
        perlFetchButton.TabIndex = data["tabindex"];
        perlFetchButton.UseVisualStyleBackColor = true;

        perlFetchButton.Click += new System.EventHandler(fetchPerlButton_Click);
    }
    private void fetchPerlButton_Click(object Sender, EventArgs e) {
        bb.PerlOp.PerlUpdateAvailableList();
        DrawComponents();
        MessageBox.Show("Successfully updated the list of available Perls.", "berrybrew fetch");
    }

    // Select - Install
    private void InitializePerlInstallSelect() {
        string name = "perlInstall";
        var data = Conf["combobox"][name];
        
        perlInstallSelect = new System.Windows.Forms.ComboBox();
        perlInstallSelect.DropDownStyle = ComboBoxStyle.DropDownList;

        perlInstallSelect.Name = data["name"];
        perlInstallSelect.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        perlInstallSelect.Size = new System.Drawing.Size(
            (int) data["size"][0],
            (int) data["size"][1]
        );
        perlInstallSelect.FormattingEnabled = data["formatting_enabled"];
        perlInstallSelect.TabIndex = data["tabindex"];

        foreach (string perlName in bb.AvailableList()) {
            perlInstallSelect.Items.Add(perlName );
        }
    }
    private void PerlInstallSelect_Redraw() {
        perlInstallSelect.Items.Clear();

        foreach (string perlName in bb.AvailableList()) {
            perlInstallSelect.Items.Add(perlName );
        }

         perlInstallSelect.SelectedIndex = -1;
    }

    // Select - Switch
    private void InitializePerlSwitchSelect() {
        string name = "perlSwitch";
        var data = Conf["combobox"][name];
        
        perlSwitchSelect = new System.Windows.Forms.ComboBox();
        perlSwitchSelect.DropDownStyle = ComboBoxStyle.DropDownList;

        perlSwitchSelect.Name = data["name"];
        perlSwitchSelect.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        perlSwitchSelect.Size = new System.Drawing.Size(
            (int) data["size"][0],
            (int) data["size"][1]
        );
        perlSwitchSelect.FormattingEnabled = data["formatting_enabled"];
        perlSwitchSelect.TabIndex = data["tabindex"];

        string perlInUse = bb.PerlOp.PerlInUse().Name;

        foreach (StrawberryPerl perl in bb.PerlOp.PerlsInstalled()) {
            if (perl.Name == perlInUse)
                continue;

            perlSwitchSelect.Items.Add(perl.Name );
        }
    }
    private void PerlSwitchSelect_Redraw() {
        perlSwitchSelect.Items.Clear();

        string perlInUse = bb.PerlOp.PerlInUse().Name;

        foreach (StrawberryPerl perl in bb.PerlOp.PerlsInstalled()) {
            if (perl.Name == perlInUse)
                continue;

            perlSwitchSelect.Items.Add(perl.Name );
        }

        perlSwitchSelect.SelectedIndex = -1;
    }

    // Select - Use
    private void InitializePerlUseSelect() {
        string name = "perlUse";
        var data = Conf["combobox"][name];
        
        perlUseSelect = new System.Windows.Forms.ComboBox();
        perlUseSelect.DropDownStyle = ComboBoxStyle.DropDownList;

        perlUseSelect.Name = data["name"];
        perlUseSelect.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        perlUseSelect.Size = new System.Drawing.Size(
            (int) data["size"][0],
            (int) data["size"][1]
        );
        perlUseSelect.FormattingEnabled = data["formatting_enabled"];
        perlUseSelect.TabIndex = data["tabindex"];

        foreach (StrawberryPerl perl in bb.PerlOp.PerlsInstalled()) {
            perlUseSelect.Items.Add(perl.Name );
        }
    }
    private void PerlUseSelect_Redraw() {
        perlUseSelect.Items.Clear();

        foreach (StrawberryPerl perl in bb.PerlOp.PerlsInstalled()) {
            perlUseSelect.Items.Add(perl.Name );
        }
         perlUseSelect.SelectedIndex = -1;
    }

    // Select - Remove
    private void InitializePerlRemoveSelect() {
        string name = "perlRemove";
        var data = Conf["combobox"][name];
        
        perlRemoveSelect = new System.Windows.Forms.ComboBox();
        perlRemoveSelect.DropDownStyle = ComboBoxStyle.DropDownList;

        perlRemoveSelect.Name = data["name"];
        perlRemoveSelect.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        perlRemoveSelect.Size = new System.Drawing.Size(
            (int) data["size"][0],
            (int) data["size"][1]
        );
        perlRemoveSelect.FormattingEnabled = data["formatting_enabled"];
        perlRemoveSelect.TabIndex = data["tabindex"];

        foreach (StrawberryPerl perl in bb.PerlOp.PerlsInstalled()) {
            perlRemoveSelect.Items.Add(perl.Name);
        }
    }
    private void PerlRemoveSelect_Redraw() {
        perlRemoveSelect.Items.Clear();

         foreach (StrawberryPerl perl in bb.PerlOp.PerlsInstalled()) {
             perlRemoveSelect.Items.Add(perl.Name);
         }

         perlRemoveSelect.SelectedIndex = -1;
    }

    // Select - Clone
    private void InitializePerlCloneSelect() {
        string name = "perlClone";
        var data = Conf["combobox"][name];
        
        perlCloneSelect = new System.Windows.Forms.ComboBox();
        perlCloneSelect.DropDownStyle = ComboBoxStyle.DropDownList;

        perlCloneSelect.Name = data["name"];
        perlCloneSelect.Location = new System.Drawing.Point(
            (int) data["location"][0],
            (int) data["location"][1]
        );
        perlCloneSelect.Size = new System.Drawing.Size(
            (int) data["size"][0],
            (int) data["size"][1]
        );
        perlCloneSelect.FormattingEnabled = data["formatting_enabled"];
        perlCloneSelect.TabIndex = data["tabindex"];

        foreach (StrawberryPerl perl in bb.PerlOp.PerlsInstalled()) {
            perlCloneSelect.Items.Add(perl.Name);
        }
    }
    private void PerlCloneSelect_Redraw() {
        perlCloneSelect.Items.Clear();

         foreach (StrawberryPerl perl in bb.PerlOp.PerlsInstalled()) {
             perlCloneSelect.Items.Add(perl.Name);
         }

         perlCloneSelect.SelectedIndex = -1;
    }

    // Tray Icon
    private void trayIcon_Click(object Sender, EventArgs e) {
        DrawComponents();

        if (WindowState == FormWindowState.Minimized) {
            Show();
            WindowState = FormWindowState.Normal;
        }
        else {
            WindowState = FormWindowState.Minimized;
            Hide();
        }
    }
    private void rightClickExit_Click(object Sender, EventArgs e) {
        Close();
    }

    private void Form1_Load(object sender, EventArgs e) {
        
        if (bb.PerlOp.PerlInUse().Name != null) {
            Controls.Add(perlOpenButton);
        }

        Controls.Add(perlSwitchButton);
        Controls.Add(perlSwitchSelect);

        Controls.Add(perlInstallButton);
        Controls.Add(perlInstallSelect);

        Controls.Add(perlUseButton);
        Controls.Add(perlUseSelect);

        Controls.Add(perlRemoveButton);
        Controls.Add(perlRemoveSelect);

        Controls.Add(perlCloneButton);
        Controls.Add(perlCloneSelect);

        Controls.Add(perlFetchButton);

        DrawComponents();

        Name = "BBUI";

        string runMode = bb.Options("run_mode", null, true);

        if (runMode == "prod" || runMode == null) {
            Text = "BB UI v" + bb.Version();
        }
        else if (runMode == "staging") {
            Text = "BB-DEV UI v" + bb.Version();
        }

        WindowState = FormWindowState.Minimized;
        Hide();
        ShowInTaskbar = false;
        ResumeLayout(false);
    }
    private void Form1_FormClosing(Object sender, FormClosingEventArgs e) {
        if (! new StackTrace().GetFrames().Any(x => x.GetMethod().Name == "Close")){
            Hide();
            WindowState = FormWindowState.Minimized;
            e.Cancel = true;
        }
    }

    private void DrawComponents() {
        if (bb.PerlOp.PerlInUse().Name != null) {
            Controls.Add(perlOpenButton);
            Controls.Add(perlOffButton);
        }
        else {
             Controls.Remove(perlOpenButton);
             Controls.Remove(perlOffButton);
        }

        CurrentPerlLabel_Redraw();
        PerlInstallSelect_Redraw();
        PerlSwitchSelect_Redraw();
        PerlUseSelect_Redraw();
        PerlRemoveSelect_Redraw();
        PerlCloneSelect_Redraw();

        FileAssocCheckBox_Redraw();
        WarnOrphansCheckBox_Redraw();
        DebugCheckBox_Redraw();
        PowershellCheckBox_Redraw();
        WindowsHomedirCheckBox_Redraw();
    }
}
