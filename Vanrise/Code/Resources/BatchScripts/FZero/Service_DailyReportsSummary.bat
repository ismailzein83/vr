ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO DailyReportsSummary
xcopy "C:\TFS\FZero\Vanrise.Fzero.Services.DailyReportsSummary\bin\Release" /y /v /z /i /Q /R 	"\\192.168.110.185\Fixes\Fzero\Services\DailyReportsSummary\%YYYYMMDD%\"
xcopy "C:\TFS\FZero\Vanrise.Fzero.Services.DailyReportsSummary\Reports" /y /v /z /i /Q /R  		"\\192.168.110.185\Fixes\Fzero\Services\DailyReportsSummary\%YYYYMMDD%\Reports\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R 					"\\192.168.110.185\Fixes\Fzero\Services\DailyReportsSummary\%YYYYMMDD%\"