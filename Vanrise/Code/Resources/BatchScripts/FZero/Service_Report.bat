ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Report
xcopy "C:\TFS\FZero\Vanrise.Fzero.Services.Report\bin\Release" /y /v /z /i /Q /R  	"\\192.168.110.185\Fixes\Fzero\Services\Report\%YYYYMMDD%\"
xcopy "C:\TFS\FZero\Vanrise.Fzero.Services.Report\Reports" /y /v /z /i /Q /R  		"\\192.168.110.185\Fixes\Fzero\Services\Report\%YYYYMMDD%\Reports\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R 		"\\192.168.110.185\Fixes\Fzero\Services\Report\%YYYYMMDD%\"