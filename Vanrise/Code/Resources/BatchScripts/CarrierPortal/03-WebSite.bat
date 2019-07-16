ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CarrierPortal WebSite

xcopy "C:\Publish\CarrierPortal" /S /E /R /y /v /i /z /Q													"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\CarrierPortalSource\list-of-excluded-files.txt
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\Client-list-of-excluded-files.txt
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Web.config.exclud" /y /v /z /i /Q /R						"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Web.config*"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Images\tiles-icons" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Images\tiles-icons\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R								"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Bin\"
::by default load flat theme
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Images\flat-logoonheader.png" /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Images\logoonheader.png*"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Images\flat-login.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Images\login.png*"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Images\flat-iconheader.ico" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-01.png" 		 /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-02.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-member.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-support.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-flat.cshtml" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-flat.cshtml" /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\CSViews\Security\Login.cshtml*"
::default theme
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Images\default-logoonheader.png" /S /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Default-theme\Images\logoonheader.png*"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Images\default-login.png" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Default-theme\Images\login.png*"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Images\default-iconheader.ico" /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Default-theme\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-01.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Default-theme\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-02.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Default-theme\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-member.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Default-theme\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-support.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Default-theme\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-default.cshtml" /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Default-theme\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-default.cshtml" /S /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Default-theme\Client\CSViews\Security\Login.cshtml*"
::flat theme
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Images\flat-logoonheader.png" /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Flat-theme\Images\logoonheader.png*"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Images\flat-login.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Flat-theme\Images\login.png*"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Images\flat-iconheader.ico" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Flat-theme\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-01.png" 		 /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Flat-theme\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-02.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Flat-theme\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-member.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Flat-theme\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-support.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Flat-theme\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-flat.cshtml" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Flat-theme\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-flat.cshtml" /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Flat-theme\Client\CSViews\Security\Login.cshtml*"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\Modules\VR_GenericData\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\Modules\Analytic\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\Modules\Runtime\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.ExcelConversion.Web\ExcelConversion" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\Modules\ExcelConversion\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Rules.Web\VR_Rules" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\Modules\VR_Rules\"
del /s /q /f																								"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																								"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Bin\*.config"
xcopy "C:\TFS\Retail\Code\Output\Retail.BusinessEntity.APIEntities.dll" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Retail\Code\Output\Retail.BusinessEntity.Entities.dll" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Bin\"