use warnings;
use strict;

# move_element_block.pl

# Moves all elements of a certain type up or down within the UI,
# asks the user if they'd like to expand the main window size,
# and then updates the UI configuration file

use FindBin qw($RealBin);
use lib "$RealBin/../../lib";

use BuildHelper qw(:all);
use Getopt::Long;

my ($element_type, $direction, $pixels);

GetOptions (
    'e|element=s'   => \$element_type,
    'd|direction=s' => \$direction,
    'p|pixels=s'    => \$pixels,
);

if (! $element_type || ! $direction || ! defined $pixels) {
    help();
}
if ($direction !~ /(?:up|down)/) {
    print "\n\t--direction must be 'up' or 'down'\n\n";
    exit;
}

if ($pixels !~ /^\d+$/) {
    print "\n\t--pixels must be an unsigned integer\n\n";
    exit;
}

my $ui_conf_file = 'dev/data/ui.json';
my $data = BuildHelper::config_read($ui_conf_file);

if (! grep { $element_type eq $_ } grep { $_ !~ /^ui_/ } keys %$data) {
    my $element_list = join ', ', grep { $_ !~ /^ui/ } keys %$data;

    print "\n--element-type argument must be one of '$element_list'\n\n";
    exit;
}

$data = BuildHelper::ui_change_element_block_location(
    $data,
    $element_type,
    $direction,
    $pixels
);

if ($direction eq 'down') {
    print "\nWould you like to expand the size of the main window by '$pixels' pixels [y|n]?";
    my $input = <>;

    if ($input =~ /(?:y|Y/) {
        my @window_size = BuildHelper::ui_window_size($data);
        $window_size[1] += $pixels;
    }
}

BuildHelper::config_write($ui_conf_file, $data);

print "\nUpdated the '$ui_conf_file' with the updated element locations\n\n";

sub help {
    print qq{
        Parameters:

        -e|--element    Mandatory: The element type
        -d|--direction  Mandatory: 'down' or 'up'
        -p|--pixels     Mandatory: The number of pixels to move the elements

    };
    exit;
}