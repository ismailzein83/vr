ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Routing Service
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\RoutePoolServiceHost\OCSHost\bin\Release" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\RoutingService\%YYYYMMDD%\"

xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.Services\bin\Release\TABS.Plugins.Services.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\RoutingService\%YYYYMMDD%\WebSiteBin\"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.Services\bin\Release\TABS.Plugins.Services.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\RoutingService\%YYYYMMDD%\WebSiteBin\"

xcopy "C:\TFS\TOne.Projects3.5\TOneServices\RoutePoolServiceHost\OCSHost\bin\Release\RoutingServiceContruct.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\RoutingService\%YYYYMMDD%\WebSiteBin\"
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\RoutePoolServiceHost\OCSHost\bin\Release\RoutingServiceContruct.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\RoutingService\%YYYYMMDD%\WebSiteBin\"
