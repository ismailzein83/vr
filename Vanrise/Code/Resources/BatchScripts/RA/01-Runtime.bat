ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO RA Runtime
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release" /y /v /z /i /Q /R			"\\192.168.110.185\Fixes\WebSite\RA\Runtime\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\RA\runtime-list-of-excluded-files.txt
xcopy "C:\TFS\Retail\Code\Retail.Runtime\App.config.RA.exclude" /y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\RA\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.dll" /y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\RA\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\RA\Runtime\%YYYYMMDD%\"

rename "\\192.168.110.185\Fixes\WebSite\RA\Runtime\%YYYYMMDD%\App.config.RA.exclude" "RevenueAssurance.Runtime.exe.config"
rename "\\192.168.110.185\Fixes\WebSite\RA\Runtime\%YYYYMMDD%\Retail.Runtime.exe" "RevenueAssurance.Runtime.exe"
del /s /q /f "\\192.168.110.185\Fixes\WebSite\RA\Runtime\%YYYYMMDD%\*.pdb"
