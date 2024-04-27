# Define parameters
param(
	[Parameter(Mandatory = $true)]
	[string]$DestinationProjectName,
	[string[]]$SourceFolders = @("GDebugPanelGodot", "TerraBrush\addons\terrabrush")
)

Write-Host "# Resolve the full path of the destination directory"
$Destination = ("..\project\" + $DestinationProjectName)
$FullDestination = Resolve-Path -Path ("..\project\" + $Destination)

# Check if the full destination directory exists, if not, abort the script
if (-Not (Test-Path -Path $FullDestination)) {
	Write-Error "Destination directory does not exist: $FullDestination"
	exit 1  # Exit the script with an error status
}

# Resolve the full path of the destination directory and append the 'addons' subdirectory
$FullDestination = Join-Path -Path (Resolve-Path -Path $Destination) -ChildPath "addons"
# Safely resolve the destination path and append 'addons' subdirectory
# $FullDestination = Join-Path -Path (Resolve-Path -Path $Destination -ErrorAction Stop) -ChildPath (".\addons")
# $FullDestination = $FullDestination + "\addons"

# Ensure the destination directory exists; create it if it does not
if (-Not (Test-Path -Path $FullDestination)) {
	Write-Host "Destination directory does not exist, creating: $FullDestination"
	New-Item -Path $FullDestination -ItemType Directory
}

Write-Host "# Iterate over each source folder"
foreach ($Folder in $SourceFolders) {

	Write-Host $Folder
	
	if ([string]::IsNullOrWhiteSpace($Folder)) {
		Write-Error "Folder path is null or empty for one of the source folders."
		continue  # Skip this iteration
	}

	$SourceDir = Join-Path -Path $PSScriptRoot -ChildPath $Folder

	# Check if the source directory exists
	if (-Not (Test-Path -Path $SourceDir -PathType Container)) {
		Write-Error "Source directory does not exist: $SourceDir"
		continue
	}

	# Copy the current source directory to the specified destination
	try {
		Copy-Item -Path $SourceDir -Destination $FullDestination -Recurse -Force
		Write-Host "Successfully copied '$SourceDir' to '$FullDestination'"
	}
 catch {
		Write-Error "Failed to copy '$SourceDir' to '$FullDestination': $_"
	}
}

# Pause the script and wait for the user to press a key
Read-Host "Press any key to continue..."

