ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k
ECHO.
ECHO TelecomCRM DataBase First Deployment Scripts

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_BusinessProcess.json" 				/y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Common.json" /y /v /z /i /Q /R								"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
::xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Localizalion.json" 					/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_ExcelConversion.json" 					/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Generic.json" 							/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Runtime.json" 							/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Security.json" 							/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Rules.json" 							/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Analytic.json" 							/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_Notification.json" 						/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_BEBridge.json" 							/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_DataAnalysis.json" 						/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\TelecomCRM.PostDeployment.sql" 										/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\CommonEntities\VR_CommonEntities.json" 	/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\CommonEntities\VR_BankEntities.json" 		/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\CommonEntities\VR_Address.json" 			/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\CommonEntities\VR_PersonProfile.json" 		/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Retail_CRMForFixedOperator.PostDeployment.json" 	/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Configuration.txt" 													/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneTransaction\BusinessProcess.PostDeployment.sql" 									/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Transaction\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneLogging\Script.PostDeployment.sql" 												/y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\TelecomCRM\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Logging\"
start /b /w /D"C:\TFS\Vanrise\Code\Vanrise.HelperTools\bin\Release" Vanrise.HelperTools.exe "Enumerations" "TelecomCRM"

