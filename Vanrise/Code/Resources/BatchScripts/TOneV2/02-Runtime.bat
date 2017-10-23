ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO TOneV2 Runtime
xcopy "C:\TFS\TOneV2\Code\TOneV2\TestRuntime\bin\x64\Release" 							/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\Runtime\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\TOneV2\runtime-list-of-excluded-files.txt
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.Invoice.Web\WhS_Invoice\Reports"				/S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\TOneV2\Runtime\%YYYYMMDD%\Reports" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\TOneV2\runtime-list-of-excluded-files.txt

xcopy "C:\TFS\TOneV2\Code\TOneV2\TestRuntime\app.config.exclude" 						/y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\TOneV2\Runtime\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\TOneV2\Runtime\%YYYYMMDD%\app.config.exclude" "TOne.WhS.Runtime.exe.config"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" 							/y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\TOneV2\Runtime\%YYYYMMDD%\"



del /s /q /f "\\192.168.110.185\Fixes\WebSite\TOneV2\Runtime\%YYYYMMDD%\*.pdb"