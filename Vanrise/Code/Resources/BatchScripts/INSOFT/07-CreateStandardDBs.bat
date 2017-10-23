ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

if not exist "\\192.168.110.185\Fixes\WebSite\INSOFT\%YYYYMMDD%\DataBaseFirstDeploymentScripts\DBsStructure\" mkdir "\\192.168.110.185\Fixes\WebSite\INSOFT\%YYYYMMDD%\DataBaseFirstDeploymentScripts\DBsStructure\"

ECHO INSOFT Create Standard DBs Structure files
start /b /w /D"C:\TFS\Vanrise\Code\Vanrise.HelperTools\bin\Release" Vanrise.HelperTools.exe "DBs" "INSOFT"