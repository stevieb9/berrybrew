use warnings;
use strict;

# This script prepares a complete release of berrybrew

use FindBin qw($RealBin);
use lib "$RealBin/../lib";

use Archive::Zip qw(:ERROR_CODES :CONSTANTS);
use BuildHelper qw(:all);
use Data::Dumper;
use Digest::SHA qw(sha1);
use Dist::Mgr qw(changes_date);
use File::Copy;
use File::Find::Rule;
use JSON::PP;
use Test::More;

use constant {
    INSTALLER_SCRIPT => 'dev/create_prod_installer.nsi',
    EXE_FILE         => 'download/berrybrewInstaller.exe',
    ZIP_FILE         => 'download/berrybrew.zip',
};

# Arg to bypass makensis check

my $testing = $ARGV[0];

# run checks

if (! $testing && ! grep { -x "$_/makensis.exe" } split /;/, $ENV{PATH}){
    die "makensis.exe not found, check your PATH. Can't build installer...";
}

my $data_dir        = 'data';
my $bak_dir         = 'bak';
my $defaults_dir    = 'dev/data';

BuildHelper::check_installer_manifest(INSTALLER_SCRIPT);
backup_configs();
compile();
update_perls_available();
changes_date();
create_changes();
create_zip();
BuildHelper::update_installer_script(INSTALLER_SCRIPT);
BuildHelper::create_installer(INSTALLER_SCRIPT);
update_readme();
check_readme();
update_license();
check_license();
update_contributing();
check_contributing();
update_docs();
finish();

done_testing();

