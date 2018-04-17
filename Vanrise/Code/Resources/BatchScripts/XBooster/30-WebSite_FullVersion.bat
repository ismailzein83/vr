ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO XBooster WebSite Full Version

xcopy "C:\Publish\XBooster" /S /E /R /y /v /i /z /Q																	"\\192.168.110.185\FixesFullVersion\WebSite\XBooster\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\XBooster\list-of-excluded-files.txt
xcopy "C:\TFS\Xbooster\Code\Xbooster.Web\Web.config.exclude" /y /v /z /i /Q /R										"\\192.168.110.185\FixesFullVersion\WebSite\XBooster\%YYYYMMDD%\"
rename "\\192.168.110.185\FixesFullVersion\WebSite\XBooster\%YYYYMMDD%\Web.config.exclude" "Web.config"

xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R										"\\192.168.110.185\FixesFullVersion\WebSite\XBooster\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q												"\\192.168.110.185\FixesFullVersion\WebSite\XBooster\%YYYYMMDD%\Client\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\FixesFullVersion\WebSite\XBooster\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\FixesFullVersion\WebSite\XBooster\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\FixesFullVersion\WebSite\XBooster\%YYYYMMDD%\Client\Modules\Runtime\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.BusinessProcess.Web\BusinessProcess" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\FixesFullVersion\WebSite\XBooster\%YYYYMMDD%\Client\Modules\BusinessProcess\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Rules.Web\VR_Rules" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\FixesFullVersion\WebSite\XBooster\%YYYYMMDD%\Client\Modules\VR_Rules\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\FixesFullVersion\WebSite\XBooster\%YYYYMMDD%\Client\Modules\VR_GenericData\"

xcopy "C:\TFS\Xbooster\Code\CDRComparison.Web\CDRComparison" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\FixesFullVersion\WebSite\XBooster\%YYYYMMDD%\Client\Modules\CDRComparison\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.ExcelConversion.Web\ExcelConversion" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\FixesFullVersion\WebSite\XBooster\%YYYYMMDD%\Client\Modules\ExcelConversion\"
xcopy "C:\TFS\Xbooster\Code\XBooster.PriceListConversion.Web\XBooster_PriceListConversion" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\FixesFullVersion\WebSite\XBooster\%YYYYMMDD%\Client\Modules\XBooster_PriceListConversion\"