ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO TOne V2 WebSite Full version

xcopy "C:\Publish\SOM" /S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\BIL\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\SOM\list-of-excluded-files.txt
xcopy "C:\TFS\SOM\Code\SOM.Web\Web.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\FixesFullVersion\WebSite\BIL\%YYYYMMDD%\"
rename "\\192.168.110.185\FixesFullVersion\WebSite\BIL\%YYYYMMDD%\Web.config.exclude" "Web.config"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\ICSharpCode.SharpZipLib.dll" /y /v /z /i /Q /R									"\\192.168.110.185\FixesFullVersion\WebSite\BIL\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q											"\\192.168.110.185\FixesFullVersion\WebSite\BIL\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q												"\\192.168.110.185\FixesFullVersion\WebSite\BIL\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q														"\\192.168.110.185\FixesFullVersion\WebSite\BIL\%YYYYMMDD%\Client\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\SOM\Client-list-of-excluded-files.txt
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.BusinessProcess.Web\BusinessProcess" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\FixesFullVersion\WebSite\BIL\%YYYYMMDD%\Client\Modules\BusinessProcess\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q												"\\192.168.110.185\FixesFullVersion\WebSite\BIL\%YYYYMMDD%\Client\Modules\Runtime\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Rules.Web\VR_Rules" /S /E /R /y /v /i /z /Q												"\\192.168.110.185\FixesFullVersion\WebSite\BIL\%YYYYMMDD%\Client\Modules\VR_Rules\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\FixesFullVersion\WebSite\BIL\%YYYYMMDD%\Client\Modules\VR_GenericData\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q											"\\192.168.110.185\FixesFullVersion\WebSite\BIL\%YYYYMMDD%\Client\Modules\Analytic\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Notification.Web\VR_Notification" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\FixesFullVersion\WebSite\BIL\%YYYYMMDD%\Client\Modules\VR_Notification\"
xcopy "C:\TFS\SOM\Code\SOM.Main.Web\SOM_Main" /S /E /R /y /v /i /z /Q														"\\192.168.110.185\FixesFullVersion\WebSite\BIL\%YYYYMMDD%\Client\Modules\SOM_Main\"