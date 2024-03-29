# berrybrew

The perlbrew for Windows Strawberry Perl! 

### [Click here to download the installer](download/berrybrewInstaller.exe?raw=true "berrybrew MSI installer")

For a quick-start, jump to the [Install](#install) and [Commands](#commands)
sections.

`berrybrew` can download, install, remove and manage multiple concurrent
versions of Strawberry Perl for Windows. There is no 
[requirement](#requirements "berrybrew requirements")
to have Strawberry Perl installed before using `berrybrew`.

Use the **bb** command as a short hand name for **berrybrew**.

There is extensive documentation available for the [berrybrew](doc/berrybrew.md)
application, as well as the [Berrybrew API](doc/Berrybrew%20API.md).

See [Other Documentation](#other-documentation) for the  full list of
documentation.

## Table of Contents

- [Install](#install)
- [Uninstall](#uninstall)
- [Configuration](#configuration)
- [Commands](#commands)
- [Examples](#examples)
- [Update Perls Available](#update-perls-available)
- [Configure Perl Instance Directory](#configure-root-directory)
- [Requirements](#requirements)
- [Troubleshooting](#troubleshooting)
- [Development, Build, Test and Release](#development-build-test-and-release)
- [Documentation](#other-documentation)
- [Hidden Commands](#hidden-commands)
- [Developed Using](#developed-using)
- [Caveats](#caveats)
- [License](#license)
- [Version](#version)

## Install

##### Self-installing executable

The easiest and most straight forward method.

[berrybrewInstaller.exe](download/berrybrewInstaller.exe?raw=true "berrybrew MSI installer") `SHA1: 5917766c003525f736a5cf409e1c4d0fd4cb345f`

##### Git clone

    git clone https://github.com/stevieb9/berrybrew
    cd berrybrew
    bin\berrybrew.exe config

##### Pre-built zip archive

[berrybrew.zip](download/berrybrew.zip?raw=true "berrybrew zip archive") `SHA1: fdf4cf104cff8d6513d0511f383c034d203c1022`

After extraction:

    cd berrybrew
    bin\berrybrew.exe config

#### Compile your own
    
You can also [Compile your own](doc/Compile%20your%20own.md)
installation.

## Uninstall

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

- If you've installed the UI, remove the 
`Computer\HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\Current Version\Run\BerrybrewUI`
  registry value

## Configuration

See the [Configuration](doc/Configuration.md)
document, and the `options` command in the [berrybrew](doc/berrybrew.md)
documentation.

Several of the modifiable options are configurable through the UI.

## Commands

See the [berrybrew](doc/berrybrew.md) documentation for a full explanation of
all of the following commands.

For all commands that require the name of a Perl (eg: `install`), we will default
to 64-bit (ie. `_64`) if this suffix is omitted.

    berrybrew <command> [subcommand] [option]

    associate *    View and set Perl file association
    available *    List available Strawberry Perl versions and which are installed
    list           List installed Strawberry Perl versions
    clean *        Remove all temporary berrybrew files
    clone          Make a complete copy of a Perl installation
    config         Add berrybrew to your PATH
    exec *         Run a command for every installed Strawberry Perl
    fetch          Update the list of Strawberry Perl instances available
    hidden         Display the list of hidden/development commands
    install        Download, extract and install a Strawberry Perl
    modules *      Export and import a module list from one Perl to install on another
    options *      Display or set a single option, or display all of them with values
    off            Disable berrybrew perls (use 'switch' to re-enable)
    register       Manually register a custom installation directory
    remove         Uninstall a Strawberry Perl
    snapshot *     Export and import snapshots of Perl instances
    switch *       Switch to use a different Strawberry Perl
    unconfig       Remove berrybrew from PATH
    use *          Use a specific Strawberry Perl version temporarily
    virtual        Allow berrybrew to manage an external Perl instance
    help           Display this help screen
    license        Show berrybrew license
    version        Displays the version


    * - view subcommand details with 'berrybrew <command> help'

## Examples

See the [berrybrew](doc/berrybrew.md)
document for usage examples.

## Upgrading

Using the [installer](download/berrybrewInstaller.exe?raw=true "berrybrew MSI installer")
is the best and safest way to upgrade your `berrybrew`. You can stop reading here
if you use the installer to install `berrybrew`.

Doing a straight `git pull` will overwrite your configuration files, so
back them up first (see [Caveats](#caveats)).

## Update Perls Available

Use the `Fetch` button in the UI, or, at the command line, use `berrybrew fetch`
to retrieve the most recent availability list from Strawberry Perl. If any new or
changed versions are found, we'll update the local `perls.json` file with them.

## Configure Root Directory

If using the [installer](download/berrybrewInstaller.exe?raw=true "berrybrew MSI installer")
to install from, you'll have the opportunity to configure this option during
install, and nothing further is required.

Otherwise, follow these directions:

By default, we manage Perls out of the `C:\berrybrew` directory. To 
change this, modify the `root_dir` value in the `data\config.json` file.
Use double-backslashes (`\\`) as the path separators. 

WARNING: At this time, it is highly advised not to change this after 
you've already installed any instances of Perl. This feature is 
incomplete, and `PATH` and other things don't get properly reset yet.
If you choose to ignore this, follow this procedure:

- create a new directory in the file system to house the Perl instances

- run `berrybrew options root_dir PATH`, where `PATH` is the full path to the
directory you'd like to store Perls in

- run `berrybrew options temp_dir PATH`, where `PATH` is the full path to the
temporary storage area. Typically, this is a directory inside of the `root_dir`
you set above

- run `berrybrew off`, to flush the `PATH` environment variables

- move all Perl installations from the old path to the new one

- remove the old directory

- run `berrybrew switch $version` to set things back up

## Requirements

- .Net Framework 2.0 or higher

- Windows only!

- [Mono](http://www.mono-project.com) or Visual Studio (only if 
compiling your own version)

## Troubleshooting

If you run into trouble installing a Perl, try clearing the berrybrew
cached downloads by running `berrybrew clean`. 

You can also enable debugging to get more verbose output on the command
line:

    berrybrew debug <command> [options] 

## Development, Build, Test and Release

Contains all information relating to the development, build, test and release
cycle of the `berrybrew` ecosystem.

See the [Berrybrew Development, Build, Test and Release](doc/Berrybrew%20development.md)
document.

## Other Documentation 

- [berrybrew](doc/berrybrew.md)
 Full documentation for the application

- [Berrybrew API](doc/Berrybrew%20API.md)
 API documentation

- [Configuration](doc/Configuration.md)
 Guide to various configuration files and options

## Hidden Commands

Please see the [hidden commands](doc/berrybrew.md#hidden-commands)
in the [berrybrew](doc/berrybrew.md)
document.

You can also get a list of them by running the hidden `berrybrew hidden` command.

## CAVEATS

- When using `git pull` to do an upgrade, your configuration files will
be overwritten with the defaults. If you have any customizations, make a
backup of the `data` directory before upgrade, then copy the files back
to their original location. Note that you may have to manually add any
new config directives into the original config files. The 
`perls_custom.json` file used for custom Perl installations (clones) and the
`perls_virtual.json` file used for virtual Perl installations will never be
overwritten, and this warning does not apply for them.

- At this time, `berrybrew` requires Administrative privileges to
operate correctly. This is due to the way Windows forces the System 
`PATH` to take precedence over User `PATH`.

## Developed Using

|Software|Description|Notes|
|---|---|---|
|[Jetbrains Rider](https://www.jetbrains.com/rider/)|.Net IDE|Thanks to their [Open Source Licensing](https://www.jetbrains.com/buy/opensource/)
|[Jetbrains intelliJ IDEA](https://www.jetbrains.com/idea/)|IDE for Perl coding|Freely available, also comes with the open source license|
|[Camelcade Perl5 Plugin](https://github.com/Camelcade/Perl5-IDEA)|Perl5 Plugin for intelliJ IDEA||
|[Devel::Camelcadedb](https://metacpan.org/pod/distribution/Devel-Camelcadedb/lib/Devel/Camelcadedb.pod)|Adds Perl5 debug support for intelliJ IDEA||
|[Mono](https://www.mono-project.com/)|Open Source .Net Framework||
|[Mono C# Compiler](https://www.mono-project.com/docs/about-mono/languages/csharp/)|C#|Open Source C# Compiler|

## License

2 Clause FreeBSD - see [LICENSE](/LICENSE).

## Original Author

David Farrell

## Current Author 

Steve Bertrand `steveb<>cpan.org`

## See Also

- [StrawberryPerl](http://strawberryperl.com) - Strawberry Perl for
Windows

- [Perlbrew](http://perlbrew.pl) - the original Perl version manager for
Unix based systems.

## Version

1.41 
