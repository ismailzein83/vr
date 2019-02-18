ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO TOneV2.Jazz Runtime
xcopy "C:\TFS\TOneV2\Code\TOneV2\TestRuntime\bin\x64\Release\TOne.WhS.Jazz*" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\TOneV2.Jazz\Runtime\%YYYYMMDD%\"
del /s /q /f																							"\\192.168.110.185\Fixes\WebSite\TOneV2.Jazz\Runtime\%YYYYMMDD%\*.pdb"