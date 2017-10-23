ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Import
xcopy "C:\TFS\FZero\Vanrise.Fzero.Services.CDRImport\bin\Release" /y /v /z /i /Q /R  	"\\192.168.110.185\Fixes\Fzero\Services\CDRImport\%YYYYMMDD%\"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R 			"\\192.168.110.185\Fixes\Fzero\Services\CDRImport\%YYYYMMDD%\"
