ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Inspkt.C4 Runtime
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release\RecordAnalysis*" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\Inspkt.C4\Runtime\%YYYYMMDD%\"
del /s /q /f																								"\\192.168.110.185\Fixes\WebSite\Inspkt.C4\Runtime\%YYYYMMDD%\*.pdb"