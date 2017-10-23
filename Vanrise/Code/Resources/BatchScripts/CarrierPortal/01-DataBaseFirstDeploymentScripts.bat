ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CarrierPortal DataBase First Deployment Scripts

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Security.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Common.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Runtime.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\CarrierPortal.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VR_Rules.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Configuration.txt" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneLogging\Script.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Logging\"

::if not exist "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\DataBaseFirstDeploymentScripts\DBsStructure\" mkdir "\\192.168.110.185\Fixes\WebSite\CarrierPortal\%YYYYMMDD%\DataBaseFirstDeploymentScripts\DBsStructure\"