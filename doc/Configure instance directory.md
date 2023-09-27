# Configure instance directory

If using the [installer](download/berrybrewInstaller.exe?raw=true "berrybrew MSI installer")
to install from, you'll have the opportunity to configure this option during
install, and nothing further is required.

Otherwise, follow these directions:

By default, we manage Perls out of the `C:\berrybrew` directory. To
change this, modify the `instance_dir` value in the `data\config.json` file.
Use double-backslashes (`\\`) as the path separators.

WARNING: At this time, it is highly advised not to change this after
you've already installed any instances of Perl. This feature is
incomplete, and `PATH` and other things don't get properly reset yet.
If you choose to ignore this, follow this procedure:

- Create a new directory in the file system to house the Perl instances

- Run `berrybrew options instance_dir PATH`, where `PATH` is the full path to the
  directory you'd like to store Perls in

- Run `berrybrew options temp_dir PATH`, where `PATH` is the full path to the
  temporary storage area. Typically, this is a directory inside of the `root_dir`
  you set above

- Run `berrybrew off`, to flush the `PATH` environment variables

- Move all Perl installations from the old path to the new one

- Remove the old directory

- Run `berrybrew switch $version` to set things back up

&copy; 2016-2023 by Steve Bertrand