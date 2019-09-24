ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k
ECHO.
ECHO CentralLog DataBase First Deployment Scripts

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_BusinessProcess.json" 	/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Common.PostDeployment.sql" 							/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_ExcelConversion.json" 	/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Generic.json" 			/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Integration.json" 		/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Queueing.json" 			/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Runtime.json" 			/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Security.json" 			/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Rules.json" 			/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Analytic.json" 			/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Notification.json" 		/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Reprocess.json" 		/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Invoice.json" 			/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_DataAnalysis.json" 		/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_CentralLog.json" 		/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Configuration.txt" 									/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneTransaction\BusinessProcess.PostDeployment.sql" 					/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Transaction\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneLogging\Script.PostDeployment.sql" 								/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Logging\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneQueueing\Script.PostDeployment.sql" 								/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CentralLog\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Queueing\"

