<?php
$remove_rows_per_time = 20;

$current_date = new DateTime();
$target_date = new DateTime();
$target_date->setTime(15,30);

$date = new DateTime();
$date->setTimezone(new DateTimeZone('GMT'));
$date->modify("-1 days");
$date->setTime(23,59,59);
$date_str = $date->format('Y-m-d');
$date_str_full = $date->format('Y-m-d H:i:s');

$tbl = "adjoe_reports";
$sql = "SELECT * FROM $tbl WHERE date='$date_str' LIMIT 1";

if ($current_date < $target_date) {
	echo "Not time for report";
	return;
}

if ($result = mysqli_query($conn, $sql)) {
	$count = mysqli_num_rows($result);
	$rows_total = 0;
	$rows_sended_count = 0;
	$is_locked = false;
	$pathFile = 'adjoe_reports/report_'.$date_str.'.csv';
	$log_path = 'adjoe_reports/log_'.$date_str.'.csv';

	// если отчет еще не сохранен, то делаем это
	if ($count == 0) {
		$file = GetAdjoeReport($date_str);
		file_put_contents($pathFile, $file);
		
		$array = array_map("str_getcsv", explode("\n", $file));
		$filtered = array_filter($array, function($x) { return !empty($x); });
		$rows_total = count($filtered);
		
		if ($rows_total > 0) {
			mysqli_query($conn, "INSERT INTO $tbl (`date`, `rows_total`, `url`) VALUES ('$date_str', '$rows_total', '-')");
		}
	} else {
		while($r=mysqli_fetch_array($result)) {
			$rows_total = (int)$r['rows_total'];
			$rows_sended_count = (int)$r['rows_sended'];
			$is_locked = $r['is_locked'] == 1;
		}

		if($rows_sended_count >= $rows_total) {
			if (file_exists($pathFile)) {
				unlink($pathFile);
			}

			return;
		}

		if ($is_locked) {
			mysqli_free_result($result);
			mysqli_close($conn);

			//echo 'All rows sended. Report completed. Date: '.$date_str.'. Rows total: '.$rows_total.', rows sended: '.$rows_sended_count;

			return;
		}
	}

	$csv = file($pathFile, FILE_SKIP_EMPTY_LINES);
	$rows_remaining = count($csv);
	$remove_lines_count = min($rows_remaining, $remove_rows_per_time);

	// блокируем доступ к отчету пока идет обработка
	mysqli_query($conn, "UPDATE $tbl SET is_locked='1' WHERE date='$date_str'");

	for ($i = 0; $i < $remove_lines_count; $i++) {
		$line = $csv[$i];
		$row = str_getcsv($line);

		$device_id = $row[6];
		$revenue = (float)$row[12];
		$adid = $row[5];
		$country = $row[3];
		$ecpm = $row[11];
		$network = $row[7];
		$channel = $row[8];
		$adjoe_clicks = $row[13];
		$reward = $revenue * 3000;
		$user_id = GetUserIdByAdid($conn, $adid);

		if ($user_id == 0) {
			$user_id = GetUserId($conn, $device_id);
		}

		if ($reward > 0) {
			mysqli_query($conn, "INSERT INTO income_adjoe_report (`date`, `reward`, `user_id`, `gps_adid`, `country`, `uanetwork`, `uachannel`, `ecpm`, `user_value`, `adjoe_clicks`) VALUES ('$date_str', '$reward', '$user_id', '$adid', '$country', '$network', '$channel', '$ecpm', '$revenue', '$adjoe_clicks')");
		}
	}

	$rows_sended_count += $remove_lines_count;

	$output = array_slice($csv, $remove_lines_count);
	file_put_contents($pathFile, $output);
	
	mysqli_query($conn, "UPDATE $tbl SET rows_sended='$rows_sended_count', is_locked='0' WHERE date='$date_str'");

	mysqli_free_result($result);

	//echo 'Success. Report date: '.$date_str.'. Rows total: '.$rows_total.', rows sended: '.$rows_sended_count;
} else {
	//echo "Failed.";
}


mysqli_close($conn);
?>