ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CarrierPortal.TOne WebSite

xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\CP.TOne\%YYYYMMDD%\Client\Modules\Analytic\"
xcopy "C:\TFS\CarrierPortal\Code\CP.WhS.Web\CP_WhS" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\CP.TOne\%YYYYMMDD%\Client\Modules\CP_WhS\"