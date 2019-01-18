ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO CarrierPortal.PartnerPortal Runtime
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.DevRuntime\bin\x64\Release\PartnerPortal*" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\CP.Partner\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.DevRuntime\bin\x64\Release\Retail*" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\CP.Partner\Runtime\%YYYYMMDD%\"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\CP.Partner\Runtime\%YYYYMMDD%\Retail.Ringo.*"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\CP.Partner\Runtime\%YYYYMMDD%\Retail.MultiNet.*"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\CP.Partner\Runtime\%YYYYMMDD%\*.pdb"