ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Component-CCT Runtime
xcopy "C:\TFS\Xbooster\Code\XBooster.Runtime\bin\x64\Release\CDRComparison.*" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\Component-CCT\Runtime\%YYYYMMDD%\"
del /s /q /f																					"\\192.168.110.185\Fixes\WebSite\Component-CCT\Runtime\%YYYYMMDD%\*.pdb"