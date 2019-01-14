ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO NetworkRental Runtime
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release" /y /v /z /i /Q /R												"\\192.168.110.185\Fixes\WebSite\NetworkRental\Runtime\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\RetailSource\runtime-list-of-excluded-files.txt
xcopy "C:\TFS\Retail\Code\Retail.Runtime\App.config.NR.exclude" /y /v /z /i /Q /R										"\\192.168.110.185\Fixes\WebSite\NetworkRental\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.dll" /y /v /z /i /Q /R											"\\192.168.110.185\Fixes\WebSite\NetworkRental\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R											"\\192.168.110.185\Fixes\WebSite\NetworkRental\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Demo.Web\Retail_Demo\Elements\NetworkRental\Reports\*.rdlc" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\NetworkRental\Runtime\%YYYYMMDD%\Modules\Retail_Demo\Elements\NetworkRental\Reports"
xcopy "C:\Publish\Retail\bin\Retail.Demo*" /S /E /R /y /v /i /z /Q														"\\192.168.110.185\Fixes\WebSite\NetworkRental\Runtime\%YYYYMMDD%\"

rename			"\\192.168.110.185\Fixes\WebSite\NetworkRental\Runtime\%YYYYMMDD%\App.config.NR.exclude" "NetworkRental.Runtime.exe.config"
rename			"\\192.168.110.185\Fixes\WebSite\NetworkRental\Runtime\%YYYYMMDD%\Retail.Runtime.exe" "NetworkRental.Runtime.exe"
del /s /q /f /Q	"\\192.168.110.185\Fixes\WebSite\NetworkRental\Runtime\%YYYYMMDD%\*.pdb" "\\192.168.110.185\Fixes\WebSite\NetworkRental\Runtime\%YYYYMMDD%\Retail.*.Web.*"