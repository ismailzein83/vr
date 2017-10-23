ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Interconnect WebSite

xcopy "C:\Publish\Interconnect" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Interconnect\list-of-excluded-files.txt
xcopy "C:\TFS\Interconnect\Code\Interconnect.Web\Web.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\Web.config.exclude" "Web.config"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\Bin\"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\Client\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\Client\Modules\Runtime\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Integration.Web\Integration" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\Client\Modules\Integration\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\Client\Modules\VR_GenericData\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.Queueing.Web\Queueing" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\Client\Modules\Queueing\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Rules.Web\VR_Rules" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\Client\Modules\VR_Rules\"

xcopy "C:\TFS\Interconnect\Code\InterConnect.BusinessEntity.Web\InterConnect_BusinessEntity" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\Client\Modules\InterConnect_BusinessEntity\"
