ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO XBooster Runtime
xcopy "C:\TFS\Xbooster\Code\XBooster.Runtime\bin\x64\Release" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\XBooster\Runtime\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\XBooster\runtime-list-of-excluded-files.txt
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\XBooster\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Xbooster\Code\XBooster.Runtime\app.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\XBooster\Runtime\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\XBooster\Runtime\%YYYYMMDD%\app.config.exclude" "XBooster.Runtime.exe.config"

del /s /q /f "\\192.168.110.185\Fixes\WebSite\XBooster\Runtime\%YYYYMMDD%\*.pdb"