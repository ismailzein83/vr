ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k
ECHO.
ECHO Retail.Ringo WebSite

xcopy "C:\TFS\Retail\Code\Retail.Web\Web.config.Ringo.exclude" /y /v /z /i /Q /R	"\\192.168.110.185\Fixes\WebSite\Retail.Ringo\%YYYYMMDD%\"
rename																				"\\192.168.110.185\Fixes\WebSite\Retail.Ringo\%YYYYMMDD%\Web.config.Ringo.exclude" "Web.Update.Ringo.config"
xcopy "C:\TFS\Retail\Code\Retail.Ringo.Web\Retail_Ringo" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\Retail.Ringo\%YYYYMMDD%\Client\Modules\Retail_Ringo\"
xcopy "C:\Publish\Retail\bin\Retail.Ringo*" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Retail.Ringo\%YYYYMMDD%\Bin"
del /s /q /f																		"\\192.168.110.185\Fixes\WebSite\Retail.Ringo\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																		"\\192.168.110.185\Fixes\WebSite\Retail.Ringo\%YYYYMMDD%\Bin\*.config"