<?php
include('config.inc.php');
include('functions.inc.php');

$app_version = rawurldecode($_GET["app_version"]);

ProcessWithdrawsStatus($conn, $app_version);

mysqli_close($conn);
?>