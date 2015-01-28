-- phpMyAdmin SQL Dump
-- version 4.1.14
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1
-- Generation Time: Jan 08, 2015 at 12:31 PM
-- Server version: 5.6.17
-- PHP Version: 5.5.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `unity_rv01`
--

-- --------------------------------------------------------

--
-- Table structure for table `marker`
--

CREATE TABLE IF NOT EXISTS `marker` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(32) NOT NULL,
  `posX` float NOT NULL,
  `posY` float NOT NULL,
  `posZ` float NOT NULL,
  `scenario_id` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=13 ;

--
-- Dumping data for table `marker`
--

INSERT INTO `marker` (`id`, `name`, `posX`, `posY`, `posZ`, `scenario_id`) VALUES
(1, 'Wagon du maréchal Foch', 250, 121, 260, 0),
(2, 'Wagon des forces allemandes', 260, 121, 240, 0),
(3, 'Canon des forces alliées', 240, 121, 250, 0),
(4, 'Canon des forces allemandes', 260, 121, 250, 0),
(5, 'Délégation alliée', 248, 121, 255, 0),
(6, 'Délégation allemande', 252, 121, 255, 0),
(7, 'Wagon du maréchal Foch', 0, 0.7, 0.41, 1),
(8, 'Délégation alliée', -0.09, 0.7, 0.225, 1),
(9, 'Délégation allemande', 0.09, 0.7, 0.225, 1),
(10, 'Canon des forces allemandes', 0.465, 0.7, 0, 1),
(11, 'Canon des forces alliées', -0.465, 0.7, 0, 1),
(12, 'Wagon des forces allemandes', 0.409, 0.7, -0.436, 1);

-- --------------------------------------------------------

--
-- Table structure for table `object`
--

