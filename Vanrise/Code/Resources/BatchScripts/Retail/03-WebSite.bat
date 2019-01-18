ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Retail WebSite

xcopy "C:\Publish\Retail" /S /E /R /y /v /i /z /Q																	"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\RetailSource\list-of-excluded-files.txt
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\logoonheader.png" /S /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Images\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\login.png" /S /R /y /v /i /z /Q											"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Images\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\iconheader.ico" /S /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Images\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\iconheader.ico" /S /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Images\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\menu-icons" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Images\menu-icons\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Web.config.exclude" /y /v /z /i /Q /R											"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\"
rename																												"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Web.config.exclude" "Web.config"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R										"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q												"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\Client-list-of-excluded-files.txt
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.AccountBalance.Web\VR_AccountBalance" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\VR_AccountBalance\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Notification.Web\VR_Notification" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\VR_Notification\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.ExcelConversion.Web\ExcelConversion" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\ExcelConversion\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.BEBridge.Web\VR_BEBridge" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\VR_BEBridge\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\Runtime\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.Queueing.Web\Queueing" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\Queueing\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Rules.Web\VR_Rules" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\VR_Rules\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Integration.Web\Integration" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\Integration\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\VR_GenericData\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\Analytic\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.BusinessProcess.Web\BusinessProcess" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\BusinessProcess\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.NumberingPlan.Web\VR_NumberingPlan" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\VR_NumberingPlan\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Invoice.Web\VR_Invoice" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\VR_Invoice\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.InvToAccBalanceRelation.Web\VR_InvToAccBalanceRelation" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\VR_InvToAccBalanceRelation\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.Reprocess.Web\Reprocess" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\Reprocess\"
xcopy "C:\TFS\Retail\Code\Retail.BusinessEntity.Web\Retail_BusinessEntity" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\Retail_BusinessEntity\"
xcopy "C:\TFS\Retail\Code\Retail.Voice.Web\Retail_Voice" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\Retail_Voice\"
xcopy "C:\TFS\Retail\Code\Retail.SMS.Web\Retail_SMS" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\Retail_SMS\"
xcopy "C:\TFS\Retail\Code\Retail.Data.Web\Retail_Data" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\Retail_Data\"
xcopy "C:\TFS\Retail\Code\Retail.Invoice.Web\Retail_Invoice" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\Retail_Invoice\"
xcopy "C:\TFS\Retail\Code\Retail.Cost.Web\Retail_Cost" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Client\Modules\Retail_Cost\"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\Retail\%YYYYMMDD%\Bin\*.config"