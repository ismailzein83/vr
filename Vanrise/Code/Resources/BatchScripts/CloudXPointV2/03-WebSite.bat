ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CloudXPointV2 WebSite

xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Web.config.CloudXPoint.exclud" /y /v /z /i /Q /R																	"\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Web.config*"
::by default load flat theme
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-flat-logoonheader.png" /S /R /y /v /i /z /Q													"\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Images\logoonheader.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-flat-login.png" /S /R /y /v /i /z /Q															"\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Images\login.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-flat-iconheader.ico" /S /R /y /v /i /z /Q														"\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Images\iconheader.ico*"
::default theme
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-logoonheader.png" /S /R /y /v /i /z /Q															"\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Default-theme\Images\logoonheader.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-login.png" /S /R /y /v /i /z /Q																"\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Default-theme\Images\login.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-iconheader.ico" /S /R /y /v /i /z /Q															"\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Default-theme\Images\iconheader.ico*"
::flat theme
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-flat-logoonheader.png" /S /R /y /v /i /z /Q													"\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Flat-theme\Images\logoonheader.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-flat-login.png" /S /R /y /v /i /z /Q															"\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Flat-theme\Images\login.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-flat-iconheader.ico" /S /R /y /v /i /z /Q														"\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Flat-theme\Images\iconheader.ico*"
xcopy "C:\TFS\NetworkProvisioning\Code\NP.IVSwitch.Web\NP_IVSwitch" /S /E /R /y /v /i /z /Q																	"\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Modules\NP_IVSwitch\"
xcopy "C:\TFS\NetworkProvisioning\Code\Output" /S /E /R /y /v /i /z /Q																						"\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.RouteSync.IVSwitch\bin\Release\TOne.WhS.RouteSync.IVSwitch.dll" /y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.BusinessEntity.Web\WhS_BusinessEntity\Views\Switch\SingleSwitchManagement.html" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Modules\WhS_BusinessEntity\Views\Switch\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.BusinessEntity.Web\WhS_BusinessEntity\Views\Switch\SingleSwitchManagementController.js" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Modules\WhS_BusinessEntity\Views\Switch\"

::DEL /F /Q "\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Modules\WhS_BusinessEntity\Views\Switch\SwitchManagement.html"
::DEL /F /Q "\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Modules\WhS_BusinessEntity\Views\Switch\SwitchManagementController.js"

::xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.RouteSync.Web\WhS_RouteSync" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Modules\WhS_RouteSync\"
::RD /S /Q "\\192.168.110.185\Fixes\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Modules\WhS_RouteSync\Views\RouteSyncDefinition\"
