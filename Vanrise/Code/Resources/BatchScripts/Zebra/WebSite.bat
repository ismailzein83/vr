ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Zebra WebSite
xcopy "C:\Publish\Zebra" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\Zebra\WebSite\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Zebra\list-of-excluded-files.txt
xcopy "C:\TFS\TOne.Projects3.5\ZebraPortalNew94\Website\Web.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\Zebra\WebSite\%YYYYMMDD%\"
xcopy "C:\TFS\TOne.Projects3.5\ZebraPortalNew94\Website\Dependencies" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\Zebra\WebSite\%YYYYMMDD%\Bin\"
rename "\\192.168.110.185\Fixes\Zebra\WebSite\%YYYYMMDD%\Web.config.exclude" "Web.config"



