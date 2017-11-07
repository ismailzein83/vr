ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO PremiumNumbersManagement
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.PremiumNumbersManagement\bin\Release\TABS.Plugins.PremiumNumbersManagement.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Plugins\PremiumNumbersManagement\%YYYYMMDD%\"
::xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.PremiumNumbersManagement\bin\Release\TABS.Plugins.PremiumNumbersManagement.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Plugins\PremiumNumbersManagement\%YYYYMMDD%\"
