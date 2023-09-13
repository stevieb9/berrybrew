# Uninstall

If you used the self-extracting installer, simply run the uninstaller from
either `Add/Remove Programs` in the Control Panel, or the `uninst.exe`
uninstaller program located in the installation directory.

If you installed via any other method:

First, run the `berrybrew associate unset` if you're managing the `.pl` file
association with `berrybrew`.

Then, run the `berrybrew unconfig` command which removes the `PATH` environment
variables for any in-use Perl installation, and then removes `berrybrew` from
the `PATH` as well.

If you wish to delete the actual installation:

- Stop the UI if it's running (right-click the System Tray Icon, and click `Exit`)

- Remove the Perl installation root directory (by default `C:\berrybrew`)

- Remove the original download directory

- Remove the `Computer\HKEY_LOCAL_MACHINE\SOFTWARE\berrybrew` registry key

- If you've installed the UI, remove the `Computer\HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\Current Version\Run\BerrybrewUI` registry value
