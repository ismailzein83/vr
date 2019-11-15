ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO NetworkInventory Runtime
::xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release" /y /v /z /i /Q /R						"\\192.168.110.185\Fixes\WebSite\NetworkInventory\Runtime\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\RetailSource\runtime-list-of-excluded-files.txt
::xcopy "C:\TFS\Retail\Code\Retail.Runtime\App.config.NetworkInventory.exclude" /y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\NetworkInventory\Runtime\%YYYYMMDD%\"
::rename																							"\\192.168.110.185\Fixes\WebSite\NetworkInventory\Runtime\%YYYYMMDD%\App.config.NetworkInventory.exclude" "NetworkInventory.Runtime.exe.config"
::rename																							"\\192.168.110.185\Fixes\WebSite\NetworkInventory\Runtime\%YYYYMMDD%\Retail.Runtime.exe" "NetworkInventory.Runtime.exe"
::xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R					"\\192.168.110.185\Fixes\WebSite\NetworkInventory\Runtime\%YYYYMMDD%\"
::del /s /q /f																					"\\192.168.110.185\Fixes\WebSite\NetworkInventory\Runtime\%YYYYMMDD%\Retail.*"
::xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release\Retail.NIM*" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\NetworkInventory\Runtime\%YYYYMMDD%\"
::del /s /q /f																					"\\192.168.110.185\Fixes\WebSite\NetworkInventory\Runtime\%YYYYMMDD%\*.pdb"
::del /s /q /f																					"\\192.168.110.185\Fixes\WebSite\NetworkInventory\Runtime\%YYYYMMDD%\Vanrise.Integration.*"
::del /s /q /f																					"\\192.168.110.185\Fixes\WebSite\NetworkInventory\Runtime\%YYYYMMDD%\Vanrise.Reprocess.*"
::del /s /q /f																					"\\192.168.110.185\Fixes\WebSite\NetworkInventory\Runtime\%YYYYMMDD%\Vanrise.AccountManager.*"
::del /s /q /f																					"\\192.168.110.185\Fixes\WebSite\NetworkInventory\Runtime\%YYYYMMDD%\Vanrise.AccountBalance.*"
::del /s /q /f																					"\\192.168.110.185\Fixes\WebSite\NetworkInventory\Runtime\%YYYYMMDD%\Vanrise.Invoice.*"
::del /s /q /f																					"\\192.168.110.185\Fixes\WebSite\NetworkInventory\Runtime\%YYYYMMDD%\Vanrise.InvToAccBalanceRelation.*"
::del /s /q /f																					"\\192.168.110.185\Fixes\WebSite\NetworkInventory\Runtime\%YYYYMMDD%\Vanrise.NumberingPlan.*"

