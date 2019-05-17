ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO TelecomCRM WebSite

xcopy "C:\Publish\Retail" /S /E /R /y /v /i /z /Q																	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\RetailSource\list-of-excluded-files.txt
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q												"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\Client-list-of-excluded-files.txt
xcopy "C:\TFS\Retail\Code\Retail.Web\Web.config.TelecomCRM.exclud" /y /v /z /i /Q /R								"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Web.config*"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\menu-icons" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Images\menu-icons\"
::by default load flat theme
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\flat-TelecomCRM-logoonheader.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Images\logoonheader.png*"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\flat-TelecomCRM-login.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Images\login.png*"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\flat-TelecomCRM-iconheader.ico" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-01.png" 		 /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-02.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-member.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-support.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-flat.cshtml" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-flat.cshtml" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\CSViews\Security\Login.cshtml*"
::default theme
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\default-TelecomCRM-logoonheader.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Default-theme\Images\logoonheader.png*"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\default-TelecomCRM-login.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Default-theme\Images\login.png*"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\default-TelecomCRM-iconheader.ico" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Default-theme\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-01.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Default-theme\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-02.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Default-theme\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-member.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Default-theme\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-support.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Default-theme\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-default.cshtml" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Default-theme\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-default.cshtml" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Default-theme\Client\CSViews\Security\Login.cshtml*"
::flat theme
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\flat-TelecomCRM-logoonheader.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Flat-theme\Images\logoonheader.png*"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\flat-TelecomCRM-login.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Flat-theme\Images\login.png*"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\flat-TelecomCRM-iconheader.ico" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Flat-theme\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-01.png" 		 /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Flat-theme\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-02.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Flat-theme\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-member.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Flat-theme\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-support.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Flat-theme\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-flat.cshtml" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Flat-theme\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-flat.cshtml" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Flat-theme\Client\CSViews\Security\Login.cshtml*"

xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Notification.Web\VR_Notification" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\Modules\VR_Notification\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.ExcelConversion.Web\ExcelConversion" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\Modules\ExcelConversion\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\Modules\Runtime\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Rules.Web\VR_Rules" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\Modules\VR_Rules\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\Modules\VR_GenericData\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\Modules\Analytic\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.BusinessProcess.Web\BusinessProcess" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\Modules\BusinessProcess\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.BEBridge.Web\VR_BEBridge" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Client\Modules\VR_BEBridge\"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Bin\Retail.*"
xcopy "C:\Publish\Retail\bin\Retail.Web.*" /S /E /R /y /v /i /z /Q													"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Bin\"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Bin\Vanrise.Integration.*"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Bin\Vanrise.Queueing.*"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Bin\Vanrise.Reprocess.*"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Bin\Vanrise.AccountManager.*"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Bin\Vanrise.AccountBalance.*"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Bin\Vanrise.Invoice.*"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Bin\Vanrise.InvToAccBalanceRelation.*"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Bin\Vanrise.NumberingPlan.*"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\Bin\*.config"
