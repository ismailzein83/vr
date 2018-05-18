ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Interconnect Runtime
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release" /y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\Interconnect\Runtime\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Interconnect\runtime-list-of-excluded-files.txt
xcopy "C:\TFS\Retail\Code\Retail.Runtime\App.config.Interconnect.exclude" /y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\Interconnect\Runtime\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\Interconnect\Runtime\%YYYYMMDD%\App.config.Interconnect.exclude" "Interconnect.Runtime.exe.config"
rename "\\192.168.110.185\Fixes\WebSite\Interconnect\Runtime\%YYYYMMDD%\Retail.Runtime.exe" "Interconnect.Runtime.exe"

xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.dll" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\Interconnect\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\Interconnect\Runtime\%YYYYMMDD%\"

del /s /q /f "\\192.168.110.185\Fixes\WebSite\Interconnect\Runtime\%YYYYMMDD%\*.pdb"
