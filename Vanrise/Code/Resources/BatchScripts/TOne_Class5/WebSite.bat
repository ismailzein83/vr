ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO TOneRetail WebSite
xcopy "C:\Publish\TOne_Retail" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\TOne_Retail\%YYYYMMDD%\"
xcopy "C:\TOneRetail\WebSite\Dependencies\Telerik.Web.Design.dll" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\TOne_Retail\%YYYYMMDD%\bin\"
