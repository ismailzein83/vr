ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO BIV2 DataBase First Deployment Scripts
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Common.PostDeployment.sql" 					/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\BIV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\ExcelConversion.PostDeployment.sql" 			/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\BIV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Security.PostDeployment.sql" 				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\BIV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\BI.PostDeployemnt.sql" 						/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\BIV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\BI.StandAloneApplication.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\BIV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Configuration.txt" 							/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\BIV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneLogging\Script.PostDeployment.sql" 						/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\BIV2\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Logging\"

