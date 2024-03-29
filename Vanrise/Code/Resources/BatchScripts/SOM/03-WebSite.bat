ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO SOM WebSite

xcopy "C:\Publish\SOM" /S /E /R /y /v /i /z /Q																"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\SOM\list-of-excluded-files.txt
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\Client-list-of-excluded-files.txt
xcopy "C:\TFS\SOM\Code\SOM.Web\Web.config.exclud" /y /v /z /i /Q /R											"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Web.config*"

::by default load flat theme
xcopy "C:\TFS\SOM\Code\SOM.Web\Images\flat-logoonheader.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Images\logoonheader.png*"
xcopy "C:\TFS\SOM\Code\SOM.Web\Images\flat-login.png" /S /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Images\login.png*"
xcopy "C:\TFS\SOM\Code\SOM.Web\Images\flat-iconheader.ico" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-01.png" 		 /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-02.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-member.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-support.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-flat.cshtml" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-flat.cshtml" /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\CSViews\Security\Login.cshtml*"
::default theme
xcopy "C:\TFS\SOM\Code\SOM.Web\Images\default-logoonheader.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Default-theme\Images\logoonheader.png*"
xcopy "C:\TFS\SOM\Code\SOM.Web\Images\default-login.png" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Default-theme\Images\login.png*"
xcopy "C:\TFS\SOM\Code\SOM.Web\Images\default-iconheader.ico" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Default-theme\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-01.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Default-theme\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-02.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Default-theme\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-member.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Default-theme\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-support.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Default-theme\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-default.cshtml" /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Default-theme\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-default.cshtml" /S /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Default-theme\Client\CSViews\Security\Login.cshtml*"
::flat theme
xcopy "C:\TFS\SOM\Code\SOM.Web\Images\flat-logoonheader.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Flat-theme\Images\logoonheader.png*"
xcopy "C:\TFS\SOM\Code\SOM.Web\Images\flat-login.png" /S /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Flat-theme\Images\login.png*"
xcopy "C:\TFS\SOM\Code\SOM.Web\Images\flat-iconheader.ico" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Flat-theme\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-01.png" 		 /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Flat-theme\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-02.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Flat-theme\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-member.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Flat-theme\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-support.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Flat-theme\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-flat.cshtml" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Flat-theme\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-flat.cshtml" /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Flat-theme\Client\CSViews\Security\Login.cshtml*"

xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\ICSharpCode.SharpZipLib.dll" /y /v /z /i /Q /R					"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R								"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.BusinessProcess.Web\BusinessProcess" /S /E /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\Modules\BusinessProcess\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\Modules\Runtime\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Rules.Web\VR_Rules" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\Modules\VR_Rules\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\Modules\VR_GenericData\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\Modules\Analytic\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Notification.Web\VR_Notification" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\Modules\VR_Notification\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.ExcelConversion.Web\ExcelConversion" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\Modules\ExcelConversion\"
xcopy "C:\TFS\SOM\Code\SOM.Main.Web\SOM_Main" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\Modules\SOM_Main\"
xcopy "C:\TFS\SOM\Code\SOM.ST.Web\SOM_ST" /S /E /R /y /v /i /z /Q											"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Client\Modules\SOM_ST\"
del /s /q /f																								"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																								"\\192.168.110.185\Fixes\WebSite\BIL\%YYYYMMDD%\Bin\*.config"