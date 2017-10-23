ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Pricing
xcopy "C:\TOne_Class5Pricing\TOneClass5PricingSETUP\Release\setup.exe" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class5\Pricing\%YYYYMMDD%\"
xcopy "C:\TOne_Class5Pricing\TOneClass5PricingSETUP\Release\TOneClass5PricingSETUP.msi" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class5\Pricing\%YYYYMMDD%\"