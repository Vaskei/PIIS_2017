<?php

/*
*    Putanja do datoteke _autoload.php u direktoriju u kojem je instaliran simpleSAMLphp:
*
*****************************************************************************************/
// require_once('/var/www/simplesamlphp/lib/_autoload.php');
require_once('C:\demo\simplesamlphp\lib\_autoload.php');

/*
*   U nastavku treba odkomentirati jednu od sljedece tri linije, ovisno o tome
*   koristi li aplikacija za autentikaciju testni fed-lab SSO servis, produkcijski
*   AAI@EduHr SSO servis ili proxy servis za autentikaciju korisnika putem
*   drustvenih mreza:
*
***********************************************************************************/
$as = new SimpleSAML_Auth_Simple('fedlab-sp');     // testni, fed-lab SSO
// $as = new SimpleSAML_Auth_Simple('default-sp');  // produkcijski SSO servis
// $as = new SimpleSAML_Auth_Simple('proxy-sp');    // proxy za drustvene mreze


if( $as->isAuthenticated() ) {
    session_start();
    unset($_SESSION['username']);
	$as->logout( array( 'ReturnTo' => 'http://dev1.mev.hr/e4-2018-auth/aspapp/User/CheckLogout' ) );
}

?>
