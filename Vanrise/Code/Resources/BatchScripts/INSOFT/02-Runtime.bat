ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO INSOFT Runtime

xcopy "C:\TFS\NetworkProvisioning\Code\Output\NP.IVSwitch.Business.dll" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\INSOFT\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\NetworkProvisioning\Code\Output\NP.IVSwitch.Data.dll" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\INSOFT\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\NetworkProvisioning\Code\Output\NP.IVSwitch.Data.Postgres.dll" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\INSOFT\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\NetworkProvisioning\Code\Output\NP.IVSwitch.Entities.dll" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\INSOFT\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.RouteSync.IVSwitch\bin\Release\TOne.WhS.RouteSync.IVSwitch.dll" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\INSOFT\Runtime\%YYYYMMDD%\"