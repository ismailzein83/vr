ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO ExcelService
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\TOneExcelServices\TOneExcelServices\TOneExcelServicesSetup\Release\setup.exe" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Common\ExcelService\%YYYYMMDD%\"
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\TOneExcelServices\TOneExcelServices\TOneExcelServicesSetup\Release\TOneExcelServicesSetup.msi" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Common\ExcelService\%YYYYMMDD%\"

xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TOne.ExcelServices\bin\Release\TOne.ExcelServices.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Common\ExcelService\%YYYYMMDD%\WebSiteBin\"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TOne.ExcelServices\bin\Release\TOne.ExcelServices.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Common\ExcelService\%YYYYMMDD%\WebSiteBin\"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\LWDataSetRemotingLibrary\bin\Release\LWDataSetRemotingLibrary.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Common\ExcelService\%YYYYMMDD%\WebSiteBin\"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\LWDataSetRemotingLibrary\bin\Release\LWDataSetRemotingLibrary.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Common\ExcelService\%YYYYMMDD%\WebSiteBin\"