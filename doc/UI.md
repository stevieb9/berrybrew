# User Interface development

- Code is in `src/berrybrew-ui.cs`
- Staging UI built binary is `staging/berrybrew-ui.exe`
- Production UI built binary is `bin/berrybrew-ui.exe`
- Configuration for UI layout is `dev/data/ui.json`

- [UI Build](#ui-build)
- [UI Simulator](#ui-simulator)
- [UI Tools](#ui-tools)
- [UI Tools - Move element block](#move-element-block)
- [UI Tools - Resize main window](#resize-main-window)

### UI Build

To build and test the development/staging UI:

- Run the `dev\build_staging_ui.bat` script, which runs `dev\build_staging.bat`
  compiling the API and the `berrybrew` binary, followed by the UI binary itself.
- Run the `staging\berrybrew-ui.exe` to start the UI. Note that the staging UI
  build will execute out of a command line window, so that you can see the debugging
  output.

To build the production UI for testing, see the 
[Prod UI build](Berrybrew%20development.md#production-build) documentation.

### UI Simulator

To aid in manipulating the UI element layout collected from the
`dev\data\ui.json` configuration file, we've got a UI simulator in
`dev\ui_simulator.pl`.

This simulator is `Tkx` based, and runs on MacOS and Linux.

It is non-functional; it's used solely to replicate the exact layout. If you
modify any of the elements in the `src\ui.json` configuration file, a click of
any button in the UI simulator will reload the window live time, so you don't
have to close and reopen the UI sim on each change.

### UI Tools

The UI manipulation tool scripts reside in the `dev/ui_tools` directory.

#### Move element block

This script is `dev/ui_tools/move_element_block.pl`.

It allows you to move an entire element block up and down by a pixel count.
The element block types are listed in the `dev/data/ui.json` file. It will
automatically update the configuration file.

If this script is used to move an element block down, you will be asked if you
want to resize the main window by the same number of pixels you move the block.

Usage of the script:

    dev/ui_tools/move_element_block.pl

    Parameters:

    -e|--element    Mandatory: The element type
    -d|--direction  Mandatory: 'down' or 'up'
    -p|--pixels     Mandatory: The number of pixels to move the elements

#### Resize main window

This script is `dev/ui_tools/resize_window.pl`.

It allows you to resize the main window dynamically. It will automatically
update the configuration file.

Usage of the script:

    dev/ui_tools/resize_window.pl

    If no parameters are sent in, we'll display the current window size.

    if only one of -x or -y are sent in, we'll re-use the existing setting
    for the missing value.

    Parameters:

    -x Optional: Number of horizontal pixels
    -y Optional: Number of vertical pixels
    -h Optional: Display this help screen

&copy; 2016-2023 by Steve Bertrand