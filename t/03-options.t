use warnings;
use strict;

use Data::Dumper;
use Test::More;
use Win32::TieRegistry;

$ENV{BERRYBREW_ENV} = "test";

my $c = $ENV{BBTEST_REPO} ? "$ENV{BBTEST_REPO}/test/berrybrew" : 'c:/repos/berrybrew/test/berrybrew';

system("assoc", ".pl=PerlScript");

my $o;

# issue #237
my $pl_assoc_key = "HKEY_CLASSES_ROOT\\.pl\\";
my $bb_key = "HKEY_LOCAL_MACHINE\\SOFTWARE\\berrybrew-test\\";

delete $Registry->{$bb_key};
# delete $Registry->{$pl_assoc_key};

is $Registry->{$bb_key}, undef, "confirmed berrybrew regkey deleted";
# is $Registry->{$pl_assoc_key}, undef, "confirmed .pl assoc regkey deleted";

like `$c`, qr/view subcommand/, "issue #237 fixed ok";

$Registry->{$pl_assoc_key}{''} = 'PerlScript';
is $Registry->{$pl_assoc_key}{''}, 'PerlScript', "re-added .pl assoc regkey ok";
# end issue #237

system("assoc", ".pl=PerlScript");

$o = `$c options`;

like $o, qr/debug:\s+false/, "debug ok";
like $o, qr/root_dir:\s+C:\\berrybrew\\test/, "root_dir ok";
like $o, qr/temp_dir:\s+C:\\berrybrew\\test\\temp/, "temp_dir ok";
like $o, qr|download_url:\s+https://strawberryperl.com/releases.json|, "download_url ok";
like $o, qr/windows_homedir:\s+false/, "windows_homedir ok";
like $o, qr/custom_exec:\s+false/, "custom_exec ok";
like $o, qr/run_mode:\s+test/, "run_mode ok";
like $o, qr/file_assoc:\s+/, "file_assoc ok";
like $o, qr/file_assoc_old:\s+/, "file_assoc_old ok";

like `$c options debug`, qr/^\s+debug:\s+false\s+$/, "single debug ok";
like `$c options root_dir`, qr/^\s+root_dir:\s+C:\\berrybrew\\test\s+$/, "single root_dir ok";
like `$c options temp_dir`, qr/^\s+temp_dir:\s+C:\\berrybrew\\test\\temp\s+$/, "single temp_dir ok";
like `$c options download_url`, qr|^\s+download_url:\s+https://strawberryperl.com/releases.json\s+$|, "single download_url ok";
like `$c options windows_homedir`, qr/^\s+windows_homedir:\s+false\s+$/, "single windows_homedir ok";
like `$c options custom_exec`, qr/^\s+custom_exec:\s+false\s+$/, "single custom_exec ok";
like `$c options run_mode`, qr/^\s+run_mode:\s+test\s+$/, "single run_mode ok";
like `$c options file_assoc`, qr/^\s+file_assoc:\s+PerlScript$/, "single file_assoc ok";
like `$c options file_assoc_old`, qr/^\s+file_assoc_old:\s+$/, "single file_assoc_old ok";

like `$c options run_mode option_test`, qr/^\s+run_mode:\s+option_test\s+$/, "changing run_dir opt ok";
like `$c options run_mode test`, qr/^\s+run_mode:\s+test\s+$/, "changing run_dir back ok";

done_testing();
