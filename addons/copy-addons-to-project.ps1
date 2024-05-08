####################################################################################################
# Script to copy the specified source folders to the destination project's 'addons' directory
# This script is not elegant, nor general-purpose.  
# It is just a hack to easily copy interesting addons to godot csharp projects while experimenting in this repo.

# Running this script will overwrite ALL the contents of the destination project's 'addons' directory with the addons described in the $SourceFolders variable.

# Usage: For Win11 based dev.  can run by right-clicking the script and selecting 'Run with PowerShell'
#        script will prompt for the destination project name.  this should be under the 'project' directory

####################################################################################################

# Define parameters
param(
	[Parameter(Mandatory = $true)]
	[string]$DestinationProjectName,
	[string[]]$SourceFolders = @(
		"GDebugPanelGodot"
		, "TerraBrush\addons\terrabrush"
		, "godot_debug_draw_3d\addons\debug_draw_3d"
		, "godot_input_helper\addons\input_helper"
		# ,"godotex\src\GodotEx"
		# ,".\GodotEx\src\GodotEx.Async"
		# ,".\GodotEx\src\GodotEx.Hosting"
	)	
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

# delete target addons folder before copying new ones over
if (Test-Path $FullDestination) {
	# Remove all contents and subdirectories recursively
	Remove-Item -Path "$FullDestination\*" -Recurse -Force

	# If you also want to delete the root folder itself, uncomment the following line:
	# Remove-Item -Path $FOLDERNAME -Force
}
else {
	Write-Output "Folder does not exist: $FullDestination"
}


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
		Copy-Item -Path $SourceDir -Destination $FullDestination -Recurse -Force -ErrorAction SilentlyContinue
		Write-Host "Successfully copied '$SourceDir' to '$FullDestination'"
	}
 catch {
		Write-Error "Failed to copy '$SourceDir' to '$FullDestination': $_"
	}
}

# Pause the script and wait for the user to press a key
Read-Host "Press any key to continue..."

