ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO CDRAnalysis Runtime
xcopy "C:\TFS\CDRAnalysis\Vanrise.Fzero.DevRuntime\bin\x64\Release" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\Runtime\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\CDRAnalysis\runtime-list-of-excluded-files.txt
xcopy "C:\TFS\CDRAnalysis\Vanrise.Fzero.DevRuntime\app.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\Runtime\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\Runtime\%YYYYMMDD%\app.config.exclude" "Vanrise.Fzero.DevRuntime.exe.config"

del /s /q /f "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\Runtime\%YYYYMMDD%\*.pdb"