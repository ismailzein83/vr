ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k
ECHO.
ECHO Retail.Ringo WebSite

xcopy "C:\TFS\Retail\Code\Retail.Web\Web.config.Ringo.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\Retail.Ringo\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\Retail.Ringo\%YYYYMMDD%\Web.config.Ringo.exclude" "Web.Update.Ringo.config"

xcopy "C:\TFS\Retail\Code\Retail.Ringo.Business\bin\Release\Retail.Ringo.Business.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Ringo\%YYYYMMDD%\bin\"
xcopy "C:\TFS\Retail\Code\Retail.Ringo.Data\bin\Release\Retail.Ringo.Data.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Ringo\%YYYYMMDD%\bin\"
xcopy "C:\TFS\Retail\Code\Retail.Ringo.Data.SQL\bin\Release\Retail.Ringo.Data.SQL.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Ringo\%YYYYMMDD%\bin\"
xcopy "C:\TFS\Retail\Code\Retail.Ringo.Entities\bin\Release\Retail.Ringo.Entities.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Ringo\%YYYYMMDD%\bin\"
xcopy "C:\TFS\Retail\Code\Retail.BusinessEntity.RingoExtensions\bin\Release\Retail.Ringo.MainExtensions.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Ringo\%YYYYMMDD%\bin\"
xcopy "C:\TFS\Retail\Code\Retail.Ringo.Web\bin\Retail.Ringo.Web.dll" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Ringo\%YYYYMMDD%\bin\"

xcopy "C:\TFS\Retail\Code\Retail.Ringo.Web\Retail_Ringo" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Ringo\%YYYYMMDD%\Client\Modules\Retail_Ringo\"