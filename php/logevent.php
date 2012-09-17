<?php require("logphoria.php");

$logphoria = new Logphoria('eddfab7a-a992-0311-80f8-bbc22b69a8a1');

$logphoria->log('INFO', 'Test message from php', array('prop1' => 'value1', 'prop2' => 2));

?>
