ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Code Preperation InstallUtil
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\CodePreperation\CodePreperationService.root\CodePreperationService\CodePreperationHost\bin\Release" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CodePreperation\InstallUtil\%YYYYMMDD%\"

xcopy "C:\TFS\TOne.Projects3.5\TOneServices\CodePreperation\CodePreperationService.root\CodePreperationService\CodePreperationWCF\Dependencies\SecurityEssentials.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CodePreperation\InstallUtil\%YYYYMMDD%\"
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\CodePreperation\CodePreperationService.root\CodePreperationService\CodePreperationWCF\Dependencies\WebHelperLibrary.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CodePreperation\InstallUtil\%YYYYMMDD%\"
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\CodePreperation\CodePreperationService.root\CodePreperationService\xlsgen.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CodePreperation\InstallUtil\%YYYYMMDD%\"
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\CodePreperation\CodePreperationService.root\CodePreperationService\xlsgen.license.lic" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CodePreperation\InstallUtil\%YYYYMMDD%\"


xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.CodePreperation\bin\Release\TABS.Plugins.CodePreperation.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CodePreperation\InstallUtil\%YYYYMMDD%\WebSiteBin\"
::xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.CodePreperation\bin\Release\TABS.Plugins.CodePreperation.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CodePreperation\InstallUtil\%YYYYMMDD%\WebSiteBin\"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.StateBackUp\bin\Release\TABS.StateBackUp.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CodePreperation\InstallUtil\%YYYYMMDD%\WebSiteBin\"
::xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.StateBackUp\bin\Release\TABS.StateBackUp.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CodePreperation\InstallUtil\%YYYYMMDD%\WebSiteBin\"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TOne.ExcelServices\bin\Release\TOne.ExcelServices.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CodePreperation\InstallUtil\%YYYYMMDD%\WebSiteBin\"
::xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TOne.ExcelServices\bin\Release\TOne.ExcelServices.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CodePreperation\InstallUtil\%YYYYMMDD%\WebSiteBin\"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\LWDataSetRemotingLibrary\bin\Release\LWDataSetRemotingLibrary.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CodePreperation\InstallUtil\%YYYYMMDD%\WebSiteBin\"
::xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\LWDataSetRemotingLibrary\bin\Release\LWDataSetRemotingLibrary.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CodePreperation\InstallUtil\%YYYYMMDD%\WebSiteBin\"

xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.Services\bin\Release\TABS.Plugins.Services.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CodePreperation\InstallUtil\%YYYYMMDD%\WebSiteBin\"
::xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.Services\bin\Release\TABS.Plugins.Services.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\CodePreperation\InstallUtil\%YYYYMMDD%\WebSiteBin\"