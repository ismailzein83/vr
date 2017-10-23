ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO TOneBI.DataAccessLayer DLL
xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TOneBI.DataAccessLayer\bin\Release\TOneBI.DataAccessLayer.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\PartialFixes\BI\%YYYYMMDD%\Bin\"
::xcopy "C:\TFS\TOne.Projects3.5\TOne_Solution\TOneBI.DataAccessLayer\bin\Release\TOneBI.DataAccessLayer.pdb" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\PartialFixes\BI\%YYYYMMDD%\Bin\"
