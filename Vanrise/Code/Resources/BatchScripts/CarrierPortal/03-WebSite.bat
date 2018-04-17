ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CarrierPortal WebSite

xcopy "C:\Publish\CarrierPortal" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\CarrierPortal\list-of-excluded-files.txt
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Web.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Web.config.exclude" "Web.config"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Bin\"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\Modules\Runtime\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Rules.Web\VR_Rules" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\Client\Modules\VR_Rules\"