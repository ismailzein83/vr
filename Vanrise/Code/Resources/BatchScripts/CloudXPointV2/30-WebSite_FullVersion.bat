ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CloudXPointV2 WebSite Full Version

xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Web.config.CloudXPoint.exclude" /y /v /z /i /Q /R	"\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\"
rename "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Web.config.CloudXPoint.exclude" "Web.Update.CloudXPoint.config"

xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\images\cloudXPoint-logoonheader.png" /S /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\images\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\images\cloudXPoint-login.png" /S /R /y /v /i /z /Q		"\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\images\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\images\cloudXPoint-iconheader.ico" /S /R /y /v /i /z /Q	"\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\images\"

rename "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\images\cloudXPoint-logoonheader.png" "logoonheader.png"
rename "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\images\cloudXPoint-login.png" "login.png"
rename "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\images\cloudXPoint-iconheader.ico" "iconheader.ico"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Themes\theme-extented-cloudXPoint.css.exclude" /S /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Themes\"
rename "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Themes\theme-extented-cloudXPoint.css.exclude" "theme-extented.css"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\cloudXPoint-01.png" /S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Images\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\cloudXPoint-02.png" /S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Images\"
rename "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Images\cloudXPoint-01.png" "01.png"
rename "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Images\cloudXPoint-02.png" "02.png"

xcopy "C:\TFS\NetworkProvisioning\Code\NP.IVSwitch.Web\NP_IVSwitch" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Modules\NP_IVSwitch\"
xcopy "C:\TFS\NetworkProvisioning\Code\Output" /S /E /R /y /v /i /z /Q														"\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.RouteSync.IVSwitch\bin\Release\TOne.WhS.RouteSync.IVSwitch.dll" /y /v /z /i /Q /R "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Bin\"

xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.BusinessEntity.Web\WhS_BusinessEntity\Views\Switch\SingleSwitchManagement.html" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Modules\WhS_BusinessEntity\Views\Switch\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.BusinessEntity.Web\WhS_BusinessEntity\Views\Switch\SingleSwitchManagementController.js" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Modules\WhS_BusinessEntity\Views\Switch\"

::DEL /F /Q "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Modules\WhS_BusinessEntity\Views\Switch\SwitchManagement.html"
::DEL /F /Q "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Modules\WhS_BusinessEntity\Views\Switch\SwitchManagementController.js"

::xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.RouteSync.Web\WhS_RouteSync" /S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Modules\WhS_RouteSync\"
::RD /S /Q "\\192.168.110.185\FixesFullVersion\WebSite\CloudXPointV2\%YYYYMMDD%\Client\Modules\WhS_RouteSync\Views\RouteSyncDefinition\"