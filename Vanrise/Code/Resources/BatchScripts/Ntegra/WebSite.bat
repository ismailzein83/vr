ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Ntegra WebSite
xcopy "C:\TOne_Solution\WebHelperLibrary\bin\Release\WebHelperLibrary.dll" /y /v /z /i /Q /R  "C:\Publish\TOne\bin\"
xcopy "C:\TOne_Solution\WebHelperLibrary\bin\Release\WebHelperLibrary.pdb" /y /v /z /i /Q /R  "C:\Publish\TOne\bin\"
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\RoutePoolServiceHost\RoutingServiceContruct\bin\Release\RoutingServiceContruct.dll" /y /v /z /i /Q /R  "C:\Publish\TOne\bin\"
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\RoutePoolServiceHost\RoutingServiceContruct\bin\Release\RoutingServiceContruct.pdb" /y /v /z /i /Q /R  "C:\Publish\TOne\bin\"
xcopy "C:\Publish\TOne" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\Ntegra\WebSite\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Ntegra\list-of-excluded-files.txt
rename "\\192.168.110.185\Fixes\Ntegra\WebSite\%YYYYMMDD%\Ntegrafavicon.ico" "favicon.ico"
xcopy "C:\TOne_Solution\WebSite\Web.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\Ntegra\WebSite\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\Ntegra\WebSite\%YYYYMMDD%\Web.config.exclude" "Web.config"



