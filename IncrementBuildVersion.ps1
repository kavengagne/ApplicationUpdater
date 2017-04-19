#
# Author: Kaven Gagné
# Description:
#   Auto-Increments the Revision part of the version in $filename
#   for the following properties: AssemblyVersion, AssemblyFileVersion
# Example:
#   1.0.2.8 becomes 1.0.2.9
#

param (
    [Parameter(Mandatory=$true)][string]$filename,
    [Parameter(ParameterSetName="major")][switch]$major,
    [Parameter(ParameterSetName="minor")][switch]$minor,
    [Parameter(ParameterSetName="build")][switch]$build,
    [Parameter(ParameterSetName="revision")][switch]$revision
)


function GetVersionNumber($content, $regex) {
    if ($content -match $regex) {
        return [version]$Matches[1]
    }
    return $null
}

function GetReplacement($propertyName, $version) {
    $part = $PSCmdlet.ParameterSetName
    $newVersion = $version.Major, $version.Minor, $version.Build, $version.Revision
    switch ($part) {
        "major" { $newVersion[0] = $newVersion[0] + 1 }
        "minor" { $newVersion[1] = $newVersion[1] + 1 }
        "build" { $newVersion[2] = $newVersion[2] + 1 }
        "revision" { $newVersion[3] = $newVersion[3] + 1 }
    }
    return ('{0}("{1}.{2}.{3}.{4}")' -f $propertyName, $newVersion[0], $newVersion[1], $newVersion[2], $newVersion[3])
}

function IncrementVersionNumberPart($content, $propertyName) {
    $patternVersion = "(\d+\.\d+\.\d+\.\d+)"
    $regex = ('{0}\("{1}"\)' -f $propertyName, $patternVersion)
    $version = (GetVersionNumber $content $regex)
    if ($version) {
        $replacement = (GetReplacement $propertyName $version)
        return ($content -replace $regex, $replacement)
    }
    else {
        throw "Version number not found."
    }
}

function IncrementVersion($file) {
    $content = [IO.File]::ReadAllText($file, [System.Text.Encoding]::UTF8)

    $content = IncrementVersionNumberPart $content "AssemblyVersion"
    $content = IncrementVersionNumberPart $content "AssemblyFileVersion"

    [IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8)
}


IncrementVersion $filename
