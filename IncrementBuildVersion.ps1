
param (
    [Parameter(Mandatory=$true)][string]$filename
)


function GetVersionNumber($content, $regex) {
    if ($content -match $regex) {
        return [version]$Matches[1]
    }
    return $null
}

function ReplaceRevisionNumber($content, $propertyName) {
    $patternVersion = "(\d+\.\d+\.\d+\.\d+)"
    $regex = ('{0}\("{1}"\)' -f $propertyName, $patternVersion)
    $version = (GetVersionNumber $content $regex)
    if ($version) {
        $replace = ('{0}("{1}.{2}.{3}.{4}")' -f $propertyName, $version.Major, $version.Minor, $version.Build, ($version.Revision + 1))
        return ($content -replace $regex, $replace)
    }
    else {
        throw "Revision number not found."
    }
}

function IncreaseBuildVersion($file) {
    $content = [IO.File]::ReadAllText($file, [System.Text.Encoding]::UTF8)

    $content = (ReplaceRevisionNumber $content "AssemblyVersion")
    $content = (ReplaceRevisionNumber $content "AssemblyFileVersion")

    [IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8)
}


IncreaseBuildVersion $filename
