ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Ntegra.CDRBilling
xcopy "C:\Ntegra.CDRBillingService\NtegraBillingSetup\Release\setup.exe" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Ntegra\Services\CDRBilling\%YYYYMMDD%\"
xcopy "C:\Ntegra.CDRBillingService\NtegraBillingSetup\Release\Ntegra.CDRBilling.msi" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Ntegra\Services\CDRBilling\%YYYYMMDD%\"