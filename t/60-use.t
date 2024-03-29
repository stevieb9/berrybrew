use warnings;
use strict;

use FindBin qw($RealBin);
use lib $RealBin;
use BB;

use IPC::Open3;
use POSIX ':sys_wait_h';
use Symbol 'gensym';
use Test::More;

BB::check_test_platform();

$ENV{BERRYBREW_ENV} = "testing";

my $c = $ENV{BBTEST_REPO}
    ? "$ENV{BBTEST_REPO}/testing/berrybrew"
    : 'c:/repos/berrybrew/testing/berrybrew';

my @installed = BB::get_installed();
my @avail = BB::get_avail();
my $cloned;

{ # test output
    
    my $err = BB::trap("$c use 5.xx.x_64");
    is $? >> 8, BB::err_code('PERL_NOT_INSTALLED'), "exit status if no Perls installed";
    like
        $err,
        qr/The selected Perl/s,
        "complain if no perls installed errmsg ok";
    like 
        $err,
        qr/Can't launch/,
        "complain if this perl isn't installed errmsg ok";

    $err = BB::trap("$c use --win 5.xx.x_64,5.yy.y_64");
    is $? >> 8, BB::err_code('PERL_NOT_INSTALLED'), "exit status if no Perls installed";
    like
        $err,
        qr/The selected Perl/s,
        "complain if multi perl not installed with --win errmsg ok";
    like
        $err,
        qr/Can't launch/,
        "complain if this perl isn't installed errmsg ok";

    $err = BB::trap("$c use --win 5.10.1_32");
    is $? >> 8, BB::err_code('PERL_NOT_INSTALLED'), "exit status ok if use tries to use a valid but not installed Perl";
    like
        $err,
        qr/Perl version 5.10.1_32/s,
        "complain with version if the version is valid but not installed errmsg ok";
}

while(@installed < 1) {
    note
        "\nInstalling $avail[-1] because only " .scalar(@installed) .
        " test perl".(@installed==1?' was':'s were')." installed\n";

    `$c install $avail[-1]`;

    @installed = BB::get_installed();
    @avail = BB::get_avail();
}

# get rid of 'dup' from 55-register.t, and get rid of
# 'myclone' from a previous run of this test

for my $delme (qw/dup myclone/)
{
    if( join("\0", '', BB::get_installed(), '') =~ /\0$delme\0/ ){
        `$c remove $delme`
    }
}

# create a clone

@installed = BB::get_installed();
note "\nCloning $installed[-1] to myclone\n";
`$c clone $installed[-1] myclone`;
$cloned = $installed[-1];

like
    join("\0", '', @installed = BB::get_installed(), ''),
    qr/\0myclone\0/,
    'verifying myclone exists';

# testing single 'berrybrew use' inside the same "window"
{   
    my $name = 'berrybrew use myclone';
    my $pid
        = open3( my $pinn, my $pout, my $perr = gensym, $c, 'use', 'myclone');

    # run some commands in the environment
    print {$pinn} "where perl\n";
    print {$pinn} 'perl -le "print $]"', $/;
    print {$pinn} "exit\n";
    print {$pinn} "where perl\n";
    print {$pinn} 'perl -le "print $]"', $/;
    print {$pinn} "exit\n";

    my $t0 = time;
    while ( (time() - $t0) < 10 ) { # wait no more than 10s
        my $kid = waitpid($pid, WNOHANG);
        note "waitpid($pid)=$kid";
        last unless $kid < 0;
        sleep(1);
    }
    my $xout = join '', <$pout>;
    my $xerr = join '', <$perr>;

    diag "`$name` resulted in STDERR=\"$xerr\"\n" if $xerr;
    is $xerr, '', "$name: STDERR should be empty";

    my $re
     = qr/where perl\s*(\S*berrybrew-testing.instance.myclone.perl.bin.perl\.exe)\s*$/ims;

    like $xout, $re, $name.': found myclone berrybrew perl';
    foreach ( $xout =~ m/$re/gims ) {
        note "$name: found perl path: '$_'\n";
    }
    like $xout, qr/perl -le "[^"]+"\s*5\.\d+/im,  $name.': found perl version';
    foreach ( $xout =~ /perl -le "[^"]+"\s*(5\.\d+)/gim ) {
        note "$name: found perl version: $_\n";
    }
}

# testing multiple 'berrybrew use' versions inside the same "window"
{   
    my $name = "berrybrew use $cloned,myclone";
    my $pid = open3(
        my $pinn, my $pout, my $perr = gensym,
        $c, 'use', join(',', $cloned, 'myclone')
    );

    # run some commands in the environment
    print {$pinn} "where perl\n";
    print {$pinn} 'perl -le "print $]"', $/;
    print {$pinn} "exit\n";
    print {$pinn} "where perl\n";
    print {$pinn} 'perl -le "print $]"', $/;
    print {$pinn} "exit\n";

    my $t0 = time;
    while ( (time() - $t0) < 10 ) { # wait no more than 10s
        my $kid = waitpid($pid, WNOHANG);
        note "waitpid($pid)=$kid";
        last unless $kid < 0;
        sleep(1);
    }
    my $xout = join '', <$pout>;
    my $xerr = join '', <$perr>;

    diag "`$name` resulted in STDERR=\"$xerr\"\n" if $xerr;
    is $xerr, '', "$name: STDERR should be empty";

    my $re
      = qr/where perl\s*(\S*berrybrew-testing.instance.$cloned.perl.bin.perl\.exe)\s*$/ims;

    like $xout, $re, $name.": found berrybrew perl $cloned";
    foreach ( $xout =~ m/$re/gims ) {
        note "$name: perl path: '$_'\n";
    }

    $re
      = qr/where perl\s*(\S*berrybrew-testing.instance.myclone.perl.bin.perl\.exe)\s*$/ims;

    like $xout, $re, $name.': found berrybrew perl myclone';
    foreach ( $xout =~ m/$re/gims ) {
        note "$name: perl path: '$_'\n";
    }

    $re = qr/perl -le "[^"]+"\s*(5\.\d+)\s*$/im;
    like $xout, $re,  $name.': found perl versions';
    foreach ( $xout =~ /$re/gim ) {
        note "$name: perl version: $_\n";
    }
}

