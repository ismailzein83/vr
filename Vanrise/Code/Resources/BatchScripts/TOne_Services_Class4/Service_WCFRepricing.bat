ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO TOne.WCFRepricing
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\TOne.WCFRepricing\Tone.RepricingWCFSetup\Release\setup.exe" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\WCFRepricing\%YYYYMMDD%\"
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\TOne.WCFRepricing\Tone.RepricingWCFSetup\Release\Tone.RepricingWCFSetup.msi" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\WCFRepricing\%YYYYMMDD%\"

xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.Services\bin\Release\TABS.Plugins.Services.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\WCFRepricing\%YYYYMMDD%\WebSiteBin\"
::xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.Services\bin\Release\TABS.Plugins.Services.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\WCFRepricing\%YYYYMMDD%\WebSiteBin\"