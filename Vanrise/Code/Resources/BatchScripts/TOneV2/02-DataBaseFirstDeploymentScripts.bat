ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO TOneV2 DataBase First Deployment Scripts

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\BusinessProcess.PostDeployment.json" /y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Common.PostDeployment.sql" /y /v /z /i /Q /R					"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\ExcelConversion.PostDeployment.sql" /y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Generic.PostDeployment.json" /y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Integration.PostDeployment.sql" /y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Queueing.PostDeployment.sql" /y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Runtime.PostDeployment.sql" /y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Security.PostDeployment.sql" /y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\TOne.PostDeployment.sql" /y /v /z /i /Q /R					"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\VR_Rules.json" /y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Analytic.PostDeployment.json" /y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Deals.PostDeployment.sql" /y /v /z /i /Q /R					"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Reprocess.PostDeployment.sql" /y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Invoice.PostDeployment.sql" /y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\VR_Notification.json" /y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\VR_AccountBalance.json" /y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\DataAnalysis.PostDeployment.sql" /y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\VR_BEBridge.json" 	/y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\DataAnalysis.TOne.PostDeployment.sql" /y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\WhS_AccountBalance.PostDeployment.sql" /y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\TOneV1Transition.PostDeployment.sql" /y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\TOne.DefaultData.PostDeployment.sql" /y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
::xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Sama.FlaggedService.sql" /y /v /z /i /Q /R					"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\TOne.AutoImportSPL.PostDeployment.sql" /y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\TOne.AccountManager.PostDeployment.sql" /y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\TOne_Ogero.PostDeployment.sql" /y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\TOne_Portal.PostDeployment.sql" /y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\TOne_Namibia.PostDeployment.sql" /y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"

start /b /w /D"C:\TFS\Vanrise\Code\Vanrise.HelperTools\bin\Release" Vanrise.HelperTools.exe "Enumerations" "TOneV2"

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Configuration.txt" /y /v /z /i /Q /R							"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneTransaction\BusinessProcess.PostDeployment.sql" /y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Transaction\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneLogging\Script.PostDeployment.sql" /y /v /z /i /Q /R						"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Logging\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneQueueing\Script.PostDeployment.sql" /y /v /z /i /Q /R						"\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Queueing\"

::if not exist "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\DBsStructure\" mkdir "\\192.168.110.185\Fixes\WebSite\TOneV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\DBsStructure\"