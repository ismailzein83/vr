ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO BQR
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.NewBQR\bin\Release\TABS.Plugins.BQR.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Plugins\BQR\%YYYYMMDD%\"
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TABS.Plugins.NewBQR\bin\Release\TABS.Plugins.BQR.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Plugins\BQR\%YYYYMMDD%\"
