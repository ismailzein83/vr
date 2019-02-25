ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Inspkt Runtime
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release" /y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Inspkt\Runtime\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\RetailSource\runtime-list-of-excluded-files.txt
xcopy "C:\TFS\Retail\Code\Retail.Runtime\App.config.RecordAnalysis.exclude" /y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\Inspkt\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R								"\\192.168.110.185\Fixes\WebSite\Inspkt\Runtime\%YYYYMMDD%\"
rename																										"\\192.168.110.185\Fixes\WebSite\Inspkt\Runtime\%YYYYMMDD%\App.config.RecordAnalysis.exclude"	"Inspkt.Runtime.exe.config"
rename																										"\\192.168.110.185\Fixes\WebSite\Inspkt\Runtime\%YYYYMMDD%\Retail.Runtime.exe"					"Inspkt.Runtime.exe"
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release\RecordAnalysis*" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\Inspkt\Runtime\%YYYYMMDD%\"
del /s /q /f																								"\\192.168.110.185\Fixes\WebSite\Inspkt\Runtime\%YYYYMMDD%\*.pdb"