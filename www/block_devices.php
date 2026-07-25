<?php
include('config.inc.php');
include('functions.inc.php');

$file = 'blocked.txt';
$handle = fopen($file, 'r');

if ($handle) {
    while (($line = fgets($handle)) !== false) {
        $device_id = preg_replace('/[^A-Za-z0-9 ]/', '', $line);
        $user_id = GetUserId($conn, $device_id);

        if ($user_id == 0) {
            echo $device_id."<br>";
        } else {
            mysqli_query($conn, "UPDATE users SET access_restricted='1' WHERE user_id='$user_id' LIMIT 1");
        }
    }

    fclose($handle);
} else {
    echo 'Не удалось открыть файл';
}
?>