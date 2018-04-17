ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k
ECHO.
ECHO Retail.Zajil WebSite

xcopy "C:\TFS\Retail\Code\Retail.Web\Web.config.Zajil.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Web.config.Zajil.exclude" "Web.Update.Zajil.config"

xcopy "C:\TFS\Retail\Code\Retail.Zajil.Business\bin\Release\Retail.Zajil.Business.dll" 				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Retail\Code\Retail.Zajil.Data\bin\Release\Retail.Zajil.Data.dll" 						/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Retail\Code\Retail.Zajil.Data.SQL\bin\Release\Retail.Zajil.Data.SQL.dll" 				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Retail\Code\Retail.Zajil.Entities\bin\Release\Retail.Zajil.Entities.dll" 				/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Retail\Code\Retail.Zajil.MainExtensions\bin\Release\Retail.Zajil.MainExtensions.dll" 	/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Retail\Code\Retail.Zajil.Web\bin\Retail.Zajil.Web.dll" 								/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\bin\"
xcopy "C:\TFS\Retail\Code\Retail.Zajil.Web\Retail_Zajil" 											/S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Client\Modules\Retail_Zajil\"

xcopy "C:\TFS\Vanrise\Code\Vanrise.InvToAccBalanceRelation.Web\VR_InvToAccBalanceRelation" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Client\Modules\VR_InvToAccBalanceRelation\"


xcopy "C:\TFS\Retail\Code\Retail.Teles.Business\bin\Release\Retail.Teles.Business.dll" 	/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Retail\Code\Retail.Teles.Entities\bin\Release\Retail.Teles.Entities.dll" 	/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Retail\Code\Retail.Teles.Web\bin\Retail.Teles.Web.dll" 					/y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Retail\Code\Retail.Teles.Web\Retail_Teles" 								/S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Zajil\%YYYYMMDD%\Client\Modules\Retail_Teles\"