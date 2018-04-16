ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO TOne V2 WebSite

xcopy "C:\Publish\TOneV2" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\TOneV2\list-of-excluded-files.txt
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Web.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Web.config.exclude" "Web.config"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\ICSharpCode.SharpZipLib.dll" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Bin\"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.ExcelConversion.Web\ExcelConversion" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\ExcelConversion\"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\TOneV2\Client-list-of-excluded-files.txt

xcopy "C:\TFS\BusinessProcess\Code\Vanrise.BusinessProcess.Web\BusinessProcess" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\BusinessProcess\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q												"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\Runtime\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.Queueing.Web\Queueing" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\Queueing\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Invoice.Web\VR_Invoice" /S /E /R /y /v /i /z /Q											"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\VR_Invoice\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Notification.Web\VR_Notification" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\VR_Notification\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Rules.Web\VR_Rules" /S /E /R /y /v /i /z /Q												"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\VR_Rules\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Integration.Web\Integration" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\Integration\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\VR_GenericData\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q											"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\Analytic\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.Reprocess.Web\Reprocess" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\Reprocess\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.AccountBalance.Web\VR_AccountBalance" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\VR_AccountBalance\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.InvToAccBalanceRelation.Web\VR_InvToAccBalanceRelation" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\VR_InvToAccBalanceRelation\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.BEBridge.Web\VR_BEBridge" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\VR_BEBridge\"

xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.CodePreparation.Web\WhS_CodePreparation" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\WhS_CodePreparation\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.BusinessEntity.Web\WhS_BusinessEntity" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\WhS_BusinessEntity\"
DEL /F /Q "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\WhS_BusinessEntity\Views\Switch\SingleSwitchManagement.html"
DEL /F /Q "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\WhS_BusinessEntity\Views\Switch\SingleSwitchManagementController.js"

xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.SupplierPriceList.Web\WhS_SupplierPriceList" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\WhS_SupplierPriceList\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.Sales.Web\WhS_Sales" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\WhS_Sales\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.Routing.Web\WhS_Routing" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\WhS_Routing\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.Analytics.Web\WhS_Analytics" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\WhS_Analytics\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.RouteSync.Web\WhS_RouteSync" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\WhS_RouteSync\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.Invoice.Web\WhS_Invoice" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\WhS_Invoice\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.Deal.Web\WhS_Deal" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\WhS_Deal\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.AccountBalance.Web\WhS_AccountBalance" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\WhS_AccountBalance\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.TOneV1Transition\WhS_TOneV1Transition" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\WhS_TOneV1Transition\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.InvToAccBalanceRelation.Web\WhS_InvToAccBalanceRelation" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\Client\Modules\WhS_InvToAccBalanceRelation\"