# testing single 'berrybrew use' versions in separate window;
{   
    #   cannot (conveniently) send commands to the spawned window,
    #   so just check that the parent process gets the PID when
    #   BBTEST_SHOW_PID is set
    my $name = "berrybrew use --win myclone";
    local $ENV{BBTEST_SHOW_PID} = 1;
    my $pid = open3(
        my $pinn, my $pout, my $perr = gensym, $c,
        'use', '--win', join(',', 'myclone')
    );

    my $t0 = time;
    while ( (time() - $t0) < 10 ) { # wait no more than 10s
        my $kid = waitpid($pid, WNOHANG);
        note "waitpid($pid)=$kid";
        last unless $kid < 0;
        sleep(1);
    }
    my $xout = join '', <$pout>;
    my $xerr = join '', <$perr>;

    diag "`$name` resulted in STDERR=\"$xerr\"\n" if $xerr;
    is $xerr, '', "$name: STDERR should be empty";

    my @matches
        = $xout =~ /: spawned in new command window, with PID=(\d+)/gims;

    is scalar @matches, 1 , "$name: spawn one window";
    my $count = 0;
    for(@matches) {
        # wait a half-second before killing each window # was: sleep(1);
        select undef, undef, undef, 0.5;
        note "kill PID#$_\n";
        $count += kill KILL => $_;
    }
    is $count, scalar @matches, "$name: kill one window";
}

# testing single 'berrybrew use' versions in separate window (Powershell)
{
    #   cannot (conveniently) send commands to the spawned window,
    #   so just check that the parent process gets the PID when
    #   BBTEST_SHOW_PID is set
   
    my $powershell_option = `$c options shell powershell`;
    like `$c options shell`, qr/powershell/, "powershell is set ok";
    
    my $name = "berrybrew use --win myclone";
    local $ENV{BBTEST_SHOW_PID} = 1;
    my $pid = open3(
        my $pinn, my $pout, my $perr = gensym, $c,
        'use', '--win', join(',', 'myclone')
    );

    my $t0 = time;
    while ( (time() - $t0) < 10 ) { # wait no more than 10s
        my $kid = waitpid($pid, WNOHANG);
        note "waitpid($pid)=$kid";
        last unless $kid < 0;
        sleep(1);
    }
    my $xout = join '', <$pout>;
    my $xerr = join '', <$perr>;

    diag "`$name` resulted in STDERR=\"$xerr\"\n" if $xerr;
    is $xerr, '', "$name: STDERR should be empty";

    my @matches
        = $xout =~ /: spawned in new command window, with PID=(\d+)/gims;

    is scalar @matches, 1 , "$name: spawn powershell one window";
    my $count = 0;
    for(@matches) {
        # wait a half-second before killing each window # was: sleep(1);
        select undef, undef, undef, 0.5;
        note "kill PID#$_\n";
        $count += kill KILL => $_;
    }
    is $count, scalar @matches, "$name: kill powershell one window";

    $powershell_option = `$c options shell cmd`;
    like `$c options shell`, qr/cmd/, "cmd is set ok";   
}

# testing multiple 'berrybrew use' versions in separate windows;
{   
    #   cannot (conveniently) send commands to the spawned windows,
    #   so just check that the parent process gets the PID when
    #   BBTEST_SHOW_PID is set
    my $name = "berrybrew use --win $cloned,myclone";
    local $ENV{BBTEST_SHOW_PID} = 1;
    my $pid = open3(
        my $pinn, my $pout, my $perr = gensym, $c,
        'use', '--win', join(',', $cloned, 'myclone')
    );

    my $t0 = time;
    while ( (time() - $t0) < 10 ) { # wait no more than 10s
        my $kid = waitpid($pid, WNOHANG);
        note "waitpid($pid)=$kid";
        last unless $kid < 0;
        sleep(1);
    }
    my $xout = join '', <$pout>;
    my $xerr = join '', <$perr>;

    diag "`$name` resulted in STDERR=\"$xerr\"\n" if $xerr;
    is $xerr, '', "$name: STDERR should be empty";

    my @matches
      = $xout =~ /: spawned in new command window, with PID=(\d+)/gims;

    is scalar @matches, 2 , "$name: spawn two windows";
    my $count = 0;
    for(@matches) {
        # wait a half-second before killing each window # was: sleep(1);
        select undef, undef, undef, 0.5;
        note "kill PID#$_\n";
        $count += kill KILL => $_;
    }
    is $count, scalar @matches, "$name: kill two windows";
}

done_testing();
