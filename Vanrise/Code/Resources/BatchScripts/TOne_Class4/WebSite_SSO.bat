ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO SSO WebSite
xcopy "C:\Publish\SSOSite" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\SSOSite\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\TOne_Class4\list-of-excluded-files-SSO.txt
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\SSOSite\Web.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\SSOSite\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\SSOSite\%YYYYMMDD%\Web.config.exclude" "Web.config"

