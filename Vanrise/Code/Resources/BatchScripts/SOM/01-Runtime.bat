ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO SOM Runtime
xcopy "C:\TFS\SOM\Code\SOM.Runtime\bin\x64\Release" 				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\BIL\Runtime\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\SOM\runtime-list-of-excluded-files.txt
xcopy "C:\TFS\SOM\Code\SOM.Runtime\app.config.exclude" 				/y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\BIL\Runtime\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\BIL\Runtime\%YYYYMMDD%\app.config.exclude" "TOne.WhS.Runtime.exe.config"

del /s /q /f "\\192.168.110.185\Fixes\WebSite\BIL\Runtime\%YYYYMMDD%\*.pdb"