<?php
	$server = "localhost";
	$user = "root";
	$psw = "";
	$database = "pickmeup";

	$conn = new mysqli($server, $user, $psw, $database);

	if ($conn->connect_error) {
		die("Connection failed! " . $conn->connect_error);
	}
?>