ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Vanrise.HelperTools
xcopy "C:\TFS\Vanrise\Code\Vanrise.HelperTools\bin\Release" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Common\Vanrise.HelperTools\%YYYYMMDD%\"