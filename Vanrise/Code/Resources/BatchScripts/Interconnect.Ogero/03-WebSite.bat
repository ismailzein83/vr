ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Interconnect.Ogero WebSite

xcopy "C:\Publish\Retail\bin\Retail.Ogero*" /S /E /R /y /v /i /z /Q											"\\192.168.110.185\Fixes\WebSite\Interconnect.Ogero\%YYYYMMDD%\Bin\"