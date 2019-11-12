ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k
ECHO.
ECHO Interconnect DataBase First Deployment Scripts

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Common.json" /y /v /z /i /Q /R							"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Currency.json" /y /v /z /i /Q /R						"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Bank.json" /y /v /z /i /Q /R							"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Company.json" /y /v /z /i /Q /R							"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Countries.json" /y /v /z /i /Q /R						"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_RateType.json" /y /v /z /i /Q /R						"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_BusinessProcess.json" 		/y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_ExcelConversion.json" 		/y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Generic.json" 				/y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Integration.json" 			/y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Queueing.json" 			/y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Runtime.json" 				/y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Security.json" 			/y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Rules.json" 			/y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Analytic.json" 			/y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_AccountBalance.json" 	/y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Notification.json" 		/y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_BEBridge.json" 			/y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_NumberingPlan.json" 	/y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Reprocess.json" 			/y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Invoice.json" 				/y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Retail.PostDeployment.sql" 				/y /v /z /i /Q /R							"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Retail_Invoice.PostDeployment.sql" 		/y /v /z /i /Q /R							"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Interconnect\Interconnect.json" 				/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Interconnect\Voice\Interconnect_Voice.json" 	/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Interconnect\Data\Interconnect_Data.json" 	/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Interconnect\SMS\Interconnect_SMS.json" 		/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_MobileNetwork.json"		/y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Configuration.txt" 						/y /v /z /i /Q /R							"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneTransaction\BusinessProcess.PostDeployment.sql" 		/y /v /z /i /Q /R							"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Transaction\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneLogging\Script.PostDeployment.sql" 					/y /v /z /i /Q /R							"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Logging\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneQueueing\Script.PostDeployment.sql" 					/y /v /z /i /Q /R							"\\192.168.110.185\Fixes\WebSite\Interconnect\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Queueing\"
start /b /w /D"C:\TFS\Vanrise\Code\Vanrise.HelperTools\bin\Release" Vanrise.HelperTools.exe "Enumerations" "Interconnect"



