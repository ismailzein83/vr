ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO TOneV2 DataBase First Deployment Scripts

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\BusinessProcess.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Common.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\ExcelConversion.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Generic.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Integration.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Queueing.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Runtime.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Security.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\TOne.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VR_Rules.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Analytic.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Deals.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Reprocess.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Invoice.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VR_Notification.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VR_AccountBalance.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\DataAnalysis.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\DataAnalysis.TOne.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\WhS_AccountBalance.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\TOneV1Transition.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\TOne.DefaultData.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
::xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Sama.FlaggedService.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Configuration.txt" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneTransaction\BusinessProcess.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Transaction\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneLogging\Script.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Logging\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneQueueing\Script.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Queueing\"

::if not exist "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\DBsStructure\" mkdir "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\DBsStructure\"