ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO DemoProject WebSite

xcopy "C:\Publish\DemoProject" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\DemoProject\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\DemoProject\list-of-excluded-files.txt
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\DemoProject\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\DemoProject\%YYYYMMDD%\Client\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\DemoProject\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\DemoProject\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Demo.project\Code\Demo.Module.Web\Demo_Module" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\DemoProject\%YYYYMMDD%\Client\Modules\Demo_Module\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\DemoProject\%YYYYMMDD%\Client\Modules\Runtime\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Integration.Web\Integration" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\DemoProject\%YYYYMMDD%\Client\Modules\Integration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Common.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\DemoProject\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneTransaction\Common.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\DemoProject\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Transaction\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneLogging\Script.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\DemoProject\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Logging\"
