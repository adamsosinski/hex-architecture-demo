Param (
       [Parameter(Mandatory=$false,Position=0)]
       [ValidateSet('local','docker','partial')]
       [string]$mode = 'local'
    )

function Exec {
    [CmdletBinding()] param(
        [Parameter(Position = 0, Mandatory = 1)][scriptblock]$cmd,
        [Parameter(Position = 1, Mandatory = 0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}  

function runLocal {
    exec { & dotnet build -c Release }
    exec { & dotnet .\bin\Release\netcoreapp2.1\PrinterSample.dll }
}

function runDocker {
    exec { & docker-compose up --build --no-start fancy-printer }
    exec { & docker start -ia fancy-printer }
}

function runOther {
    exec { & docker -v }
    exec { & docker-compose down --rmi all }
    exec { & docker-compose up -d rabbitmq }
}

Push-Location .\sample\PrinterSample\

switch ($mode){
    'local'{
        runLocal
    }
    'docker'{
        runOther
        runDocker
    }
    'partial'{
        runOther
        runLocal
    }
}