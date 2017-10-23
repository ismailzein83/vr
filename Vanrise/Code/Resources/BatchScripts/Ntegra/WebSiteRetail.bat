ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO NtegraRetail WebSite
xcopy "C:\Publish\Ntegra_Retail" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\Ntegra\WebSiteRetail\%YYYYMMDD%\"  /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Ntegra\C5-list-of-excluded-files.txt
xcopy "C:\TFS\TOne.Projects3.5\NtegraRetail\WebSite\Web.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\Ntegra\WebSiteRetail\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\Ntegra\WebSiteRetail\%YYYYMMDD%\Web.config.exclude" "Web.config"



