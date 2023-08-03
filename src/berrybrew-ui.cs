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

    private dynamic uiConfig;
    
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

        uiConfig = bb.JsonParse("ui");
        
        components = new System.ComponentModel.Container();
        contextMenu = new System.Windows.Forms.ContextMenu();
        rightClickExit = new System.Windows.Forms.MenuItem();

        contextMenu.MenuItems.AddRange(
            new System.Windows.Forms.MenuItem[] { rightClickExit }
        );

        rightClickExit.Index = 0;
        rightClickExit.Text = "Exit";
        rightClickExit.Click += new System.EventHandler(rightClickExit_Click);

        ClientSize = new System.Drawing.Size(265, 325);
        Text = "berrybrew UI";

        trayIcon = new System.Windows.Forms.NotifyIcon(components);

        string iconPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        string iconFile = System.IO.Directory.GetParent(iconPath) + @"\inc\berrybrew.ico";

        trayIcon.Icon = new Icon(iconFile);
        trayIcon.ContextMenu = contextMenu;
        trayIcon.Text = "berrybrew UI";
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

    private void InitializeCurrentPerlLabel() {
        currentPerlLabel = new System.Windows.Forms.Label();
        SuspendLayout();

        currentPerlLabel.AutoSize = true;
        currentPerlLabel.Location = new System.Drawing.Point(10, 10);
        currentPerlLabel.Name = "currentPerlLabel";
        currentPerlLabel.Size = new System.Drawing.Size(35, 35);
        currentPerlLabel.TabIndex = 0;
        currentPerlLabel.Font = new Font(Font, FontStyle.Bold);

        Controls.Add(currentPerlLabel);
        Name = "BBUI";
        ResumeLayout(false);
        PerformLayout();
    }

    private void InitializePerlOpenButton() {
        perlOpenButton = new System.Windows.Forms.Button();

        perlOpenButton.Location = new System.Drawing.Point(169, 10);
        perlOpenButton.Name = "perlOpenButton";
        perlOpenButton.Size = new System.Drawing.Size(45, 20);
        perlOpenButton.TabIndex = 1;
        perlOpenButton.Text = "Open";
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

    private void InitializePerlOffButton() {
        perlOffButton = new System.Windows.Forms.Button();

        perlOffButton.Location = new System.Drawing.Point(215, 10);
        perlOffButton.Name = "perlOffButton";
        perlOffButton.Size = new System.Drawing.Size(35, 20);
        perlOffButton.TabIndex = 1;
        perlOffButton.Text = "Off";
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

    private void CurrentPerlLabel_Redraw() {
         currentPerlLabel.Text = "Current Perl: ";

         string perlInUse = bb.PerlOp.PerlInUse().Name;

         if (perlInUse == null) {
             perlInUse = "Not configured";
         }

         currentPerlLabel.Text = currentPerlLabel.Text += perlInUse;
    }

    private void InitializeFileAssocCheckBox() {
        fileAssocCheckBox = new System.Windows.Forms.CheckBox();
        fileAssocCheckBox.Width = 200;
        fileAssocCheckBox.AutoSize = true;
        fileAssocCheckBox.Text = "Manage file association";
        fileAssocCheckBox.Location = new System.Drawing.Point(10, 255);
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

    private void InitializeWarnOrphansCheckBox() {
        warnOrphansCheckBox = new System.Windows.Forms.CheckBox();
        warnOrphansCheckBox.Width = 200;
        warnOrphansCheckBox.AutoSize = true;
        warnOrphansCheckBox.Text = "Warn on orphans";
        warnOrphansCheckBox.Checked = WarnOrphans() ? true : false;
        warnOrphansCheckBox.Location = new System.Drawing.Point(10, 275);
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

    private void InitializeDebugCheckBox() {
        debugCheckBox = new System.Windows.Forms.CheckBox();
        debugCheckBox.Width = 200;
        debugCheckBox.AutoSize = true;
        debugCheckBox.Checked = bb.Options("debug", null, true) == "true" ? true : false;
        debugCheckBox.Text = "Debug";
        debugCheckBox.Location = new System.Drawing.Point(10, 215);
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

    private void InitializeUsePowershellCheckbox() {
        powershellCheckBox = new System.Windows.Forms.CheckBox();
        powershellCheckBox.Width = 200;
        powershellCheckBox.AutoSize = true;
        powershellCheckBox.Checked = bb.Options("shell", null, true) == "powershell" ? true : false;
        powershellCheckBox.Text = "Use Powershell";
        powershellCheckBox.Location = new System.Drawing.Point(10, 235);
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

    private void InitializeWindowsHomedirCheckBox() {
        windowsHomedirCheckBox = new System.Windows.Forms.CheckBox();
        windowsHomedirCheckBox.Width = 200;
        windowsHomedirCheckBox.AutoSize = true;
        windowsHomedirCheckBox.Checked = bb.Options("windows_homedir", null, true) == "true" ? true : false;
        windowsHomedirCheckBox.Text = "Windows homedir";
        windowsHomedirCheckBox.Location = new System.Drawing.Point(10, 295);
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

    private void InitializePerlInstallButton() {
        perlInstallButton = new System.Windows.Forms.Button();

        perlInstallButton.Location = new System.Drawing.Point(139, 65);
        perlInstallButton.Name = "perlInstallButton";
        perlInstallButton.Size = new System.Drawing.Size(75, 23);
        perlInstallButton.TabIndex = 1;
        perlInstallButton.Text = "Install";
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

    private void InitializePerlSwitchButton() {
        perlSwitchButton = new System.Windows.Forms.Button();

        perlSwitchButton.Location = new System.Drawing.Point(139, 35);
        perlSwitchButton.Name = "perlSwitchButton";
        perlSwitchButton.Size = new System.Drawing.Size(75, 23);
        perlSwitchButton.TabIndex = 1;
        perlSwitchButton.Text = "Switch";
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

    private void InitializePerlUseButton() {
        perlUseButton = new System.Windows.Forms.Button();

        perlUseButton.Location = new System.Drawing.Point(139, 95);
        perlUseButton.Name = "perlUseButton";
        perlUseButton.Size = new System.Drawing.Size(75, 23);
        perlUseButton.TabIndex = 1;
        perlUseButton.Text = "Use";
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

    private void InitializePerlRemoveButton() {
        perlRemoveButton = new System.Windows.Forms.Button();

        perlRemoveButton.Location = new System.Drawing.Point(139, 125);
        perlRemoveButton.Name = "perlRemoveButton";
        perlRemoveButton.Size = new System.Drawing.Size(75, 23);
        perlRemoveButton.TabIndex = 1;
        perlRemoveButton.Text = "Remove";
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

    private void InitializePerlCloneButton() {
        perlCloneButton = new System.Windows.Forms.Button();

        perlCloneButton.Location = new System.Drawing.Point(139, 155);
        perlCloneButton.Name = "perlCloneButton";
        perlCloneButton.Size = new System.Drawing.Size(75, 23);
        perlCloneButton.TabIndex = 1;
        perlCloneButton.Text = "Clone";
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

    private void InitializePerlFetchButton() {
        perlFetchButton = new System.Windows.Forms.Button();

        perlFetchButton.Location = new System.Drawing.Point(10, 185);
        perlFetchButton.Name = "perlFetchButton";
        perlFetchButton.Size = new System.Drawing.Size(75, 23);
        perlFetchButton.TabIndex = 1;
        perlFetchButton.Text = "Fetch";
        perlFetchButton.UseVisualStyleBackColor = true;

        perlFetchButton.Click += new System.EventHandler(fetchPerlButton_Click);
    }

    private void fetchPerlButton_Click(object Sender, EventArgs e) {
        bb.PerlOp.PerlUpdateAvailableList();
        DrawComponents();
        MessageBox.Show("Successfully updated the list of available Perls.", "berrybrew fetch");
    }

    private void InitializePerlInstallSelect() {
        perlInstallSelect = new System.Windows.Forms.ComboBox();
        perlInstallSelect.DropDownStyle = ComboBoxStyle.DropDownList;

        perlInstallSelect.FormattingEnabled = true;
        perlInstallSelect.Location = new System.Drawing.Point(10, 65);
        perlInstallSelect.Name = "perlSwitchSelect";
        perlInstallSelect.Size = new System.Drawing.Size(121, 30);
        perlInstallSelect.TabIndex = 0;

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

    private void InitializePerlSwitchSelect() {
        perlSwitchSelect = new System.Windows.Forms.ComboBox();
        perlSwitchSelect.DropDownStyle = ComboBoxStyle.DropDownList;

        perlSwitchSelect.FormattingEnabled = true;
        perlSwitchSelect.Location = new System.Drawing.Point(10, 35);
        perlSwitchSelect.Name = "perlSwitchSelect";
        perlSwitchSelect.Size = new System.Drawing.Size(121, 30);
        perlSwitchSelect.TabIndex = 0;

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

    private void InitializePerlUseSelect() {
        perlUseSelect = new System.Windows.Forms.ComboBox();
        perlUseSelect.DropDownStyle = ComboBoxStyle.DropDownList;

        perlUseSelect.FormattingEnabled = true;
        perlUseSelect.Location = new System.Drawing.Point(10, 95);
        perlUseSelect.Name = "perlUseSelect";
        perlUseSelect.Size = new System.Drawing.Size(121, 30);
        perlUseSelect.TabIndex = 0;

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

    private void InitializePerlRemoveSelect() {
        perlRemoveSelect = new System.Windows.Forms.ComboBox();
        perlRemoveSelect.DropDownStyle = ComboBoxStyle.DropDownList;

        perlRemoveSelect.FormattingEnabled = true;
        perlRemoveSelect.Location = new System.Drawing.Point(10, 125);
        perlRemoveSelect.Name = "perlRemoveSelect";
        perlRemoveSelect.Size = new System.Drawing.Size(121, 30);
        perlRemoveSelect.TabIndex = 0;

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

    private void InitializePerlCloneSelect() {
        perlCloneSelect = new System.Windows.Forms.ComboBox();
        perlCloneSelect.DropDownStyle = ComboBoxStyle.DropDownList;

        perlCloneSelect.FormattingEnabled = true;
        perlCloneSelect.Location = new System.Drawing.Point(10, 155);
        perlCloneSelect.Name = "perlCloneSelect";
        perlCloneSelect.Size = new System.Drawing.Size(121, 30);
        perlCloneSelect.TabIndex = 0;

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
