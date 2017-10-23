ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO TOne WebSite
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\WebHelperLibrary\bin\Release\WebHelperLibrary.dll" /y /v /z /i /Q /R  "C:\Publish\TOne\bin\"
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\RoutePoolServiceHost\RoutingServiceContruct\bin\Release\RoutingServiceContruct.dll" /y /v /z /i /Q /R  "C:\Publish\TOne\bin\"
xcopy "C:\Publish\TOne" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\TOne\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\TOne_Class4\list-of-excluded-files.txt
rename "\\192.168.110.185\Fixes\WebSite\TOne\%YYYYMMDD%\TOnefavicon.ico" "favicon.ico"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\WebSite\Web.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\TOne\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\TOne\%YYYYMMDD%\Web.config.exclude" "Web.config"

del /s /q /f "\\192.168.110.185\Fixes\WebSite\TOne\bin\%YYYYMMDD%\*.pdb"

if not exist "\\192.168.110.185\Fixes\WebSite\TOne\%YYYYMMDD%\DataBaseFirstDeploymentScripts\DBsStructure\" mkdir "\\192.168.110.185\Fixes\WebSite\TOne\%YYYYMMDD%\DataBaseFirstDeploymentScripts\DBsStructure\"

ECHO TOne Create Standard DBs Structure files
start /b /w /D"C:\TFS\Vanrise\Code\Vanrise.HelperTools\bin\Release" Vanrise.HelperTools.exe "DBs" "TOne"