ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Retail.Zajil Runtime
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release\Retail.Zajil*" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\Retail.Zajil\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release\Retail.Teles*" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\Retail.Zajil\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Zajil.Web\Retail_Zajil\Reports\*.rdlc"	/S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\Retail.Zajil\Runtime\%YYYYMMDD%\Modules\Retail_Zajil\Reports"
del /s /q /f																					"\\192.168.110.185\Fixes\WebSite\Retail.Zajil\Runtime\%YYYYMMDD%\*.pdb"