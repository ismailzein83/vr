ECHO OFF
::this tool fix Invoice Total amount Issue starting version 20190318
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO ResolveInvoiceIssue
xcopy "C:\TFS\TOneV2\Code\TOneV2\ResolveInvoiceIssue\bin\Release" /y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\TOneV2\ResolveInvoiceIssue\%YYYYMMDD%\"
del /s /q /f																			"\\192.168.110.185\Fixes\WebSite\TOneV2\ResolveInvoiceIssue\%YYYYMMDD%\*.pdb"
PAUSE
EXIT