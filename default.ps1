Framework "4.5.1"

properties {
  $pwd = Split-Path $psake.build_script_file
  $ci_build_dir = "$pwd\Build\"
  $configuration = "Debug"
  $projectName = "Poc.Automate.Azure.Infrastructure"
  $solutionFile = $projectName + ".sln"
}

TaskSetup {
  if($env:TEAMCITY_VERSION) {
    Write-Output "##teamcity[progressMessage 'Running task $($psake.context.Peek().currentTaskName)']"
  }
}

task default -depends build
task init -depends config-iis, register-eventsource, bootstrap-paket, restore-packages

task restore-packages {
  Write-Host "== Restoring packages ==" -ForegroundColor Green
  Exec { & .\tools\nuget\nuget.exe restore }
}

task build {
  Write-Host "== Building solution ==" -ForegroundColor Green
  Exec { & msbuild /p:Configuration=$configuration $solutionFile /m /v:quiet }
}

task build-ci -depends restore-packages {
  Write-Host "== Building solution ==" -ForegroundColor Green
  Exec { & msbuild /p:Configuration=$configuration /p:OutDir=$ci_build_dir $solutionFile /m /v:quiet /p:DebugSymbols=false /p:DebugType=None }
}