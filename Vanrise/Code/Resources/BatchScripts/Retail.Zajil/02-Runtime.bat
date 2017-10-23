ECHO OFF
::Make Directory from current date as YYYYMMDD

Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO.
ECHO Retail.Zajil Runtime
xcopy "C:\TFS\Retail\Code\Retail.Zajil.Business\bin\Release\Retail.Zajil.Business.dll" 				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Zajil.Data\bin\Release\Retail.Zajil.Data.dll" 						/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Zajil.Data.SQL\bin\Release\Retail.Zajil.Data.SQL.dll" 				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Zajil.Entities\bin\Release\Retail.Zajil.Entities.dll" 				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Zajil.MainExtensions\bin\Release\Retail.Zajil.MainExtensions.dll" 	/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Zajil.Web\Retail_Zajil\Reports"									/S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\Runtime\%YYYYMMDD%\Modules\Retail_Zajil\Reports" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Retail.Zajil\runtime-list-of-excluded-files.txt

xcopy "C:\TFS\Retail\Code\Retail.Teles.Business\bin\Release\Retail.Teles.Business.dll" 				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\Runtime\%YYYYMMDD%\"
xcopy "C:\TFS\Retail\Code\Retail.Teles.Entities\bin\Release\Retail.Teles.Entities.dll" 				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\Runtime\%YYYYMMDD%\"