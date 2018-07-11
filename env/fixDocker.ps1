#fix issue for ERROR: for lb  Cannot create container for service lb: b'Mount denied:\nThe source path "\\\\var\\\\run\\\\docker.sock:/var/run/docker.sock"\nis not a valid Windows path'
# run from package manager console
$Env:COMPOSE_CONVERT_WINDOWS_PATHS=1 
