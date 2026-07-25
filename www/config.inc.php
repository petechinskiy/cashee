<?php
$db = 'cashee';
$conn = mysqli_connect('pp1264167-007.eu.clouddb.ovh.net', 'cashee', 'sJNm4kZN8oo9UwVi', $db, '35228');
//mysqli_query($conn, "SET NAMES 'utf8'"); 
//mysqli_query($conn, "SET CHARACTER SET 'utf8'");
//mysqli_query($conn, "SET SESSION collation_connection = 'utf8_general_ci'");
mysqli_report(MYSQLI_REPORT_ERROR | MYSQLI_REPORT_STRICT);

if (!function_exists('str_contains')) {
    function str_contains(string $haystack, string $needle): bool {
        return '' === $needle || false !== strpos($haystack, $needle);
    }
}
?>