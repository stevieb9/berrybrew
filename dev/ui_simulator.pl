use warnings;
use strict;

use FindBin qw($RealBin);
use lib "$RealBin/../lib";

use BuildHelper qw(:all);
use Data::Dumper;
use Tkx;


# NOTE: If a UTF error occurs reading the JSON, open the conf
# file up in vi and execute: ':set nobomb' (remove BOM)

# NOTE: All buttons have a command that will destroy the existing
# window and recreate it from scratch. This allows us to auto-refresh
# the UI live time while making changes to the UI config file

my $ui_conf_file = 'dev/data/ui.json';
my $data = BuildHelper::config_read($ui_conf_file);
my $font_size = $data->{ui_simulator}{font_size};

my $mw;

window_create_and_display();

sub buttons {
    my $button_conf = $data->{button};

    my @buttons;

    for (keys %$button_conf) {
        push @buttons, $button_conf->{$_};
    }

    return @buttons;
}
sub checkboxes {
    my $checkboxes_conf = $data->{checkbox};

    my @checkboxes;

    for (keys %$checkboxes_conf) {
        push @checkboxes, $checkboxes_conf->{$_};
    }

    return @checkboxes;
}
sub comboboxes {
    my $combobox_conf = $data->{combobox};

    my @comboboxes;

    for (keys %$combobox_conf) {
        push @comboboxes, $combobox_conf->{$_};
    }

    return @comboboxes;
}
sub labels {
    my $label_conf = $data->{label};

    my @labels;

    for (keys %$label_conf) {
        push @labels, $label_conf->{$_};
    }

    return @labels;
}
sub window_create_and_display {
    $data = BuildHelper::config_read($ui_conf_file);
    $font_size = $data->{ui_simulator}{font_size};

    $mw = Tkx::widget->new(".");

    # Window
    $mw->g_wm_title("BB UI Simulator");
    $mw->g_wm_minsize(_window_size());

    # Buttons
    _generate_buttons($mw);

    # Checkboxes
    _generate_checkboxes($mw);

    # Comboboxex
    _generate_comboboxes($mw);

    # Labels
    _generate_labels($mw);

    # Deploy
    Tkx::MainLoop();
}

sub _generate_buttons {
    for my $button_conf (buttons()) {
        my $button = $mw->new_button(
            -text    => $button_conf->{text},
            -font    => [ -size => $font_size ],
            -command => sub {
                $mw->DESTROY;
                window_create_and_display();
            }
        );
        $button->g_place(
            -width  => $button_conf->{size}[0],
            -height => $button_conf->{size}[1],
            -x      => $button_conf->{location}[0],
            -y      => $button_conf->{location}[1]
        );
    }
}
sub _generate_checkboxes {
    for my $checkbox_conf (checkboxes()) {
        my $checkbox = $mw->new_checkbutton(
            -text   => $checkbox_conf->{text},
            -font   => [ -size => $font_size ],
            -anchor => 'w'
        );
        $checkbox->g_place(
            -width      => $checkbox_conf->{width},
            -x          => $checkbox_conf->{location}[0],
            -y          => $checkbox_conf->{location}[1]
        );
    }
}
sub _generate_comboboxes {
    for my $combobox_conf (comboboxes()) {
        my $combobox = $mw->new_ttk__combobox(
            -values  => [ $combobox_conf->{name} ],
        );
        $combobox->g_place(
            -width  => $combobox_conf->{size}[0],
            -height => $combobox_conf->{size}[1],
            -x      => $combobox_conf->{location}[0],
            -y      => $combobox_conf->{location}[1]
        );
    }
}
sub _generate_labels {
    for my $label_conf (labels()) {
        my $label = $mw->new_label(
            -text => $label_conf->{text},
            -font => [
                -size   => $font_size,
            ]
        );
        $label->g_place(
            -width  => $label_conf->{size}[0],
            -height => $label_conf->{size}[1],
            -x      => $label_conf->{location}[0],
            -y      => $label_conf->{location}[1]
        );
    }
}
sub _window_size {
    return @{ $data->{ui_object}{client_size} };
}