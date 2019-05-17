ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k
ECHO.
ECHO NetworkInventory DataBase First Deployment Scripts

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\BusinessProcess.PostDeployment.sql" 		/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\NetworkInventory\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Common.PostDeployment.sql" 				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\NetworkInventory\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\ExcelConversion.PostDeployment.sql" 		/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\NetworkInventory\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Generic.PostDeployment.sql" 				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\NetworkInventory\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Runtime.PostDeployment.sql" 				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\NetworkInventory\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Security.PostDeployment.sql" 			/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\NetworkInventory\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VR_Rules.PostDeployment.sql" 			/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\NetworkInventory\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Analytic.PostDeployment.sql" 			/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\NetworkInventory\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VR_Notification.PostDeployment.sql" 		/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\NetworkInventory\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VR_BEBridge.PostDeployment.sql" 			/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\NetworkInventory\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\DataAnalysis.PostDeployment.sql" 		/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\NetworkInventory\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\NetworkInventory.PostDeployment.sql" 	/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\NetworkInventory\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Configuration.txt" 						/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\NetworkInventory\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneTransaction\BusinessProcess.PostDeployment.sql" 		/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\NetworkInventory\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Transaction\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneLogging\Script.PostDeployment.sql" 					/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\NetworkInventory\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Logging\"
start /b /w /D"C:\TFS\Vanrise\Code\Vanrise.HelperTools\bin\Release" Vanrise.HelperTools.exe "Enumerations" "NetworkInventory"

