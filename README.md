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
- [Configure Perl Instance Directory](#configure-instance-directory)
- [Requirements](#requirements)
- [Troubleshooting](#troubleshooting)
- [Development, Build, Test and Release](#development-build-test-and-release)
- [Documentation](#other-documentation)
- [Hidden Commands](#hidden-commands)
- [Developed Using](#developed-using)
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

See the [Uninstall](doc/Uninstall.md) documentation.

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

See the [berrybrew](doc/berrybrew.md) document for usage examples.

## Upgrading

Using the [installer](download/berrybrewInstaller.exe?raw=true "berrybrew MSI installer")
is the best and safest way to upgrade your `berrybrew`. You can stop reading here
if you use the installer to install `berrybrew`.

As of version 1.42, a registry export of berrybrew's configuration will be
saved to the `backup` directory within the installation directory. You can use
these settings if anything you didn't want overwritten was.

## Update Perls Available

Use the `Fetch` button in the UI, or, at the command line, use `berrybrew fetch`
to retrieve the most recent availability list from Strawberry Perl. If any new or
changed versions are found, we'll update the local `perls.json` file with them.

## Configure Instance Directory

If using the [installer](download/berrybrewInstaller.exe?raw=true "berrybrew MSI installer")
to install from, you'll have the opportunity to configure this option during
install, and nothing further is required.

Otherwise, read [this document](doc/Configure%20instance%20directory.md).

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

## Developed Using

|Software|Description|Notes|
|---|---|---|
|[Jetbrains Rider](https://www.jetbrains.com/rider/)|.Net IDE|Thanks to their [Open Source Licensing](https://www.jetbrains.com/buy/opensource/)|
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
