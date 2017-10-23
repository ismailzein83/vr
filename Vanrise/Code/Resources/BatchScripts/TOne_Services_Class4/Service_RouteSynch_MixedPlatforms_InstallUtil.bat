ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO RouteSynch Service InstallUtil
xcopy "C:\TFS\TOne.Projects3.5\TOneServices\ToneRouteSynch\RoutingTaskServiceInstaller\bin\Release" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\RouteSynch\InstallUtil\%YYYYMMDD%\"

xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.Services\bin\Release\TABS.Plugins.Services.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\RouteSynch\InstallUtil\%YYYYMMDD%\WebSiteBin\"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.Services\bin\Release\TABS.Plugins.Services.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Services\Class4\RouteSynch\InstallUtil\%YYYYMMDD%\WebSiteBin\"