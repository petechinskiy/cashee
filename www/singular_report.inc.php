<?php
$remove_rows_per_time = 20;

$app_name = 'com.plusgames.cashee';
$tbl = "singular_reports";
$sql = "SELECT * FROM $tbl LIMIT 1";

if ($result = mysqli_query($conn, $sql)) {
	$count = mysqli_num_rows($result);
	$rows_total = 0;
	$rows_sended_count = 0;
	$is_locked = false;

	// если еще нет ни одного отчета
	$file = GetSingularReport();
	$array = array_map("str_getcsv", explode("\n", $file));
	$filtered = array_filter($array, function($x) { return !empty($x); });
	$headers = array_shift($filtered); // первая строка = заголовки

	$nameIndex = array_search('app_longname', $headers);

	$filtered = array_filter($filtered, function($row) use ($app_name, $nameIndex) {
		return ($row[$nameIndex] ?? '') === $app_name;
	});

	$filtered = array_values($filtered); // сбросить ключи
	$rows_total = count($filtered);
		
	while($r=mysqli_fetch_array($result)) {
		$rows_total_prev = (int)$r['rows_total'];
		//$is_locked = $r['is_locked'] == 1;
		$is_cleared = $rows_total < $rows_total_prev; // данные в таблице были очищены
		$rows_sended_count = $is_cleared ? 0 : (int)$r['rows_sended'];
	}

	if ($is_locked) {
		mysqli_free_result($result);
		return;
	}
	
	$rows_remaining = $rows_total - $rows_sended_count;
	$remove_lines_count = min($rows_remaining, $remove_rows_per_time);
	$end_lines_num = $rows_sended_count + $remove_lines_count;

	// блокируем доступ к отчету пока идет обработка
	//mysqli_query($conn, "UPDATE $tbl SET is_locked='1' LIMIT 1");

	for (; $rows_sended_count < $end_lines_num; $rows_sended_count++) {
		$row = $filtered[$rows_sended_count];
		$rowData = array_combine($headers, $row);
    
		$status = $rowData['fraud_status'] ?? '';
		$gps_adid = $rowData['device_id'] ?? '';
		$fraud_reason = $rowData['fraud_reason'] ?? '';
		$is_fraud = $status == 'rejected';

		$user_id = GetUserIdByAdid($conn, $gps_adid);

		if ($is_fraud) {
			mysqli_query($conn, "INSERT INTO singular_antifraud (`user_id`, `gps_adid`, `fraud_status`, `fraud_reason`) VALUES ('$user_id', '$gps_adid', '$status', '$fraud_reason')");

			if ($user_id != 0) {
				mysqli_query($conn, "UPDATE users SET country_switch=1 WHERE user_id='$user_id'");
			}
		}
	}
	
	mysqli_query($conn, "UPDATE $tbl SET rows_sended='$rows_sended_count', rows_total='$rows_total', is_locked='0' LIMIT 1");

	mysqli_free_result($result);

	//echo 'Rows total: '.$rows_total.', rows sended: '.$rows_sended_count;
} else {
	//echo "Failed.";
}
?>