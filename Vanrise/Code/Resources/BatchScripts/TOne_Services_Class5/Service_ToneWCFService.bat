ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO MNP
xcopy "C:\TOneRetail\TOne_WCFService\TOne_WCFServiceSETUP\Release\setup.exe" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class5\ToneWCFService\%YYYYMMDD%\"
xcopy "C:\TOneRetail\TOne_WCFService\TOne_WCFServiceSETUP\Release\TOne_WCFServiceSETUP.msi" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class5\ToneWCFService\%YYYYMMDD%\"