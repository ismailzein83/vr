ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Inspkt.C4 WebSite
xcopy "C:\TFS\Retail\Code\RecordAnalysis.Web\RecordAnalysis" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\Inspkt\%YYYYMMDD%\Client\Modules\RecordAnalysis\"
xcopy "C:\Publish\Retail\bin\RecordAnalysis*" /S /E /R /y /v /i /z /Q												"\\192.168.110.185\Fixes\WebSite\Inspkt\%YYYYMMDD%\Bin\"
del /s /q /f																										"\\192.168.110.185\Fixes\WebSite\Inspkt\%YYYYMMDD%\Bin\*.pdb"