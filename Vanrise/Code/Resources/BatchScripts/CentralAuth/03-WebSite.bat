ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CentralAuth WebSite

xcopy "C:\Publish\TOneV2" /S /E /R /y /v /i /z /Q													"\\192.168.110.185\Fixes\WebSite\CentralAuth\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\CentralAuth\list-of-excluded-files.txt
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Web.config.CentralAuth.exclude" /y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\CentralAuth\%YYYYMMDD%\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R						"\\192.168.110.185\Fixes\WebSite\CentralAuth\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\ICSharpCode.SharpZipLib.dll" /y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\CentralAuth\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\CentralAuth\%YYYYMMDD%\Client\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\CentralAuth\Client-list-of-excluded-files.txt
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\CentralAuth\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\CentralAuth\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Notification.Web\VR_Notification" /S /E /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\CentralAuth\%YYYYMMDD%\Client\Modules\VR_Notification\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\CentralAuth\%YYYYMMDD%\Client\Modules\VR_GenericData\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\CentralAuth\%YYYYMMDD%\Client\Modules\Analytic\"

rename "\\192.168.110.185\Fixes\WebSite\CentralAuth\%YYYYMMDD%\images\CentralAuth-logoonheader.png" "logoonheader.png"
rename "\\192.168.110.185\Fixes\WebSite\CentralAuth\%YYYYMMDD%\images\CentralAuth-login.png" "login.png"
rename "\\192.168.110.185\Fixes\WebSite\CentralAuth\%YYYYMMDD%\images\CentralAuth-iconheader.ico" "iconheader.ico"
rename "\\192.168.110.185\Fixes\WebSite\CentralAuth\%YYYYMMDD%\Web.config.CentralAuth.exclude" "Web.config"