sub backup_configs {

    if (!-d $bak_dir) {
        mkdir $bak_dir or die $!;
        print "created backup dir, $bak_dir\n";
    }

    my @files = glob "$data_dir/*";

    for (@files) {
        copy $_, $bak_dir or die $!;
        print "copied $_ to $bak_dir\n";
    }

    @files = glob "$defaults_dir/*";

    for (@files) {
        copy $_, $data_dir or die $!;
        print "copied $_ to $data_dir\n";
    }
}
sub check_contributing {
    open my $fh, '<', 'CONTRIBUTING.md' or die "Can't open CONTRIBUTING.md: $!";

    my ($current_year) = (localtime)[5];
    $current_year += 1900;

    my $year_found = 0;

    while (<$fh>) {
        if (/^.*2016-(\d{4}) by Steve Bertrand/) {
            my $copyright_year = $1;
            is
                $copyright_year,
                $current_year,
                "CONTRIBUTING.md copyright year updated ok";

            $year_found = 1;
            last;
        }
    }

    is $year_found, 1, "Found and changed the copyright year in CONTRIBUTING.md ok";
}
sub check_license {
    open my $fh, '<', 'LICENSE' or die "Can't open LICENSE: $!";

    my ($current_year) = (localtime)[5];
    $current_year += 1900;
   
    my $year_found = 0;
    
    while (<$fh>) {
        if (/^Copyright \(c\) 2016-(\d{4}) by Steve Bertrand/) {
            my $license_year = $1;
            is $license_year, $current_year, "LICENSE copyright year updated ok";
            $year_found = 1;
            last;
        }
    }

    is $year_found, 1, "Found and changed the copyright year in LICENSE ok";
}
sub check_readme {
    open my $fh, '<', 'README.md' or die "Can't open README: $!";
    my ($zip_sha, $inst_sha, $readme_ver);
    my $ver = _berrybrew_version();
    my $c = 0;

    while (<$fh>) {

        if (/^\[berrybrew\.zip/) {
            if (/^\[berrybrew\.zip.*`SHA1:\s+(.*)`/) {
                $zip_sha = $1;
            }
            like $zip_sha, qr/[A-Fa-f0-9]{40}/, "berrybrew zip archive SHA1 is a checksum ok";
            my $actual_zip_sha = _generate_shasum(ZIP_FILE);
            is $zip_sha, $actual_zip_sha, "...and the README has the correct checksum for the zip file";
        }
       
        if (/^\[berrybrewInstaller\.exe/) {
            if (/^\[berrybrewInstaller\.exe.*`SHA1:\s+(.*)`/) {
                $inst_sha = $1;
                print(length($1));
            }
            like $inst_sha, qr/[A-Fa-f0-9]{40}/, "berrybrew installer SHA1 is a checksum ok";
            my $actual_inst_sha = _generate_shasum(EXE_FILE);
            is $inst_sha, $actual_inst_sha, "...and the README has the correct checksum for the installer";
        }
        if (/## Version/) {
            $c++;
            next;
        }
        if ($c == 1) {
            $c++;
            next;
        }
        if ($c == 2) {
            if (/(\d+\.\d+)/) {
                $readme_ver = $1;
            }
            is $readme_ver, $ver, "Version was updated ok";
            $c++;
        }
    }        
}
sub compile {
    print "\ncompiling the berrybrew core API dll...\n";

    my $api_build = "" .
        "mcs " .
        "src/berrybrew.cs " .
        "src/messaging.cs " .
        "src/pathoperations.cs " .
        "src/perlinstance.cs " .
        "src/perloperations.cs " .
        "-lib:bin " .
        "-t:library " .
        "-out:bin/bbapi.dll " .
        "-r:Newtonsoft.Json.dll " .
        "-r:ICSharpCode.SharpZipLib.dll";

    system $api_build;

    print "\ncompiling the berrybrew binary...\n";

    my $bin_build = "" .
        "mcs " .
        "src/bbconsole.cs " .
        "-lib:bin  " .
        "-out:bin/berrybrew.exe " .
        "-r:bbapi.dll  " .
        "-win32icon:inc/berrybrew.ico";

    system $bin_build;
    
    print "\ncompiling the berrybrew UI...\n";
 
    my $ui_build = "" .
        "mcs " .
        "src/berrybrew-ui.cs " .
        "src/perloperations.cs " .
        "-lib:bin " .
        "-t:winexe " .
        "-out:bin/berrybrew-ui.exe " .
        "-r:bbapi.dll " .
        "-r:Microsoft.VisualBasic.dll " .
        "-r:Newtonsoft.Json.dll " .
        "-r:System.Drawing.dll " .
        "-r:System.Windows.Forms.dll " .
        "-win32icon:inc/berrybrew.ico " .
        "-win32manifest:berrybrew.manifest";

    system $ui_build;        
    
    print "\nCopying berrybrew.exe to bb.exe...\n";
    
    copy 'bin/berrybrew.exe', 'bin/bb.exe' or die $!;
}
sub create_zip {
    print "\npackaging pre-built zipfile...\n";

    my $zip = Archive::Zip->new;

    chdir ".." or die $!;

    $zip->addTree('berrybrew/bin', 'bin', sub {!/Debug/});
    $zip->addTree("berrybrew/$defaults_dir", 'data');
    $zip->writeToFileNamed('berrybrew/download/berrybrew.zip');

    chdir "berrybrew" or die $!;
}
sub create_changes {
    print "\nGenerating a Changes markdown file...\n";

    my $changes = 'Changes';
    my $changes_md = 'Changes.md';

    copy($changes, $changes_md) or die $!;

    open my $changes_fh, '<', $changes or die $!;
    open my $changes_md_wfh, '>', $changes_md or die $!;

    while (<$changes_fh>) {
        if ($_ !~ /^$/ && $_ !~ /^\s+$/) {
            s/^\s+//;
        }
        print $changes_md_wfh $_;
    }
}
sub finish {
    print "\nDone!\n";
}
sub update_perls_available {
    my $out = `bin/berrybrew.exe fetch`;
    like $out, qr/Successfully updated/, "available perls updated ok";
   
    is
        eval { copy 'data/perls.json', 'dev/data/perls.json' or die "can't copy perls.json: $!"; 1 },
        1,
        "data/perls.json copied to dev/data ok";
}
sub update_contributing {
    print "\nupdating CONTRIBUTING.md with new Copyright year...\n";

    open my $fh, '<', 'CONTRIBUTING.md' or die $!;
    my @contents = <$fh>;
    close $fh or die $!;

    my ($current_year) = (localtime)[5];
    $current_year += 1900;

    my $changed = 0;

    for (@contents) {
        if (/^.*2016-(\d{4}) by Steve Bertrand/) {
            my $copyright_year = $1;
            if ($copyright_year != $current_year) {
                $changed = 1;
                s/$copyright_year/$current_year/;
            }
        }
    }

    if ($changed) {
        open my $wfh, '>', 'CONTRIBUTING.md' or die $!;

        for (@contents) {
            print $wfh $_;
        }
    }
}
sub update_docs {
    print "\nlooking for docs that need copyright updated...\n";
    
    my @docs  = File::Find::Rule->file
                                ->name('*.md')
                                ->in('doc/');
   
    for my $doc (@docs) {
        open my $fh, '<', $doc or die $!;
        my @contents = <$fh>;
        close $fh or die $!;

        my ($current_year) = (localtime)[5];
        $current_year += 1900;

        my $changed = 0;

        for (@contents) {
            if (/^\&copy; 2016-(\d{4}) by/) {
                my $license_year = $1;
                if ($license_year != $current_year) {
                    $changed = 1;
                    s/$license_year/$current_year/;
                }
            }
        }

        if ($changed) {
            print "\nupdating $doc with new Copyright year...\n";
            open my $wfh, '>', $doc or die $!;

            for (@contents) {
                print $wfh $_;
            }
        }
    } 
}
sub update_readme {
    print "\nupdating README with new SHA1 sums and version...\n";

    open my $fh, '<', 'README.md' or die $!;
    my @contents = <$fh>;
    close $fh or die $!;

    my $bb_ver = _berrybrew_version();
    my $zip_sha = _generate_shasum(ZIP_FILE);
    my $exe_sha = _generate_shasum(EXE_FILE);

    my $c = 0;

    for (@contents) {
        if (/^\[berrybrew\.zip.*(`SHA1:.*`)/) {
            s/$1/`SHA1: $zip_sha`/;
        }
        if (/^\[berrybrewInstaller\.exe.*(`SHA1:.*`)/) {
            s/$1/`SHA1: $exe_sha`/;
        }
        if (/## Version/) {
            $c++;
            next;
        }
        if ($c == 1) {
            $c++;
            next;
        }
        if ($c == 2) {
            s/\d+\.\d+/$bb_ver/;
            $c++;
        }
    }

    open my $wfh, '>', 'README.md' or die $!;

    for (@contents) {
        print $wfh $_;
    }
}
sub update_license {
    print "\nupdating LICENSE with new Copyright year...\n";

    open my $fh, '<', 'LICENSE' or die $!;
    my @contents = <$fh>;
    close $fh or die $!;

    my ($current_year) = (localtime)[5];
    $current_year += 1900;
   
    my $changed = 0;
    
    for (@contents) {
        if (/^Copyright \(c\) 2016-(\d{4}) by Steve Bertrand/) {
            my $license_year = $1;
            if ($license_year != $current_year) {
                $changed = 1;
                s/$license_year/$current_year/;
            }
        }
    }

    if ($changed) {
        open my $wfh, '>', 'LICENSE' or die $!;

        for (@contents) {
            print $wfh $_;
        }
    } 
}
sub _generate_shasum {
    my ($file) = @_;

    if (! defined $file){
        die "_generate_shasum() requres a filename sent in";
    }

    print "\ncalculating SHA1 for $file...\n";

    my $digest = `shasum $file`;
    $digest = (split /\s+/, $digest)[0];

    return $digest;
}
sub _berrybrew_version {
    open my $fh, '<', 'src/berrybrew.cs' or die $!;

    my $c = 0;
    my $ver;

    while (<$fh>) {

        if (/public string Version\(\)\s+\{/) {
            $c = 1;
            next;
        }
        if ($c == 1) {
            ($ver) = $_ =~ /(\d+\.\d+)/;
            last;
        }
    }

    close $fh;

    return $ver;
}
