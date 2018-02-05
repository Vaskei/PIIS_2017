<?php
	require __DIR__ . '/vendor/autoload.php';

	Predis\Autoloader::register();

	//$client = new Predis\Client();
	//$client->set('foo', 'bar');
	//$value = $client->get('test2');
	
	if(!empty($_GET['guid'])){
		$client = new Predis\Client();
		$value = $client->get($_GET['guid']);
		
		//echo $value;
		//echo $_GET['guid'];
		
		if($value != null){
			if($value == true){
				echo "Prolazi=> ".$value." i ".$_GET['guid']." su isti";
			}
			else{
				echo "Nije isto";
			}
		}
		else{
			echo "value je prazan";
		}
	}
?>