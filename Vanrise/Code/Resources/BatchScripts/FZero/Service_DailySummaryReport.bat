ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO DailySummaryReport
xcopy "C:\TFS\FZero\Vanrise.Fzero.Services.DailySummaryReport\bin\Release" /y /v /z /i /Q /R 	"\\192.168.110.185\Fixes\Fzero\Services\DailySummaryReport\%YYYYMMDD%\"
xcopy "C:\TFS\FZero\Vanrise.Fzero.Services.DailySummaryReport\Reports" /y /v /z /i /Q /R  		"\\192.168.110.185\Fixes\Fzero\Services\DailySummaryReport\%YYYYMMDD%\Reports\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R 					"\\192.168.110.185\Fixes\Fzero\Services\DailySummaryReport\%YYYYMMDD%\"