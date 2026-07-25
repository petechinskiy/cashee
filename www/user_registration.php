<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);
$is_male_str = !isset($_GET['is_male'])? "true" : strtolower(rawurldecode($_GET["is_male"]));
$age = !isset($_GET['age'])? 0 : rawurldecode($_GET["age"]);
$ip = GetClientIP();
$gps_adid = !isset($_GET['gps_adid'])? "" : rawurldecode($_GET["gps_adid"]);
$campaign = !isset($_GET['utm_campaign'])? "" : rawurldecode($_GET["utm_campaign"]);
$publisher_id = !isset($_GET['utm_content'])? "" : rawurldecode($_GET["utm_content"]);
$creative = !isset($_GET['utm_term'])? "" : rawurldecode($_GET["utm_term"]);
$source = !isset($_GET['utm_source'])? "" : rawurldecode($_GET["utm_source"]);
$referrer_code = !isset($_GET['referrer_code'])? "" : strtoupper(rawurldecode($_GET["referrer_code"]));
$network_name = !isset($_GET['network_name'])? "" : rawurldecode($_GET["network_name"]);
$campaign_name = !isset($_GET['campaign_name'])? "" : rawurldecode($_GET["campaign_name"]);
$install_site = !isset($_GET['install_site'])? "" : rawurldecode($_GET["install_site"]);
$creative_name = !isset($_GET['creative_name'])? "" : rawurldecode($_GET["creative_name"]);

$status = 0;

if ($device_id != "" && CheckClient()) {
	$age = (int)$age;
	$is_male = $is_male_str == "true" ? 1 : 0;
	$is_organic = 1;
	$country_code = "";
	$creative = str_contains($creative, "refcode") ? "" : $creative; // в этот параметр можем вставлять реферральный код, поэтому не записываем такой креатив

	$user_id = GetUserId($conn, $device_id);

	if ($user_id == 0) {
		$country_code = GetCountryCodeByIp($ip);
		$referrer_user_id = GetUserIdByReferrerCode($conn, $referrer_code);
		$vpn_usage = 0;
		$country_switch = 0;

		if ($gps_adid != "" && $gps_adid != '{android_id}' && !UserWasRegistered($conn, $device_id)) {
			$campaign = $campaign_name;
			$publisher_id = $install_site;
			$creative = $creative_name;
			$vpn_usage = IsVPNUsage($ip) ? 1 : 0;
			$country_switch = IsCountrySwitch($country_code) ? 1 : 0;

			if (str_contains($network_name, "iron") || str_contains($network_name, "unity") || str_contains($network_name, "google") || str_contains($campaign, "google")) {
				$is_organic = 0;

				if (!InstallIsExists($conn, $gps_adid)) {
					mysqli_query($conn, "INSERT INTO ironsource_installs (gps_adid, campaign_id, creative_id, publisher_id, network_name) VALUES ('$gps_adid', '$campaign', '$creative', '$publisher_id', '$network_name')");
				} else {
					$sql_install = "SELECT campaign_id, creative_id, publisher_id FROM ironsource_installs WHERE gps_adid='$gps_adid'";

					if ($res_install = mysqli_query($conn, $sql_install)) {
						while ($r_install=mysqli_fetch_array($res_install)) {
							$campaign = $r_install['campaign_id'];
							$creative = $r_install['creative_id'];
							$publisher_id = $r_install['publisher_id'];
						}
						mysqli_free_result($res_install);
					} 
				}

				if ($campaign == "") {
					$campaign = $network_name;
				}
			}
		}

		mysqli_query($conn, "INSERT INTO users (device_id, ip, country, referrer_user_id, is_organic, gps_adid, campaign, publisher_id, creative, vpn_usage, country_switch) VALUES ('$device_id', '$ip', '$country_code', '$referrer_user_id', '$is_organic', '$gps_adid', '$campaign', '$publisher_id', '$creative', '$vpn_usage', '$country_switch')");
	}

	$status = 1;

	mysqli_close($conn);

	echo $status;
}
?>