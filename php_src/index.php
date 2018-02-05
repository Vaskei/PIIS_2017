<?php

/*
*	Primjer PHP skripte za autentikaciju korisnika uporabom programskog alata simpleSAMLphp
*	Vrijeme zadnje izmjene:	10.10.2013.
*	Autor:	Dubravko Voncina
*	Mail:	team@aaiedu.hr
*
*******************************************************************************************/

/*
*    Putanja do Composer autoload.php datoteke koja ucitava Redis:
*
*****************************************************************************************/
require __DIR__ . '/vendor/autoload.php';
Predis\Autoloader::register();


/*
*    Putanja do datoteke _autoload.php u direktoriju u kojem je instaliran simpleSAMLphp:
*
*****************************************************************************************/
require_once('C:\demo\simplesamlphp\lib\_autoload.php');


/*
*	U nastavku treba odkomentirati jednu od sljedece tri linije, ovisno o tome
*	koristi li aplikacija za autentikaciju testni fed-lab SSO servis, produkcijski
*	AAI@EduHr SSO servis ili proxy servis za autentikaciju korisnika putem
*	drustvenih mreza:
*
***********************************************************************************/
$as = new SimpleSAML_Auth_Simple('fedlab-sp');		// testni, fed-lab SSO
// $as = new SimpleSAML_Auth_Simple('default-sp');	// produkcijski SSO servis
// $as = new SimpleSAML_Auth_Simple('proxy-sp');	// proxy za drustvene mreze

//if( !$as->isAuthenticated() ){
	$valid = false;
	$guid = null;
	$client = null;
    if(!empty($_GET['guid'])){
        try {
            $client = new Predis\Client();
            $client->ping();

            $guidCheck = $client->get($_GET['guid']);
            $guid = (string)$_GET['guid'];

            if ($guidCheck != null) {
				$as->requireAuth();
				$attributes = $as->getAttributes();
				session_start();
				if(isset($attributes['hrEduPersonUniqueID'][0])){
					$client->del(array($guid));
					$client->hset($guid, 'mail', (isset($attributes['hrEduPersonUniqueID'][0]) ? $attributes['hrEduPersonUniqueID'][0] : "Nema"));
					$client->hset($guid, 'oib', (isset($attributes['hrEduPersonOIB'][0]) ? $attributes['hrEduPersonOIB'][0] : "Nema"));
					$client->hset($guid, 'first_name', (isset($attributes['displayName'][0]) ? $attributes['displayName'][0] : "Nema"));
					$client->hset($guid, 'last_name', (isset($attributes['cn'][0]) ? $attributes['cn'][0] : "Nema"));
					$client->expire($guid, 10);
					//die($guid . '_result');
					$valid = true;
					$_SESSION['username'] = $attributes['hrEduPersonUniqueID'][0];
				}
            } else {
                die('Error 2');
            }
        }

        catch (Exception $e) {
            echo "Couldn't connected to Redis" . "<br>";
            echo $e->getMessage();
            die();
        }
    } else {
        die('Error 1');
    }

	if($valid){
        header('Location: ' . 'http://dev1.mev.hr/e4-2018-auth/aspapp/User/CheckLogin?guid='.$guid, true, 301);
        die();
	} else {
        header('Location: ' . 'http://dev1.mev.hr/e4-2018-auth/aspapp/', true, 301);
        die();
	}
//} else {

	/*
	*	Ako je korisnik vec autenticiran, tj. postavljene su odgovarajuce vrijednosti session varijabli, redirect na naslovnicu:
	*
	*********************************************************************************************************************/
/*    header('Location: ' . 'http://dev1.mev.hr/e4-2018-auth/aspapp/', true, 301);
    die();
}*/

?>
