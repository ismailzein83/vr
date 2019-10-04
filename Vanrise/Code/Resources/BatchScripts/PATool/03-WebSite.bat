ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO PA WebSite

xcopy "C:\Publish\Vanrise.Web.Host" /S /E /R /y /v /i /z /Q												"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\PATool\list-of-excluded-files.txt
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Client\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\Client-list-of-excluded-files.txt
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Web.config.PA.exclud" /y /v /z /i /Q /R						"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Web.config*"
::by default load flat theme
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\flat-PATool-logoonheader.png" /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Images\logoonheader.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\flat-PATool-login.png" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Images\login.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\flat-PATool-iconheader.ico" /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-01.png" 		 /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-02.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-member.png" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-support.png" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-flat.cshtml" /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-flat.cshtml" /S /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Client\CSViews\Security\Login.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R							"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\ICSharpCode.SharpZipLib.dll" /y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Notification.Web\VR_Notification" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Client\Modules\VR_Notification\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Client\Modules\VR_GenericData\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Client\Modules\Analytic\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.ExcelConversion.Web\ExcelConversion" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Client\Modules\ExcelConversion\"
del /s /q /f																							"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																							"\\192.168.110.185\Fixes\WebSite\PA\%YYYYMMDD%\Bin\*.config"