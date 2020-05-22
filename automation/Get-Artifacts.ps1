$ArtifactName = $args[0]
$ArtifactSource = Join-Path $(Pipeline.Workspace) s/src/$ArtifactName
$ArtifactDestination = Join-Path $(Pipeline.Workspace) s/output
$ArtifactDestinationLayer = Join-Path $ArtifactDestination $ArtifactName
$SubFoldersList = dir $ArtifactSource | Where-Object {$_.PSIsContainer} | ForEach-Object -Process {$_.FullName}

ForEach ($Folder in $SubFoldersList) {
	$ArtifactSourcePath = Join-Path $Folder code
	robocopy $ArtifactSourcePath/bin $ArtifactDestinationLayer/bin HCA.$ArtifactName.*
	robocopy $ArtifactSourcePath/App_Config $ArtifactDestinationLayer/App_Config /s
	robocopy $ArtifactSourcePath/dist $ArtifactDestinationLayer/dist /s
	robocopy $ArtifactSourcePath/Views $ArtifactDestinationLayer/Views /s
	}
robocopy $ArtifactSource $ArtifactDestination/unicorn/$ArtifactName *.yml /s
