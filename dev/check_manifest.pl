#!/usr/bin/env perl
use warnings;
use strict;

use FindBin qw($RealBin);
use lib "$RealBin/../lib";

use BuildHelper qw(:all);

use constant {
    INSTALLER_SCRIPT_PROD       => 'dev/create_prod_installer.nsi',
    INSTALLER_SCRIPT_STAGING    => 'dev/create_staging_installer.nsi',
};

print "\nPRODUCTION:\n\n";

check_installer_manifest(INSTALLER_SCRIPT_PROD);

print "\nSTAGING:\n\n";

check_installer_manifest(INSTALLER_SCRIPT_STAGING);
