ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO ExternalAccess WebSite
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\WebHelperLibrary\bin\Release\WebHelperLibrary.dll" /y /v /z /i /Q /R  "C:\Publish\Traffic_Monitor\bin\"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\WebHelperLibrary\bin\Release\WebHelperLibrary.pdb" /y /v /z /i /Q /R  "C:\Publish\Traffic_Monitor\bin\"

xcopy "C:\Publish\Traffic_Monitor" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\ExternalAccess\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\ExternalAccess\list-of-excluded-files-ExternalAccess.txt
xcopy "C:\TFS\TOne.Projects3.5\TrafficMonitor\WebSite\Web.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\ExternalAccess\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\ExternalAccess\%YYYYMMDD%\Web.config.exclude" "Web.config"
rename "\\192.168.110.185\Fixes\WebSite\ExternalAccess\%YYYYMMDD%\TOnefavicon.ico" "favicon.ico"