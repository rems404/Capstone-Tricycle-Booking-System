<?php
	include 'connection.php';

	/**
	 * 
	 */
	class session
	{
		
		function adminLogin($name)
		{
			global $conn;
			$adminLogin = $conn->prepare("INSERT INTO adminSession (adminName, loggedIn) VALUES (?, NOW())");
			$adminLogin->bind_param("s", $name);
			$adminLogin->execute();
		}

		function adminLogout($name)
		{
			global $conn;
			$adminLogout = $conn->prepare("UPDATE adminSession SET loggedOut=NOW() WHERE adminName=? AND loggedOut IS NULL");
			$adminLogout->bind_param("s", $name);
			$adminLogout->execute();
		}

		function userLogin($name)
		{
			global $conn;
			$userLogin = $conn->prepare("INSERT INTO userSession (userName, loggedIn) VALUES (?, NOW())");
			$userLogin->bind_param("s", $name);
			$userLogin->execute();
		}

		function userLogout($name)
		{
			global $conn;
			$userLogout = $conn->prepare("UPDATE userSession SET loggedOut=NOW() WHERE userName=? AND loggedOut IS NULL");
			$userLogout->bind_param("s", $name);
			$userLogout->execute();
		}
	}
?>