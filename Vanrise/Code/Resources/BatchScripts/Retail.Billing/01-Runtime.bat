ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Retail.Billing Runtime
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release" /y /v /z /i /Q /R										"\\192.168.110.185\Fixes\WebSite\Retail.Billing\Runtime\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\RetailSource\runtime-list-of-excluded-files.txt
xcopy "C:\TFS\Retail\Code\Retail.Runtime\App.config.Retail.Billing.exclude" /y /v /z /i /Q /R					"\\192.168.110.185\Fixes\WebSite\Retail.Billing\Runtime\%YYYYMMDD%\"
rename																											"\\192.168.110.185\Fixes\WebSite\Retail.Billing\Runtime\%YYYYMMDD%\App.config.Retail.Billing.exclude"	"Retail.Billing.Runtime.exe.config"
rename																											"\\192.168.110.185\Fixes\WebSite\Retail.Billing\Runtime\%YYYYMMDD%\Retail.Runtime.exe"					"Retail.Billing.Runtime.exe"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release\Retail.Billing*" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\Retail.Billing\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Billing.Web\Retail_Billing\Elements\Reports\*.rdlc" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\Runtime\%YYYYMMDD%\Modules\Retail_Billing\Elements\Reports"
del /s /q /f																									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\Runtime\%YYYYMMDD%\*.pdb"
