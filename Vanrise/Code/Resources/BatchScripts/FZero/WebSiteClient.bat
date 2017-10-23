ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO FZero WebSite Client
xcopy "C:\Publish\FZeroClient" /S /E /R /y /v /i /z /Q 							"\\192.168.110.185\Fixes\Fzero\WebSite\Client\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\FZero\list-of-excluded-files-Client.txt
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R 	"\\192.168.110.185\Fixes\Fzero\WebSite\Client\%YYYYMMDD%\Bin\"