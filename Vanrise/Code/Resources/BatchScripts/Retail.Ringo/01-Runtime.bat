ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Retail.Ringo Runtime
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release\Retail.Ringo*" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\Retail.Ringo\Runtime\%YYYYMMDD%\"
del /s /q /f																					"\\192.168.110.185\Fixes\WebSite\Retail.Ringo\Runtime\%YYYYMMDD%\*.pdb"