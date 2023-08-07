use warnings;
use strict;

# resize_window.pl

# Resizes the main UI window

use FindBin qw($RealBin);
use lib "$RealBin/../../lib";

use BuildHelper qw(:all);
use Getopt::Long;

my $ui_conf_file = 'dev/data/ui.json';
my $data = BuildHelper::config_read($ui_conf_file);

my ($x_current, $y_current) = BuildHelper::ui_window_size($data);
my ($x, $y, $help);

GetOptions (
    'x=i'       => \$x,
    'y=i'       => \$y,
    'h|help'    => \$help
);

if ($help) {
    help();
}

$x //= $x_current;
$y //= $y_current;

if ($x != $x_current && $y != $y_current) {
    $data = BuildHelper::ui_window_size($data, $x, $y);
    BuildHelper::config_write($ui_conf_file, $data);
    print "\nUpdated the '$ui_conf_file' with the updated main window size...\n\n";
}
else {
    print "\nNo new coordinates sent in. Existing window is '$x' x '$y' pixels...\n\n";
}

sub help {
    print qq{

        If no parameters are sent in, we'll display the current window size.

        if only one of -x or -y are sent in, we'll re-use the existing setting
        for the missing value.

        Parameters:

        -x Optional: Number of horizontal pixels
        -y Optional: Number of vertical pixels
        -h Optional: Display this help screen

    };
    exit;
}