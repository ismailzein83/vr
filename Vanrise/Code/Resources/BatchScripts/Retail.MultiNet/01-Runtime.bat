ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Retail.MultiNet Runtime

xcopy "C:\TFS\Retail\Code\Retail.MultiNet.Web\Retail_MultiNet\Reports\*.rdlc" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\Runtime\%YYYYMMDD%\Modules\Retail_MultiNet\Reports"
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release\Retail.MultiNet*" /S /E /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release\Retail.Teles*" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\Runtime\%YYYYMMDD%\"
del /s /q /f /Q																							"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\Runtime\%YYYYMMDD%\*.pdb"