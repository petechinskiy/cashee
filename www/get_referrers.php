<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);

$user_id = GetUserId($conn, $device_id);

if ($user_id == 0) {
	return;
}

$first_level_members = GetReferrers($conn, $user_id);
$first_level_members_count = count($first_level_members);
$second_level_members_count = 0;

foreach ($first_level_members as $r_id) {
    $sec_members = GetReferrers($conn, $r_id);
	$second_level_members_count += count($sec_members);
}

$first_level_coins = 0;
$second_level_coins = 0;

$sql = "SELECT referrer_level, from_user_id, coins FROM referrer_callbacks WHERE user_id='$user_id'";

if ($result = mysqli_query($conn, $sql)) {
	while($r = mysqli_fetch_array($result)) {
		$referrer_lvl = $r['referrer_level'];
		$from_user_id = $r['from_user_id'];
		$coins = $r['coins'];

		if ($referrer_lvl == 1) { 
			$first_level_coins += $coins;
		} else {
			$second_level_coins += $coins;
		}
	}
	mysqli_free_result($result);
}

$rows = array('FirstLevelMembers' => $first_level_members_count, 'FirstLevelCoins' => $first_level_coins, 'SecondLevelMembers' => $second_level_members_count, 'SecondLevelCoins' => $second_level_coins);

echo json_encode($rows, JSON_PRETTY_PRINT);

mysqli_close($conn);
?>