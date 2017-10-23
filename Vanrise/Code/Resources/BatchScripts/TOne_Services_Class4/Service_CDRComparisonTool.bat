ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO CDRComparisonsTool
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TOne_CDRComparisonTool\CDRComparisonsTool\bin\Release" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CDRComparisonsTool\%YYYYMMDD%\"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TOne_CDRComparisonTool\CDRComparisonsTool\DownloadSample" /S /E /R /y /v /i /z /Q  "\\192.168.110.185\Fixes\Services\Class4\CDRComparisonsTool\%YYYYMMDD%\DownloadSample\"
xcopy "C:\Publish\CDRCommparisonToolLicense" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CDRCommparisonToolWebLicense\%YYYYMMDD%\"
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\CDRComparisonToolLicense\ToneLicense\License.Generator\bin\Release" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CDRComparisonsToolLicenseGenerator\%YYYYMMDD%\"

 


