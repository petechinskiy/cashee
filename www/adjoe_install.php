<?php
include('config.inc.php');
include('functions.inc.php');

$tbl = "adjoe_installs";
$ip_whitelist = ['3.121.65.44','18.185.166.67','52.29.52.48'];

$device_id = !isset($_GET['external_user_id'])? "-1" : rawurldecode($_GET["external_user_id"]);
$app_name = !isset($_GET['app_name'])? "" : mysqli_real_escape_string($conn, rawurldecode($_GET["app_name"]));
$app_name = iconv_substr($app_name, 0, 64, 'UTF-8');
$cpi = !isset($_GET['cpi'])? 0.0 : rawurldecode($_GET["cpi"]);

$ip = GetClientIP();
$is_valid = in_array($ip, $ip_whitelist);

if (!$is_valid)
{
	echo "Not valid";
	//mysqli_query($conn, "UPDATE users SET access_restricted='3' WHERE user_id='$user_id' LIMIT 1");
	//echo "User is blocked!";
}
else
{
	$user_id = 0;
	$gps_adid = "";
	$ip = "";

	$sql = "SELECT user_id, gps_adid, ip, country, first_open_date, campaign FROM users WHERE device_id='$device_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$user_id = $r['user_id'];
			$gps_adid = $r['gps_adid'];
			$ip = $r['ip'];
			$country = $r['country'];
			$registration_date = $r['first_open_date'];
			$campaign = $r['campaign'];
		}

		mysqli_free_result($result);
	}

	if ($user_id == 0) {
		mysqli_close($conn);
		return;
	}

	$current_date = new DateTime();
	$current_date_str = $current_date->format('Y-m-d');

	if (mysqli_query($conn, "INSERT INTO $tbl (user_id, app_name, cpi, timestamp) VALUES ('$user_id', '$app_name', '$cpi', '$current_date_str')")) {
 		echo "Record added succesfully";

		 if (CanShowMissions($registration_date, $country, $campaign, UserWasRegisteredIP($conn, $device_id, $ip))) {
			CheckMissions($conn, $user_id, AdjoeForEarnButton($country, $registration_date));
		}
	}
}

mysqli_close($conn);
?>