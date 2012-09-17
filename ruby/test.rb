require 'logphoria'

logphoria = Logphoria.new "eddfab7a-a992-0311-80f8-bbc22b69a8a1"
update = logphoria.log "INFO", "Test message from ruby", "prop1" => "1", "prop2" => 2, "prop3" => 4
