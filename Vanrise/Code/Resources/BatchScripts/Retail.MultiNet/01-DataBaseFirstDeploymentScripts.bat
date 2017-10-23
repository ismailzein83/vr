ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k
ECHO.
ECHO Retail.MultiNet DataBase First Deployment Scripts

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Retail_Centrex.PostDeployment.sql" 				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Retail_Teles.PostDeployment1.sql" 				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Retail_Multinet.PostDeployment.sql"				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Retail_Multinet_Overridden.PostDeployment.sql"	/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Retail_Cost.PostDeployment.sql"					/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Configuration.txt" 								/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"