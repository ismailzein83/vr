ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO BI WebSite
xcopy "C:\Publish\BI" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\BI\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\BI\list-of-excluded-files.txt
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\WebSiteBI\Dependencies" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\BI\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\WebSiteBI\Web.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\BI\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\BI\%YYYYMMDD%\Web.config.exclude" "Web.config"




