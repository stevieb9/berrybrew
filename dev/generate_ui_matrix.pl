use warnings;
use strict;

use Data::Dumper;
use JSON;
use Tkx;

# NOTE: If a UTF error occurs reading the JSON, open the conf
# file up in vi and execute: ':set nobomb'

my $ui_conf_file = 'dev/data/ui.json';
my $data = _parse_config($ui_conf_file);

window_display();

sub buttons {
    my $button_conf = $data->{button};

    my @buttons;

    for (sort { $button_conf->{$a}{index} <=> $button_conf->{$b}{index} } keys %$button_conf) {
        push @buttons, $button_conf->{$_};
    }

    return @buttons;
}
sub window_size {
    return @{ $data->{ui_object}{client_size} };
}
sub window_display {
    my $mw = Tkx::widget->new(".");
    # Window
    $mw->g_wm_title("Hello, world");
    $mw->g_wm_minsize(window_size());

    # Button

    for my $button (buttons()) {
        my $b = $mw->new_button(
            -text   => $button->{text},

        );

        #my $text_width  = Tkx::font_measure($font, $text);

        $b->g_place(
            -width  => $button->{size}[0],
            -height => $button->{size}[1],
            -x      => $button->{location}[0],
            -y      => $button->{location}[1]
        );
#        $b->g_pack;
    }
    Tkx::MainLoop();
}

sub _parse_config {
    my ($file) = @_;
    local $/;
    open my $fh, '<', $ui_conf_file or die $!;
    my $json = <$fh>;
    return decode_json $json;
}
