ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO NtegraRetail ExternalAccess WebSite
xcopy "C:\Publish\Ntegra_RetailExternalAccess" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\Ntegra\ExternalAccess\%YYYYMMDD%\"  /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Ntegra\C5ExternalAccess-list-of-excluded-files.txt
xcopy "C:\TFS\TOne.Projects3.5\NtegraRetail\NtegraC5ExternalAccess\Web.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\Ntegra\ExternalAccessRetail\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\Ntegra\ExternalAccessRetail\%YYYYMMDD%\Web.config.exclude" "Web.config"



