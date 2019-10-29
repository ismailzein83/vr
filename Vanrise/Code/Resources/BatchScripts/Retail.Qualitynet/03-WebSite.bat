ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k
ECHO.
ECHO Retail.Qualitynet WebSite

::xcopy "C:\TFS\Retail\Code\Retail.Zajil.Web\Retail_Zajil" 	/S /E /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\Retail.Qualitynet\%YYYYMMDD%\Client\Modules\Retail_Zajil\"
::xcopy "C:\Publish\Retail\bin\Retail.Zajil*" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\Retail.Qualitynet\%YYYYMMDD%\Bin"

xcopy "C:\TFS\Retail\Code\Retail.QualityNet.Web\Retail_QualityNet" 	/S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Qualitynet\%YYYYMMDD%\Client\Modules\Retail_QualityNet\"
xcopy "C:\Publish\Retail\bin\Retail.Qualitynet*" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Retail.Qualitynet\%YYYYMMDD%\Bin"

::xcopy "C:\TFS\Retail\Code\Retail.Teles.Web\Retail_Teles" 	/S /E /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\Retail.Qualitynet\%YYYYMMDD%\Client\Modules\Retail_Teles\"
::xcopy "C:\Publish\Retail\bin\Retail.Teles*" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\Retail.Qualitynet\%YYYYMMDD%\Bin"

del /s /q /f																				"\\192.168.110.185\Fixes\WebSite\Retail.Qualitynet\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																				"\\192.168.110.185\Fixes\WebSite\Retail.Qualitynet\%YYYYMMDD%\Bin\*.config"