ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k
ECHO.
ECHO Retail.Billing DataBase First Deployment Scripts

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\BusinessProcess.PostDeployment.json" 		/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Common.PostDeployment.sql" 				/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\ExcelConversion.PostDeployment.sql" 		/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Generic.PostDeployment.json" 				/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Integration.PostDeployment.sql" 			/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Queueing.PostDeployment.sql" 			/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Runtime.PostDeployment.sql" 				/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Security.PostDeployment.sql" 			/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\VR_Rules.json" 			/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Analytic.PostDeployment.json" 			/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\VR_AccountBalance.json" 	/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\VR_Notification.json" 		/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\VR_BEBridge.json" 			/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\VR_NumberingPlan.json" 	/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Reprocess.PostDeployment.sql" 			/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Invoice.PostDeployment.sql" 				/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\DataAnalysis.PostDeployment.sql" 		/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Retail.PostDeployment.sql" 				/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Retail_Invoice.PostDeployment.sql" 		/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\CommonEntities_BankEntities.PostDeployment.json" 	/y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\CommonEntities_Address.PostDeployment.json" 	/y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\CommonEntities_PersonProfile.PostDeployment.json" 	/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Retail_RetailBilling.PostDeployment.json" 	/y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Configuration.txt" 						/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneTransaction\BusinessProcess.PostDeployment.sql" 		/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Transaction\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneLogging\Script.PostDeployment.sql" 					/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Logging\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneQueueing\Script.PostDeployment.sql" 					/y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Retail.Billing\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Queueing\"
start /b /w /D"C:\TFS\Vanrise\Code\Vanrise.HelperTools\bin\Release" Vanrise.HelperTools.exe "Enumerations" "Retail.Billing"

