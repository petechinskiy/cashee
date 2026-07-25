<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);

mysqli_query($conn, "UPDATE users SET click_earn_button='1' WHERE device_id='$device_id' LIMIT 1");
mysqli_close($conn);
?>