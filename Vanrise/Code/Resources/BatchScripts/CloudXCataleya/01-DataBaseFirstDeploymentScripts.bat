ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CloudXCataleya DataBase First Deployment Scripts
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\VRJSON_PostScripts\Common\VR_GoogleAnalytics.json" /y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\CloudXPoint.PostDeployment.sql" 	/y /v /z /i /Q /R					"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\common.Countries.sql" 			/y /v /z /i /Q /R					"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\TOneWhS_BE.CodeGroup.sql" 		/y /v /z /i /Q /R					"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\common.Currency.sql" 			/y /v /z /i /Q /R					"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Configuration.txt" 				/y /v /z /i /Q /R					"\\192.168.110.185\Fixes\WebSite\CloudXCataleya\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"