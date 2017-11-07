ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Bilateral
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.BilateralAgreement\bin\Release\TABS.Plugins.BilateralAgreement.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Plugins\Bilateral\%YYYYMMDD%\"
::xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.BilateralAgreement\bin\Release\TABS.Plugins.BilateralAgreement.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Plugins\Bilateral\%YYYYMMDD%\"

xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.BilateralAgreement\bin\Release\TABS.Addons.Utilities.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Plugins\Bilateral\%YYYYMMDD%\WebSiteBin\"
::xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.BilateralAgreement\bin\Release\TABS.Addons.Utilities.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Plugins\Bilateral\%YYYYMMDD%\WebSiteBin\"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.BilateralAgreement\bin\Release\Newtonsoft.Json.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Plugins\Bilateral\%YYYYMMDD%\WebSiteBin\"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Routing\bin\Release\TABS.Routing.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Plugins\Bilateral\%YYYYMMDD%\WebSiteBin\"
::xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Routing\bin\Release\TABS.Routing.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Plugins\Bilateral\%YYYYMMDD%\WebSiteBin\"
