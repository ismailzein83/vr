ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO ClientReport
xcopy "C:\TFS\FZero\Vanrise.Fzero.Services.ClientReport\bin\Release" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\Fzero\Services\ClientReport\%YYYYMMDD%\"
xcopy "C:\TFS\FZero\Vanrise.Fzero.Services.ClientReport\Reports" /y /v /z /i /Q /R  	"\\192.168.110.185\Fixes\Fzero\Services\ClientReport\%YYYYMMDD%\Reports\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R 			"\\192.168.110.185\Fixes\Fzero\Services\ClientReport\%YYYYMMDD%\"
