-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Generation Time: Mar 13, 2025 at 03:34 AM
-- Server version: 10.4.28-MariaDB
-- PHP Version: 8.2.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `pickmeup`
--

-- --------------------------------------------------------

--
-- Table structure for table `activeDrivers2`
--

CREATE TABLE `activeDrivers2` (
  `id` int(11) NOT NULL,
  `driver_id` int(11) NOT NULL,
  `login_time` datetime NOT NULL DEFAULT current_timestamp(),
  `status` enum('BOOKED','AVAILABLE') DEFAULT NULL,
  `trips` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `activeDrivers2`
--

INSERT INTO `activeDrivers2` (`id`, `driver_id`, `login_time`, `status`, `trips`) VALUES
(73, 12, '2025-01-11 13:31:02', 'AVAILABLE', 2),
(74, 13, '2025-01-11 13:53:56', 'AVAILABLE', 0),
(75, 13, '2025-01-11 13:53:58', 'AVAILABLE', 0);

-- --------------------------------------------------------

--
-- Table structure for table `admin`
--

CREATE TABLE `admin` (
  `id` int(11) NOT NULL,
  `adminName` varchar(100) NOT NULL,
  `password` varchar(100) NOT NULL,
  `last_access` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `admin`
--

INSERT INTO `admin` (`id`, `adminName`, `password`, `last_access`) VALUES
(1, 'Remy Dianne Ventura', '123', '2024-12-15 16:25:00');

-- --------------------------------------------------------

--
-- Table structure for table `adminSession`
--

CREATE TABLE `adminSession` (
  `id` int(11) NOT NULL,
  `adminName` varchar(100) NOT NULL,
  `loggedIn` datetime DEFAULT NULL,
  `loggedOut` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `adminSession`
--

INSERT INTO `adminSession` (`id`, `adminName`, `loggedIn`, `loggedOut`) VALUES
(37, 'Remy Dianne Ventura', '2025-01-09 23:12:38', '2025-01-10 16:27:51'),
(38, 'Remy Dianne Ventura', '2025-01-09 23:13:45', '2025-01-10 16:27:51'),
(39, 'Remy Dianne Ventura', '2025-01-10 00:05:03', '2025-01-10 16:27:51'),
(40, 'Remy Dianne Ventura', '2025-01-10 14:41:29', '2025-01-10 16:27:51'),
(41, 'Remy Dianne Ventura', '2025-01-10 16:19:45', '2025-01-10 16:27:51'),
(42, 'Remy Dianne Ventura', '2025-01-10 20:30:56', '2025-01-10 20:39:14'),
(43, 'Remy Dianne Ventura', '2025-01-10 21:06:08', '2025-01-11 13:21:01'),
(44, 'Remy Dianne Ventura', '2025-01-11 01:12:43', '2025-01-11 13:21:01'),
(45, 'Remy Dianne Ventura', '2025-01-11 01:29:36', '2025-01-11 13:21:01'),
(46, 'Remy Dianne Ventura', '2025-01-11 01:49:01', '2025-01-11 13:21:01'),
(47, 'Remy Dianne Ventura', '2025-01-11 05:59:26', '2025-01-11 13:21:01'),
(48, 'Remy Dianne Ventura', '2025-01-11 09:41:30', '2025-01-11 13:21:01'),
(49, 'Remy Dianne Ventura', '2025-01-11 13:23:34', NULL),
(50, 'Remy Dianne Ventura', '2025-01-20 04:35:56', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `bookingReport`
--

CREATE TABLE `bookingReport` (
  `id` int(11) NOT NULL,
  `driver_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `location` varchar(100) NOT NULL,
  `seats` int(11) NOT NULL,
  `booking_time` datetime NOT NULL DEFAULT current_timestamp(),
  `status` enum('COMPLETED','PENDING','ONGOING','CANCELLED') NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `bookingReport`
--

INSERT INTO `bookingReport` (`id`, `driver_id`, `user_id`, `location`, `seats`, `booking_time`, `status`) VALUES
(226, 12, 125, 'CA', 2, '2025-01-11 13:32:39', 'COMPLETED'),
(227, 12, 125, 'CBEA', 2, '2025-01-11 13:34:11', 'CANCELLED'),
(228, 12, 125, 'CBEA', 2, '2025-01-11 13:40:10', 'COMPLETED'),
(229, 12, 114, 'CBEA', 1, '2025-01-11 13:43:10', 'COMPLETED'),
(230, 12, 124, 'CICS', 4, '2025-01-11 13:45:34', 'CANCELLED'),
(231, 13, 125, 'CBEA', 3, '2025-01-11 13:54:57', 'CANCELLED');

-- --------------------------------------------------------

--
-- Table structure for table `drivers`
--

CREATE TABLE `drivers` (
  `id` int(11) NOT NULL,
  `card_no` varchar(100) NOT NULL,
  `lastName` varchar(100) NOT NULL,
  `firstName` varchar(100) NOT NULL,
  `plate_no` varchar(100) NOT NULL,
  `address` varchar(100) NOT NULL,
  `phone_no` varchar(100) NOT NULL,
  `dateAdded` datetime NOT NULL DEFAULT current_timestamp(),
  `dateLastUpdated` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `drivers`
--

INSERT INTO `drivers` (`id`, `card_no`, `lastName`, `firstName`, `plate_no`, `address`, `phone_no`, `dateAdded`, `dateLastUpdated`) VALUES
(12, '13b47ae4', 'Hidalgo', 'Roberto', '1320GHJ', 'Flourishing, Gonzaga', '09997521287', '2025-01-11 01:14:58', '2025-01-11 01:56:06'),
(13, 'f3fa9be4', 'Ladera', 'Ian ', '7HGD787', 'Smart, Gonzaga', '09352958720', '2025-01-11 06:17:55', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `newUserReq`
--

CREATE TABLE `newUserReq` (
  `id` int(11) NOT NULL,
  `lastName` varchar(100) NOT NULL,
  `firstName` varchar(100) NOT NULL,
  `email` varchar(100) NOT NULL,
  `phone_no` varchar(13) NOT NULL,
  `username` varchar(100) NOT NULL,
  `password` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `lastName` varchar(100) NOT NULL,
  `firstName` varchar(100) NOT NULL,
  `email` varchar(100) NOT NULL,
  `phone_no` varchar(13) NOT NULL,
  `username` varchar(100) NOT NULL,
  `password` varchar(100) NOT NULL,
  `dateAdded` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`id`, `lastName`, `firstName`, `email`, `phone_no`, `username`, `password`, `dateAdded`) VALUES
(114, 'Los Banos', 'Jasmin Kate', 'jasmin@gmail.com', '09565782150', 'iamjas', '$2y$10$ZTpkSN.kT9BwHb.A04NMmuBqGskMb4Bbo7z1iDH0zKfU58Uhk8h6C', '2025-01-07 20:11:40'),
(115, 'Padin', 'Romelyn', 'romelyn@gmail.com', '09565782150', 'iamprincess', '$2y$10$Sl04T9SBI4T6.tfxJjfjzeZFzS8Ox/mYHIDz8L4qP..1ZJ1sqlk6i', '2025-01-07 23:13:32'),
(116, 'Dutdut', 'Mary Grace', 'mary@gmail.com', '09565782150', 'maryyy', '$2y$10$j8y/VdR84jDuxhCKRNThs.ieLF7uW5ejtD8mdEhub0TOR0rSo3NSq', '2025-01-07 23:14:38'),
(124, 'Carrera', 'Althea', 'Althea@gmail.com', '09366123550', 'eya', '$2y$10$q7IMaqtG3wQHn5eHQXpn/.ZXGaUs6unVUU8q/Y7kmE0mPlIeT8rou', '2025-01-11 01:29:51'),
(125, 'Ventura', 'Remy Dianne ', 'remydventuraaa@gmail.com', '09352958720', 'remyyy', '$2y$10$UNXLKzsCt41VDScM0MvD3OIuR0yacoVzBI1R55kW8JsrOpZQfqz0G', '2025-01-11 13:28:24');

-- --------------------------------------------------------

--
-- Table structure for table `userSession`
--

CREATE TABLE `userSession` (
  `id` int(11) NOT NULL,
  `userName` varchar(100) NOT NULL,
  `loggedIn` datetime DEFAULT NULL,
  `loggedOut` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `userSession`
--

INSERT INTO `userSession` (`id`, `userName`, `loggedIn`, `loggedOut`) VALUES
(89, 'Ventura, Remy Dianne ', '2025-01-11 13:28:37', '2025-01-11 13:40:12'),
(90, 'Los Banos, Jasmin Kate', '2025-01-11 13:40:16', '2025-01-11 14:02:07'),
(91, 'Carrera, Althea', '2025-01-11 13:44:57', '2025-01-11 13:52:48'),
(92, 'Ventura, Remy Dianne ', '2025-01-11 13:53:27', NULL);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `activeDrivers2`
--
ALTER TABLE `activeDrivers2`
  ADD PRIMARY KEY (`id`),
  ADD KEY `driver_id` (`driver_id`);

--
-- Indexes for table `admin`
--
ALTER TABLE `admin`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `adminSession`
--
ALTER TABLE `adminSession`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `bookingReport`
--
ALTER TABLE `bookingReport`
  ADD PRIMARY KEY (`id`),
  ADD KEY `d_id` (`driver_id`),
  ADD KEY `u_id` (`user_id`);

--
-- Indexes for table `drivers`
--
ALTER TABLE `drivers`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `newUserReq`
--
ALTER TABLE `newUserReq`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `userSession`
--
ALTER TABLE `userSession`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `activeDrivers2`
--
ALTER TABLE `activeDrivers2`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=76;

--
-- AUTO_INCREMENT for table `admin`
--
ALTER TABLE `admin`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `adminSession`
--
ALTER TABLE `adminSession`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=51;

--
-- AUTO_INCREMENT for table `bookingReport`
--
ALTER TABLE `bookingReport`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=232;

--
-- AUTO_INCREMENT for table `drivers`
--
ALTER TABLE `drivers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT for table `newUserReq`
--
ALTER TABLE `newUserReq`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=189;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=126;

--
-- AUTO_INCREMENT for table `userSession`
--
ALTER TABLE `userSession`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=93;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `activeDrivers2`
--
ALTER TABLE `activeDrivers2`
  ADD CONSTRAINT `driver_id` FOREIGN KEY (`driver_id`) REFERENCES `drivers` (`id`) ON DELETE CASCADE;

--
-- Constraints for table `bookingReport`
--
ALTER TABLE `bookingReport`
  ADD CONSTRAINT `d_id` FOREIGN KEY (`driver_id`) REFERENCES `drivers` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `u_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
