ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO TOne.SMS Runtime
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release" /y /v /z /i /Q /R							"\\192.168.110.185\Fixes\WebSite\TOne.SMS\Runtime\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\TOne.SMS\runtime-list-of-excluded-files.txt
xcopy "C:\TFS\Retail\Code\Retail.Runtime\App.config.SMS-WHS.exclude" /y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\TOne.SMS\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.dll" /y /v /z /i /Q /R						"\\192.168.110.185\Fixes\WebSite\TOne.SMS\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R						"\\192.168.110.185\Fixes\WebSite\TOne.SMS\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Demo.Web\Retail_Demo\Elements\SMS\Reports" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\TOne.SMS\Runtime\%YYYYMMDD%\Modules\Retail_Demo\Elements\SMS\Reports" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\TOne.SMS\runtime-list-of-excluded-files.txt
rename "\\192.168.110.185\Fixes\WebSite\TOne.SMS\Runtime\%YYYYMMDD%\App.config.SMS-WHS.exclude" "TOne.SMS.Runtime.exe.config"
rename "\\192.168.110.185\Fixes\WebSite\TOne.SMS\Runtime\%YYYYMMDD%\Retail.Runtime.exe" "TOne.SMS.Runtime.exe"
del /s /q /f "\\192.168.110.185\Fixes\WebSite\TOne.SMS\Runtime\%YYYYMMDD%\*.pdb"
