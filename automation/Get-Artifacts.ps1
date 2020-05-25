$ArtifactName = $args[0]
$ArtifactSource = Join-Path $env:Workspace s/src/$ArtifactName
$ArtifactDestination = Join-Path $env:Workspace s/output
$ArtifactDestinationLayer = Join-Path $ArtifactDestination $ArtifactName
$ToolsSource = Join-Path $env:Workspace s/src/tools
$UnicornSyncLocation = Join-Path $ToolsSource cake.sitecore/Cake.Sitecore/tools/unicorn
$SubFoldersList = dir $ArtifactSource | Where-Object {$_.PSIsContainer} | ForEach-Object -Process {$_.FullName}

ForEach ($Folder in $SubFoldersList) {
	$ArtifactSourcePath = Join-Path $Folder code
	robocopy $ArtifactSourcePath/bin $ArtifactDestinationLayer/bin HCA.$ArtifactName.*
	robocopy $ArtifactSourcePath/App_Config $ArtifactDestinationLayer/App_Config /s
	robocopy $ArtifactSourcePath/Views $ArtifactDestinationLayer/Views /s
	if ($Folder -match "Project") {
		$DistArtifactSourcePath = Join-Path $Folder client/build
		robocopy $DistArtifactSourcePath $ArtifactDestinationLayer/dist /s
		}
	}
robocopy $ArtifactSource $ArtifactDestination/unicorn/$ArtifactName *.yml /s
robocopy $UnicornSyncLocation $ArtifactDestination/unicorn-sync
