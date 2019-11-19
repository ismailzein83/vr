ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CloudXCataleya WebSite

::by default load flat theme
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-flat-logoonheader.png" /S /R /y /v /i /z /Q													"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\Images\logoonheader.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-flat-login.png" /S /R /y /v /i /z /Q															"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\Images\login.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-flat-iconheader.ico" /S /R /y /v /i /z /Q														"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\Images\iconheader.ico*"
::default theme
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-logoonheader.png" /S /R /y /v /i /z /Q															"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\Default-theme\Images\logoonheader.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-login.png" /S /R /y /v /i /z /Q																"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\Default-theme\Images\login.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-iconheader.ico" /S /R /y /v /i /z /Q															"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\Default-theme\Images\iconheader.ico*"
::flat theme
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-flat-logoonheader.png" /S /R /y /v /i /z /Q													"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\Flat-theme\Images\logoonheader.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-flat-login.png" /S /R /y /v /i /z /Q															"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\Flat-theme\Images\login.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\cloudXPoint-flat-iconheader.ico" /S /R /y /v /i /z /Q														"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\Flat-theme\Images\iconheader.ico*"

::xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.RouteSync.Cataleya\bin\Release\TOne.WhS.RouteSync.Cataleya.dll" /y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\Bin\"

xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.BusinessEntity.Web\WhS_BusinessEntity\Views\Switch\SingleSwitchManagement.html" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\Client\Modules\WhS_BusinessEntity\Views\Switch\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.BusinessEntity.Web\WhS_BusinessEntity\Views\Switch\SingleSwitchManagementController.js" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\Client\Modules\WhS_BusinessEntity\Views\Switch\"