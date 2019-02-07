ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO FMSV1 WebSite

xcopy "C:\Publish\Retail" /S /E /R /y /v /i /z /Q																	"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\RetailSource\list-of-excluded-files.txt
xcopy "C:\TFS\Retail\Code\Retail.Web\Web.config.RA.exclude" /y /v /z /i /Q /R										"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\"
xcopy "C:\TFS\CDRAnalysis\CDRAnalysis.Web\images\logoonheader.png" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\Images\"
xcopy "C:\TFS\CDRAnalysis\CDRAnalysis.Web\images\login.png" /S /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\Images\"
xcopy "C:\TFS\CDRAnalysis\CDRAnalysis.Web\images\iconheader.ico" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\Images\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\menu-icons" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\Images\menu-icons\"
rename																												"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\Web.config.RA.exclude" "Web.config"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R										"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q												"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\Client\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\Client-list-of-excluded-files.txt
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.ExcelConversion.Web\ExcelConversion" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\Client\Modules\ExcelConversion\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\Client\Modules\VR_GenericData\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\Client\Modules\Analytic\"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\FMSV1\%YYYYMMDD%\Bin\*.config"