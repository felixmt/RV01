<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
	<link rel="stylesheet" href="./css/style.css" type="text/css" media="screen,handheld,print"/>
	<title>RV01 - Clairière de l'armistice</title>
</head>
<body>
<h1>Clairière de l'armistice</h1>
<h2>Elément courant : </h2>
<div id="container">
</div>
<div id="map">
	<img src="./img/photo_clairiere.jpg" alt="photo clairière"/>
</div>
</body>
</html>

<script>

function Timer() {
       $.get( "ajax.php", function( data ) {
			$( "#container" ).html( data );
		});
       setTimeout("Timer()",1000);
   }
   Timer();
</script>