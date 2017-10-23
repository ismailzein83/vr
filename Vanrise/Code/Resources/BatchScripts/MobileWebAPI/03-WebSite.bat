ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO MobileWebAPI WebSite

xcopy "C:\Publish\TOne.Web.Online" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\TOne.Web.Online\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\MobileWebAPI\list-of-excluded-files.txt
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\TOne.Web.Online\%YYYYMMDD%\Bin\"