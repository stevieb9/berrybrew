use warnings;
use strict;

use FindBin qw($RealBin);
use lib "$RealBin/../../lib";

use BuildHelper qw(:all);

my $ui_conf_file = 'dev/data/ui.json';
my $data = BuildHelper::config_read($ui_conf_file);

BuildHelper::ui_change_element_block_location(
    $data,
    'checkbox',
    'down',
    20
);
