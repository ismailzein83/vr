ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO SecurityEssentials DLL
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\SecurityEssentials\bin\Release\SecurityEssentials.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\PartialFixes\TOne\%YYYYMMDD%\Bin\"
::xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\SecurityEssentials\bin\Release\SecurityEssentials.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\PartialFixes\TOne\%YYYYMMDD%\Bin\"
