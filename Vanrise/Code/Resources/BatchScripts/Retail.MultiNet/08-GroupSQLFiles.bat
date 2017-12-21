ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Retail.MultiNet Group SQL Scripts
start /b /w /D"C:\TFS\Vanrise\Code\Vanrise.HelperTools\bin\Release" Vanrise.HelperTools.exe "GRPSQLOverridden" "Retail.MultiNet"

rename "\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\%YYYYMMDD%\DataBaseFirstDeploymentScripts\DBsStructure\DBsStructure_Overridden.sql" "DBsStructure.sql"