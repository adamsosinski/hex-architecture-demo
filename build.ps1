Param(
[Parameter(Mandatory=$false,Position=0)]
[string]$dir
)

if($PSBoundParameters.ContainsKey('dir')) { Set-Location $dir }

function Exec
{
    [CmdletBinding()] param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0){
        throw ("Exec: " + $errorMessage)
    }
}

if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

Push-Location .\test\Printer.Test\
exec { & dotnet test -c Release }
Pop-Location

exec { & dotnet publish .\src\Printer -c Release -o ..\..\artifacts }
