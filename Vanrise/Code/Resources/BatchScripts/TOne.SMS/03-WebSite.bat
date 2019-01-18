ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO TOne.SMS WebSite

xcopy "C:\Publish\Retail" /S /E /R /y /v /i /z /Q																	"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\RetailSource\list-of-excluded-files.txt
xcopy "C:\TFS\Retail\Code\Retail.Web\Web.config.SMS-WHS.exclude" /y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R										"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\SMS-WHS-logoonheader.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Images\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\SMS-WHS-login.png" /S /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Images\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\SMS-WHS-iconheader.ico" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Images\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\menu-icons" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Images\menu-icons\"
rename																												"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Web.config.SMS-WHS.exclude" "Web.config"
rename																												"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Images\SMS-WHS-logoonheader.png" "logoonheader.png"
rename																												"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Images\SMS-WHS-login.png" "login.png"
rename																												"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Images\SMS-WHS-iconheader.ico" "iconheader.ico"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q												"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\RetailSource\Client-list-of-excluded-files.txt
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.AccountBalance.Web\VR_AccountBalance" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\VR_AccountBalance\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Notification.Web\VR_Notification" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\VR_Notification\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.ExcelConversion.Web\ExcelConversion" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\ExcelConversion\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\Runtime\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.Queueing.Web\Queueing" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\Queueing\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Rules.Web\VR_Rules" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\VR_Rules\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Integration.Web\Integration" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\Integration\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\VR_GenericData\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\Analytic\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.BusinessProcess.Web\BusinessProcess" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\BusinessProcess\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.NumberingPlan.Web\VR_NumberingPlan" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\VR_NumberingPlan\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Invoice.Web\VR_Invoice" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\VR_Invoice\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.InvToAccBalanceRelation.Web\VR_InvToAccBalanceRelation" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\VR_InvToAccBalanceRelation\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.Reprocess.Web\Reprocess" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\Reprocess\"
xcopy "C:\TFS\Retail\Code\Retail.BusinessEntity.Web\Retail_BusinessEntity" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\Retail_BusinessEntity\"
xcopy "C:\TFS\Retail\Code\Retail.Invoice.Web\Retail_Invoice" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\Retail_Invoice\"
xcopy "C:\TFS\Retail\Code\Retail.Demo.Web\Retail_Demo" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Client\Modules\Retail_Demo\"
xcopy "C:\Publish\Retail\bin\Retail.Demo*" /S /E /R /y /v /i /z /Q													"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Bin\"
xcopy "C:\Publish\Retail\bin\Retail.SMS*" /S /E /R /y /v /i /z /Q													"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Bin\"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\TOne.SMS\%YYYYMMDD%\Bin\*.config"