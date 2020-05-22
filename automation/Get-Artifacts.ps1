$ArtifactName = $args[0]
$ArtifactSource = $(Pipeline.Workspace)/s/src/$ArtifactName
$ArtifactDestinationPathPath = $(Pipeline.Workspace)/s/output/$ArtifactName
$SubFoldersList = dir $ArtifactSource | Where-Object {$_.PSIsContainer} | ForEach-Object -Process {$_.FullName}

ForEach ($Folder in $SubFoldersList) {
	$ArtifactSourcePath = Join-Path $Folder code
	robocopy $ArtifactSourcePath/bin $ArtifactDestinationPath/bin HCA.$ArtifactName.*
	robocopy $ArtifactSourcePath/App_Config $ArtifactDestinationPath/App_Config /s
	robocopy $ArtifactSourcePath/dist $ArtifactDestinationPath/dist /s
	robocopy $ArtifactSourcePath/Views $ArtifactDestinationPath/Views /s
	}
