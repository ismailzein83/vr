ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k
ECHO.
ECHO Retail.MultiNet WebSite

xcopy "C:\TFS\Retail\Code\Retail.Web\Web.config.MultiNet.exclude" /y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\%YYYYMMDD%\"
rename																					"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\%YYYYMMDD%\Web.config.MultiNet.exclude" "Web.Update.MultiNet.config"
xcopy "C:\TFS\Retail\Code\Retail.MultiNet.Web\Retail_MultiNet"	/S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\%YYYYMMDD%\Client\Modules\Retail_MultiNet\"
xcopy "C:\Publish\Retail\bin\Retail.MultiNet*" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Retail\Code\Retail.Teles.Web\Retail_Teles"	/S /E /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\%YYYYMMDD%\Client\Modules\Retail_Teles\"
xcopy "C:\Publish\Retail\bin\Retail.Teles*" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\%YYYYMMDD%\Bin"
del /s /q /f																			"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																			"\\192.168.110.185\Fixes\WebSite\Retail.MultiNet\%YYYYMMDD%\Bin\*.config"