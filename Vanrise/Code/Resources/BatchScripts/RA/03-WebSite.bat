ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO RA WebSite

xcopy "C:\Publish\Retail" /S /E /R /y /v /i /z /Q																	"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\RetailSource\list-of-excluded-files.txt
xcopy "C:\TFS\Retail\Code\Retail.Web\Web.config.RA.exclude" /y /v /z /i /Q /R										"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\RA-logoonheader.png" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Images\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\RA-login.png" /S /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Images\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\RA-iconheader.ico" /S /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Images\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\menu-icons" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Images\menu-icons\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\figure-icons" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Images\figure-icons\"
rename																												"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Web.config.RA.exclude" "Web.config"
rename																												"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Images\RA-logoonheader.png" "logoonheader.png"
rename																												"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Images\RA-login.png" "login.png"
rename																												"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Images\RA-iconheader.ico" "iconheader.ico"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R										"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q												"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\Client-list-of-excluded-files.txt
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.AccountBalance.Web\VR_AccountBalance" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\VR_AccountBalance\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Notification.Web\VR_Notification" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\VR_Notification\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.ExcelConversion.Web\ExcelConversion" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\ExcelConversion\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.BEBridge.Web\VR_BEBridge" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\VR_BEBridge\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\Runtime\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.Queueing.Web\Queueing" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\Queueing\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Rules.Web\VR_Rules" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\VR_Rules\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Integration.Web\Integration" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\Integration\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\VR_GenericData\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\Analytic\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.BusinessProcess.Web\BusinessProcess" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\BusinessProcess\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.NumberingPlan.Web\VR_NumberingPlan" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\VR_NumberingPlan\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Invoice.Web\VR_Invoice" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\VR_Invoice\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.InvToAccBalanceRelation.Web\VR_InvToAccBalanceRelation" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\VR_InvToAccBalanceRelation\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.Reprocess.Web\Reprocess" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\Reprocess\"
xcopy "C:\TFS\Retail\Code\Retail.BusinessEntity.Web\Retail_BusinessEntity" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\Retail_BusinessEntity\"
xcopy "C:\TFS\Retail\Code\Retail.Voice.Web\Retail_Voice" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\Retail_Voice\"
xcopy "C:\TFS\Retail\Code\Retail.SMS.Web\Retail_SMS" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\Retail_SMS\"
xcopy "C:\TFS\Retail\Code\Retail.Data.Web\Retail_Data" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\Retail_Data\"
xcopy "C:\TFS\Retail\Code\Retail.Invoice.Web\Retail_Invoice" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\Retail_Invoice\"
xcopy "C:\TFS\Retail\Code\Retail.RA.Web\Retail_RA" /S /E /R /y /v /i /z /Q											"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\Retail_RA\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.MobileNetwork.Web\VR_MobileNetwork" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Client\Modules\VR_MobileNetwork\"
xcopy "C:\Publish\Retail\bin\Retail.RA*" /S /E /R /y /v /i /z /Q													"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Bin\"
xcopy "C:\Publish\Retail\bin\Vanrise.MobileNetwork*" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Bin\"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\RA\%YYYYMMDD%\Bin\*.config"