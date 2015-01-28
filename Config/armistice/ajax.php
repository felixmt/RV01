<?php
	$hote="localhost;";
	$login="root";
	$mdp="";
	$bd="unity_rv01";
	$pdo_options[PDO::ATTR_ERRMODE] = PDO::ERRMODE_EXCEPTION;
    $bdd = new PDO('mysql:host=localhost;dbname=unity_rv01', 'root', '', $pdo_options);
	$sql = "select * from object where isCurrent = 1";
	$res = $bdd->query ($sql);
	$i = 0;
	foreach ($res as $row) {
		$i++;
		echo $row['name'];
		echo "<h2>Indices pour disposer l'élément :</h2>" . $row['description'];
	}
	if ($i == 0) { echo "aucun élément n'est en cours de manipulation par votre partenaire"; }
?>