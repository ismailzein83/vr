ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO TestCallAnalysis Runtime
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release" /y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\TestCallAnalysis\Runtime\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\RetailSource\runtime-list-of-excluded-files.txt
xcopy "C:\TFS\Retail\Code\Retail.Runtime\App.config.TestCallAnalysis.exclude" /y /v /z /i /Q /R				"\\192.168.110.185\Fixes\WebSite\TestCallAnalysis\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R								"\\192.168.110.185\Fixes\WebSite\TestCallAnalysis\Runtime\%YYYYMMDD%\"
rename																										"\\192.168.110.185\Fixes\WebSite\TestCallAnalysis\Runtime\%YYYYMMDD%\App.config.TestCallAnalysis.exclude"	"TestCallAnalysis.Runtime.exe.config"
rename																										"\\192.168.110.185\Fixes\WebSite\TestCallAnalysis\Runtime\%YYYYMMDD%\Retail.Runtime.exe"					"TestCallAnalysis.Runtime.exe"
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release\TestCallAnalysis.*" /S /E /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\TestCallAnalysis\Runtime\%YYYYMMDD%\"
del /s /q /f																								"\\192.168.110.185\Fixes\WebSite\TestCallAnalysis\Runtime\%YYYYMMDD%\Vanrise.AccountManager.*"
del /s /q /f																								"\\192.168.110.185\Fixes\WebSite\TestCallAnalysis\Runtime\%YYYYMMDD%\Vanrise.AccountBalance.*"
del /s /q /f																								"\\192.168.110.185\Fixes\WebSite\TestCallAnalysis\Runtime\%YYYYMMDD%\Vanrise.Invoice.*"
del /s /q /f																								"\\192.168.110.185\Fixes\WebSite\TestCallAnalysis\Runtime\%YYYYMMDD%\Vanrise.InvToAccBalanceRelation.*"
del /s /q /f																								"\\192.168.110.185\Fixes\WebSite\TestCallAnalysis\Runtime\%YYYYMMDD%\Vanrise.NumberingPlan.*"
del /s /q /f																								"\\192.168.110.185\Fixes\WebSite\TestCallAnalysis\Runtime\%YYYYMMDD%\*.pdb"