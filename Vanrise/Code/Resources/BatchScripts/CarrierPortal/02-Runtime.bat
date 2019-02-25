ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO CarrierPortal Runtime
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.DevRuntime\bin\x64\Release" /y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\CarrierPortal\Runtime\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\CarrierPortalSource\runtime-list-of-excluded-files.txt
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.DevRuntime\app.config.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\CarrierPortal\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R					"\\192.168.110.185\Fixes\WebSite\CarrierPortal\Runtime\%YYYYMMDD%\"
rename																							"\\192.168.110.185\Fixes\WebSite\CarrierPortal\Runtime\%YYYYMMDD%\app.config.exclude"			"CarrierPortal.Runtime.exe.config"
rename																							"\\192.168.110.185\Fixes\WebSite\CarrierPortal\Runtime\%YYYYMMDD%\CarrierPortal.DevRuntime.exe" "CarrierPortal.Runtime.exe"
del /s /q /f																					"\\192.168.110.185\Fixes\WebSite\CarrierPortal\Runtime\%YYYYMMDD%\*.pdb"