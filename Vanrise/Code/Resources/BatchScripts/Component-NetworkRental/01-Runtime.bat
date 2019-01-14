ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Component-NetworkRental Runtime

xcopy "C:\TFS\Retail\Code\Retail.Demo.Web\Retail_Demo\Elements\NetworkRental\Reports\*.rdlc" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\Component-NetworkRental\Runtime\%YYYYMMDD%\Modules\Retail_Demo\Elements\NetworkRental\Reports"
xcopy "C:\TFS\Retail\Code\Retail.Demo.Web\bin\Retail.Demo*" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\Component-NetworkRental\Runtime\%YYYYMMDD%\"
del /s /q /f /Q																											"\\192.168.110.185\Fixes\WebSite\Component-NetworkRental\Runtime\%YYYYMMDD%\*.pdb"
del /s /q /f /Q																											"\\192.168.110.185\Fixes\WebSite\Component-NetworkRental\Runtime\%YYYYMMDD%\Retail.*.Web.*"

