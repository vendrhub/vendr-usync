for /f "usebackq tokens=*" %%i in (`.\build\_tools\vswhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do (
  "%%i" "build\Build.proj  -target:%%BUILD_TARGET%%"
  exit /b !errorlevel!
)