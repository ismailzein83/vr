ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO CentralLog Runtime
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release" /y /v /z /i /Q /R					"\\192.168.110.185\Fixes\WebSite\CentralLog\Runtime\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\RetailSource\runtime-list-of-excluded-files.txt
xcopy "C:\TFS\Retail\Code\Retail.Runtime\App.config.CentralLog.exclud" /y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\CentralLog\Runtime\%YYYYMMDD%\CentralLog.Runtime.exe.config*"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\CentralLog\Runtime\%YYYYMMDD%\"
del /s /q /f																				"\\192.168.110.185\Fixes\WebSite\CentralLog\Runtime\%YYYYMMDD%\*.pdb"
del /s /q /f																				"\\192.168.110.185\Fixes\WebSite\CentralLog\Runtime\%YYYYMMDD%\Retail.*.dll"
del /s /q /f																				"\\192.168.110.185\Fixes\WebSite\CentralLog\Runtime\%YYYYMMDD%\Vanrise.AccountManager.*"
del /s /q /f																				"\\192.168.110.185\Fixes\WebSite\CentralLog\Runtime\%YYYYMMDD%\Vanrise.AccountBalance.*"
del /s /q /f																				"\\192.168.110.185\Fixes\WebSite\CentralLog\Runtime\%YYYYMMDD%\Vanrise.Invoice.*"
del /s /q /f																				"\\192.168.110.185\Fixes\WebSite\CentralLog\Runtime\%YYYYMMDD%\Vanrise.InvToAccBalanceRelation.*"
del /s /q /f																				"\\192.168.110.185\Fixes\WebSite\CentralLog\Runtime\%YYYYMMDD%\Vanrise.NumberingPlan.*"
del /s /q /f																				"\\192.168.110.185\Fixes\WebSite\CentralLog\Runtime\%YYYYMMDD%\Vanrise.BEBridge.*"
rename																						"\\192.168.110.185\Fixes\WebSite\CentralLog\Runtime\%YYYYMMDD%\Retail.Runtime.exe"		"CentralLog.Runtime.exe"