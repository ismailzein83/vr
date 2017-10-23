ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Retail.MultiNet Runtime

xcopy "C:\TFS\Retail\Code\Retail.MultiNet.Business\bin\Release\Retail.MultiNet.Business.dll"		/y /v /z /i /Q /R  		"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.MultiNet.Entities\bin\Release\Retail.MultiNet.Entities.dll" 		/y /v /z /i /Q /R  		"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.MultiNet.APIEntities\bin\Release\Retail.MultiNet.APIEntities.dll" 	/y /v /z /i /Q /R  		"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.MultiNet.Web\Retail_MultiNet\Reports"								/S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\Runtime\%YYYYMMDD%\Modules\Retail_MultiNet\Reports" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Retail.MultiNet\runtime-list-of-excluded-files.txt

xcopy "C:\TFS\Retail\Code\Retail.Teles.Business\bin\Release\Retail.Teles.Business.dll" 				/y /v /z /i /Q /R  		"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Teles.Entities\bin\Release\Retail.Teles.Entities.dll" 				/y /v /z /i /Q /R  		"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\Runtime\%YYYYMMDD%\"