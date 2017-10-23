ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO ClearVoice Runtime
xcopy "C:\TFS\QualityMeasurement\Code\QualityMeasurement.DevRuntime\bin\x64\Release" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\ClearVoice\Runtime\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\ClearVoice\runtime-list-of-excluded-files.txt
xcopy "C:\TFS\QualityMeasurement\Code\QualityMeasurement.DevRuntime\app.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\ClearVoice\Runtime\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\ClearVoice\Runtime\%YYYYMMDD%\app.config.exclude" "QM.Runtime.exe.config"

del /s /q /f "\\192.168.110.185\Fixes\WebSite\ClearVoice\Runtime\%YYYYMMDD%\*.pdb"
