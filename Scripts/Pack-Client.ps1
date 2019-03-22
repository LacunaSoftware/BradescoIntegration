$ErrorActionPreference = "Stop"

function Write-Log($s) {
	Write-Host ""
	Write-Host -ForegroundColor Green (">>> {0}" -f $s)
	Write-Host ""
}

function Check-ExitCode($errorMessage) {
	if ($LASTEXITCODE -ne 0) {
		throw $errorMessage
	}
}

try {

	#
	# Initialization
	#
	Write-Log "Initializing ..."
	if (Test-Path ".\*.nupkg") {
		Remove-Item ".\*.nupkg"
	}

	#
	# Restore nuget packages
	#
	Write-Log "Restoring nuget packages ..."

	& dotnet restore "..\Lacuna.BradescoIntegration\Lacuna.BradescoIntegration.csproj"
	Check-ExitCode "An error occurred while restoring the nuget packages"

	#
	# Build & pack
	#
	Write-Log "Building and packing class libraries ..."

	& dotnet pack "..\Lacuna.BradescoIntegration\Lacuna.BradescoIntegration.csproj" --configuration Release
	Check-ExitCode "An error occurred while building the class library"
	Move-Item ..\Lacuna.BradescoIntegration\bin\Release\*.nupkg .

	#
	# End
	#
	Write-Log "Done."

} catch {

	Write-Host -ForegroundColor Red ("FATAL: An unhandled exception has occurred`n{0}`n{1}" -f $_.Exception, $_.ScriptStackTrace)
	
}
