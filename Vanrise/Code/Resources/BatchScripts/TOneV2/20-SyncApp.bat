ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
::ECHO TOneV2 SyncApp
::xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.DBSync.App\bin\Release" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\TOneV2\SyncApp\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\TOneV2\SyncApp-list-of-excluded-files.txt