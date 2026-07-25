<?php
$code = !isset($_GET['ref_code'])? "" : rawurldecode($_GET["ref_code"]);

$url = "https://play.google.com/store/apps/details?id=com.plusgames.cashee&referrer=utm_term%3Drefcode".$code;

header('Location: '.$url);
?>