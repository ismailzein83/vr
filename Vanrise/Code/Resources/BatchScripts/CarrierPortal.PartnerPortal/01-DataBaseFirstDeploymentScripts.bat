ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CarrierPortal.PartnerPortal DataBase First Deployment Scripts

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Analytic.PostDeployment.sql" 							/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Generic.PostDeployment.sql" 								/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\CarrierPortal.RetailPortal.PostDeployment.sql" 			/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\CarrierPortal.RetailPortal_Centrex.PostDeployment.sql" 	/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\CarrierPortal.RetailPortal_Zajil.PostDeployment.sql" 	/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Configuration.txt" 										/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"