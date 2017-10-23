ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Teles Class5 Switch
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Addons.MeraIISwitchLibrary\bin\Release" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Switches\Teles\SwitchClass5\%YYYYMMDD%\"