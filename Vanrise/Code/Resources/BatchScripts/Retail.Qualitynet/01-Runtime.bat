ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Retail.Qualitynet Runtime
::xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release\Retail.Zajil*" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\Retail.Qualitynet\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release\Retail.Qualitynet*" /S /E /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\Retail.Qualitynet\Runtime\%YYYYMMDD%\"
::xcopy "C:\TFS\Retail\Code\Retail.Runtime\bin\x64\Release\Retail.Teles*" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\Retail.Qualitynet\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.QualityNet.Web\Retail_QualityNet\Reports\*.rdlc"	/S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\Retail.Qualitynet\Runtime\%YYYYMMDD%\Modules\Retail_QualityNet\Reports"
del /s /q /f																								"\\192.168.110.185\Fixes\WebSite\Retail.Qualitynet\Runtime\%YYYYMMDD%\*.pdb"