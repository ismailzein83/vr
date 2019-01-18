ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k
ECHO.
ECHO Retail.Zajil WebSite

xcopy "C:\TFS\Retail\Code\Retail.Web\Web.config.Zajil.exclude" /y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\"
rename																				"\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Web.config.Zajil.exclude"	"Web.Update.Zajil.config"
xcopy "C:\TFS\Retail\Code\Retail.Zajil.Web\Retail_Zajil" 	/S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Client\Modules\Retail_Zajil\"
xcopy "C:\Publish\Retail\bin\Retail.Zajil*" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Bin"
xcopy "C:\TFS\Retail\Code\Retail.Teles.Web\Retail_Teles" 	/S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Client\Modules\Retail_Teles\"
xcopy "C:\Publish\Retail\bin\Retail.Teles*" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Bin"
del /s /q /f																		"\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																		"\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Bin\*.config"