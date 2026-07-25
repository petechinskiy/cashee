<?php
include('../config.inc.php');

// https://plustesttrack.com/ironsource/index_new.php?android_id={android_id}&campaign_id={campaign_id}&creative_id={creative_id}

$gps_adid = !isset($_GET['android_id'])? "" : rawurldecode($_GET["android_id"]);
$campaign_id = !isset($_GET['campaign_tier1_id'])? "" : rawurldecode($_GET["campaign_tier1_id"]);
$creative_id = !isset($_GET['creative_id'])? "" : rawurldecode($_GET["creative_id"]);
$publisher_id = !isset($_GET['site_id'])? "" : rawurldecode($_GET["site_id"]);
$network_name = !isset($_GET['network_name'])? "" : rawurldecode($_GET["network_name"]);

if ($creative_id != "" && $campaign_id != "" && $publisher_id != "" && $network_name != "") {
	mysqli_query($conn, "INSERT INTO ironsource_installs (gps_adid, campaign_id, creative_id, publisher_id, network_name) VALUES ('$gps_adid', '$campaign_id', '$creative_id', '$publisher_id', '$network_name')");
}

$url = "https://play.google.com/store/apps/details?id=com.plusgames.cashup&referrer=utm_campaign%3D".$campaign_id."%26utm_content%3D".$publisher_id."%26utm_term%3D".$creative_id;

header('Location: '.$url);
?>