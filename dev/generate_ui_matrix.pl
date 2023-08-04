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

sub labels {
    my $label_conf = $data->{label};

    my @labels;

    for (keys %$label_conf) {
        push @labels, $label_conf->{$_};
    }

    return @labels;
}
sub buttons {
    my $button_conf = $data->{button};

    my @buttons;

    for (keys %$button_conf) {
        push @buttons, $button_conf->{$_};
    }

    return @buttons;
}
sub comboboxes {
    my $combobox_conf = $data->{combobox};

    my @comboboxes;

    for (keys %$combobox_conf) {
        push @comboboxes, $combobox_conf->{$_};
    }

    return @comboboxes;
}
sub window_size {
    return @{ $data->{ui_object}{client_size} };
}
sub window_display {
    my $mw = Tkx::widget->new(".");

    # Window

    $mw->g_wm_title("BB UI Simulator");
    $mw->g_wm_minsize(window_size());

    # Labels

    for my $label_conf (labels()) {
        my $label = $mw->new_label(-text => $label_conf->{text});
        $label->g_place(
            -width  => $label_conf->{size}[0],
            -height => $label_conf->{size}[1],
            -x      => $label_conf->{location}[0],
            -y      => $label_conf->{location}[1]
        );
    }

    # Button

    for my $button_conf (buttons()) {
        my $button = $mw->new_button(-text => $button_conf->{text});
        $button->g_place(
            -width  => $button_conf->{size}[0],
            -height => $button_conf->{size}[1],
            -x      => $button_conf->{location}[0],
            -y      => $button_conf->{location}[1]
        );
    }

    # Combobox

    for my $combobox_conf (comboboxes()) {
        my $combobox = $mw->new_ttk__combobox(-values => [$combobox_conf->{name}]);
        $combobox->g_place(
            -width  => $combobox_conf->{size}[0],
            -height => $combobox_conf->{size}[1],
            -x      => $combobox_conf->{location}[0],
            -y      => $combobox_conf->{location}[1]
        );
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
