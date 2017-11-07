ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO ExcelService InstallUtil
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\TOneExcelServices\TOneExcelServices\TOneExcelServiceHost\bin\Release" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Common\ExcelService\InstallUtil\%YYYYMMDD%\"

xcopy "C:\TFS\TOne.Projects3.5\TOneServices\TOneExcelServices\TOneExcelServices\TOneExcelServices\Dependencies\TABS.Addons.Utilities.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Common\ExcelService\InstallUtil\%YYYYMMDD%\"
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\TOneExcelServices\TOneExcelServices\TOneExcelServices\Dependencies\SecurityEssentials.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Common\ExcelService\InstallUtil\%YYYYMMDD%\"

xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TOne.ExcelServices\bin\Release\TOne.ExcelServices.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Common\ExcelService\InstallUtil\%YYYYMMDD%\WebSiteBin\"
::xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TOne.ExcelServices\bin\Release\TOne.ExcelServices.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Common\ExcelService\InstallUtil\%YYYYMMDD%\WebSiteBin\"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\LWDataSetRemotingLibrary\bin\Release\LWDataSetRemotingLibrary.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Common\ExcelService\InstallUtil\%YYYYMMDD%\WebSiteBin\"
::xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\LWDataSetRemotingLibrary\bin\Release\LWDataSetRemotingLibrary.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Common\ExcelService\InstallUtil\%YYYYMMDD%\WebSiteBin\"