ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO ZTE Mediator
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Addons.ZTEMediator\bin\Release" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Switches\ZTE\Mediator\%YYYYMMDD%\"
