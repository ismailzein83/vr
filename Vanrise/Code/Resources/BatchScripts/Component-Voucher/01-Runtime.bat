ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Component-Voucher Runtime

xcopy "C:\TFS\Vanrise\Code\Output\Vanrise.Voucher*"	/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\Component-Voucher\Runtime\%YYYYMMDD%\"
del /s /q /f /Q															"\\192.168.110.185\Fixes\WebSite\Component-Voucher\Runtime\%YYYYMMDD%\Vanrise.Voucher.Web.dll"