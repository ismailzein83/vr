ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Component-CCT Create Standard DBs Structure files
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\DBsStructure.txt" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Component-CCT\%YYYYMMDD%\DataBaseFirstDeploymentScripts\DBsStructure\"
start /b /w /D"C:\TFS\Vanrise\Code\Vanrise.HelperTools\bin\Release" Vanrise.HelperTools.exe "DBs" "Component-CCT"