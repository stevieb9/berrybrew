use warnings;
use strict;

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

if (! $element_type || ! $direction || ! $pixels) {
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

BuildHelper::ui_change_element_block_location(
    $data,
    $element_type,
    $direction,
    $pixels
);
sub help {
    print qq{
        Parameters:

        -e|--element    Mandatory: The element type
        -d|--direction  Mandatory: 'down' or 'up'
        -p|--pixels     Mandatory: The number of pixels to move the elements

    };
    exit;
}