CREATE TABLE IF NOT EXISTS `object` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(32) CHARACTER SET utf8 NOT NULL,
  `description` text,
  `object_type_id` int(11) NOT NULL,
  `scenario_id` int(11) NOT NULL,
  `associated_marker_id` int(11) NOT NULL,
  `current_marker_id` int(11) DEFAULT NULL,
  `posX` float NOT NULL,
  `posY` float NOT NULL,
  `posZ` float NOT NULL,
  `isCurrent` tinyint(1) NOT NULL DEFAULT '0',
  `order` int(11) NOT NULL,
  `initRot` int(11) NOT NULL,
  `finalRot` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `associated_marker_id` (`associated_marker_id`),
  KEY `current_marker_id` (`current_marker_id`),
  KEY `object_type_id` (`object_type_id`),
  KEY `scenario_id` (`scenario_id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=21 ;

--
-- Dumping data for table `object`
--

INSERT INTO `object` (`id`, `name`, `description`, `object_type_id`, `scenario_id`, `associated_marker_id`, `current_marker_id`, `posX`, `posY`, `posZ`, `isCurrent`, `order`, `initRot`, `finalRot`) VALUES
(1, 'Wagon du maréchal Foch', 'Le wagon allié suivait l''armée alliée et a fait son entrée dans la clairière.\r\nIl fera figure de centre névralgique de cette cérémonie de signature de l''armistice.\r\nC’est dans ce wagon 2419D, fabriqué par la compagnie internationale des wagons lits, celui du Maréchal Foch, que se dérouleront les négociations de l’armistice.\r\nPositionnez le maintenant au bon emplacement.', 1, 2, 1, 7, 250, 8, 240, 0, 2, 60, 100),
(2, 'Wagon des forces allemandes', 'Le wagon allemand est arrivé à la suite des forces allemandes, il viendra se poster au sud ouest de la clairière, mettez le en place.', 1, 2, 2, 12, 250, 8, 240, 0, 4, 60, 20),
(3, 'Canon des forces alliées', 'Aujourd''hui, 11 novembre 1918, va être signé l''armistice dans la clairière de Compiègne. La maréchal Foch a volontairement fait le choix de ce lieu calme et isolé, témoignant ainsi de son respect pour l’adversaire vaincu.\r\nLe canon qui symbolise les forces alliées est arrivé à l''entrée de la clairière, il viendra se poster au centre ouest, veuillez le positionner.', 2, 2, 3, 11, 250, 8, 240, 0, 1, 150, 230),
(12, 'Canon des forces allemandes', 'Désormais les forces allemandes sont arrivées, et viendront se placer au centre est, veuillez positionner le canon qui les symbolise.', 2, 2, 4, 10, 250, 8, 240, 0, 3, 210, 130),
(13, 'Délégation alliée', 'Le maréchal Foch entouré de l''amiral de la flotte britannique Wemyss, du contre-amiral britannique Hope et du général français Weygand, constituent la délégation alliée, et sont sortis de leur wagon, ils doivent maintenant aller attendre la délégation allemande au point de rendez-vous, pour la ratification de l''armistice.', 3, 2, 5, 8, 252, 5, 258, 0, 5, 0, 0),
(14, 'Délégation allemande', 'La délégation allemande, constituée du ministre d''État Matthias Erzberger, du général major von Winterfeldt de l''armée impériale, du comte Alfred von Oberndorff des Affaires étrangères et du capitaine de vaisseau Vanselow de la Marine impériale, est descendue du wagon allemand, faîtes lui rejoindre la délégation alliée qui l''attend près du wagon du maréchal Foch.', 3, 2, 6, 9, 256, 5, 240, 0, 6, 0, 0),
(15, 'Canon des forces alliées', 'Aujourd''hui, 11 novembre 1918, va être signé l''armistice dans la clairière de Compiègne. La maréchal Foch a volontairement fait le choix de ce lieu calme et isolé, témoignant ainsi de son respect pour l’adversaire vaincu.\r\nLe canon qui symbolise les forces alliées est arrivé à l''entrée de la clairière, il viendra se poster au centre ouest, veuillez le positionner.', 2, 1, 3, 11, 0, 0.77, -0.48, 1, 1, 150, 230),
(16, 'Délégation allemande', 'La délégation allemande, constituée du ministre d''État Matthias Erzberger, du général major von Winterfeldt de l''armée impériale, du comte Alfred von Oberndorff des Affaires étrangères et du capitaine de vaisseau Vanselow de la Marine impériale, est descendue du wagon allemand, faîtes lui rejoindre la délégation alliée qui l''attend près du wagon du maréchal Foch.', 3, 1, 6, 9, 0.286, 0.77, -0.288, 1, 6, 0, 0),
(17, 'Wagon du maréchal Foch', 'Le wagon allié suivait l''armée alliée et a fait son entrée dans la clairière.\r\nIl fera figure de centre névralgique de cette cérémonie de signature de l''armistice.\r\nC’est dans ce wagon 2419D, fabriqué par la compagnie internationale des wagons lits, celui du Maréchal Foch, que se dérouleront les négociations de l’armistice.\r\nPositionnez le maintenant au bon emplacement.', 1, 1, 1, 7, 0, 0.77, -0.48, 1, 2, 60, 100),
(18, 'Wagon des forces allemandes', 'Le wagon allemand est arrivé à la suite des forces allemandes, il viendra se poster au sud ouest de la clairière, mettez le en place.', 1, 1, 2, 12, 0, 0.77, -0.48, 1, 4, 60, 20),
(19, 'Délégation alliée', 'Le maréchal Foch entouré de l''amiral de la flotte britannique Wemyss, du contre-amiral britannique Hope et du général français Weygand, constituent la délégation alliée, et sont sortis de leur wagon, ils doivent maintenant aller attendre la délégation allemande au point de rendez-vous, pour la ratification de l''armistice.', 3, 1, 5, 8, -0.25, 0.7, 0.29, 1, 5, 0, 0),
(20, 'Canon des forces allemandes', 'Désormais les forces allemandes sont arrivées, et viendront se placer au centre est, veuillez positionner le canon qui les symbolise.', 2, 1, 4, 10, 0, 0.77, -0.48, 1, 3, 210, 130);

-- --------------------------------------------------------

--
-- Table structure for table `object_type`
--

CREATE TABLE IF NOT EXISTS `object_type` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=4 ;

--
-- Dumping data for table `object_type`
--

INSERT INTO `object_type` (`id`, `name`) VALUES
(1, 'wagon'),
(2, 'gun'),
(3, 'person');

-- --------------------------------------------------------

--
-- Table structure for table `scenario`
--

CREATE TABLE IF NOT EXISTS `scenario` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(33) CHARACTER SET utf8 NOT NULL,
  `description` text CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=3 ;

--
-- Dumping data for table `scenario`
--

INSERT INTO `scenario` (`id`, `name`, `description`) VALUES
(1, 'Armistice 1918', 'Description armistice 1918'),
(2, 'Armistice 1918 (No Oculus)', '');

--
-- Constraints for dumped tables
--

--
-- Constraints for table `object`
--
ALTER TABLE `object`
  ADD CONSTRAINT `FK_ASSM_OBJ` FOREIGN KEY (`associated_marker_id`) REFERENCES `marker` (`id`),
  ADD CONSTRAINT `FK_CURRM_OBJ` FOREIGN KEY (`current_marker_id`) REFERENCES `marker` (`id`),
  ADD CONSTRAINT `FK_OBJT_OBJ` FOREIGN KEY (`object_type_id`) REFERENCES `object_type` (`id`),
  ADD CONSTRAINT `FK_SCE_OBJ` FOREIGN KEY (`scenario_id`) REFERENCES `scenario` (`id`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
