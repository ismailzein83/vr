ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Component-NetworkRental WebSite

xcopy "C:\TFS\Retail\Code\Retail.Demo.Web\Retail_Demo" /S /E /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\Component-NetworkRental\%YYYYMMDD%\Client\Modules\Retail_Demo\"
xcopy "C:\TFS\Retail\Code\Retail.Demo.Web\bin\Retail.Demo*" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\Component-NetworkRental\%YYYYMMDD%\Bin\"
del /s /q /f																		"\\192.168.110.185\Fixes\WebSite\Component-NetworkRental\%YYYYMMDD%\Bin\*.pdb"