ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Retail.MultiNet WebSite Full Version

xcopy "C:\TFS\Retail\Code\Retail.Web\Web.config.MultiNet.exclude" /y /v /z /i /Q /R											"\\192.168.110.185\FixesFullVersion\WebSite\Retail.MultiNet\%YYYYMMDD%\"
rename "\\192.168.110.185\FixesFullVersion\WebSite\Retail.MultiNet\%YYYYMMDD%\Web.config.MultiNet.exclude" "Web.Update.MultiNet.config"

xcopy "C:\TFS\Retail\Code\Retail.MultiNet.Business\bin\Release\Retail.MultiNet.Business.dll" 		/y /v /z /i /Q /R  		"\\192.168.110.185\FixesFullVersion\WebSite\Retail.MultiNet\%YYYYMMDD%\bin\"
xcopy "C:\TFS\Retail\Code\Retail.MultiNet.Entities\bin\Release\Retail.MultiNet.Entities.dll" 		/y /v /z /i /Q /R  		"\\192.168.110.185\FixesFullVersion\WebSite\Retail.MultiNet\%YYYYMMDD%\bin\"
xcopy "C:\TFS\Retail\Code\Retail.MultiNet.Web\bin\Retail.MultiNet.Web.dll"							/y /v /z /i /Q /R  		"\\192.168.110.185\FixesFullVersion\WebSite\Retail.MultiNet\%YYYYMMDD%\bin\"
xcopy "C:\TFS\Retail\Code\Retail.MultiNet.APIEntities\bin\Release\Retail.MultiNet.APIEntities.dll" 	/y /v /z /i /Q /R  		"\\192.168.110.185\FixesFullVersion\WebSite\Retail.MultiNet\%YYYYMMDD%\bin\"
xcopy "C:\TFS\Retail\Code\Retail.MultiNet.Web\Retail_MultiNet"										/S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\Retail.MultiNet\%YYYYMMDD%\Client\Modules\Retail_MultiNet\"

xcopy "C:\TFS\Vanrise\Code\Vanrise.InvToAccBalanceRelation.Web\VR_InvToAccBalanceRelation"			/S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\Retail.MultiNet\%YYYYMMDD%\Client\Modules\VR_InvToAccBalanceRelation\"

xcopy "C:\TFS\Retail\Code\Retail.Teles.Business\bin\Release\Retail.Teles.Business.dll" 				/y /v /z /i /Q /R 		"\\192.168.110.185\FixesFullVersion\WebSite\Retail.MultiNet\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Retail\Code\Retail.Teles.Entities\bin\Release\Retail.Teles.Entities.dll" 				/y /v /z /i /Q /R  		"\\192.168.110.185\FixesFullVersion\WebSite\Retail.MultiNet\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Retail\Code\Retail.Teles.Web\bin\Retail.Teles.Web.dll" 								/y /v /z /i /Q /R  		"\\192.168.110.185\FixesFullVersion\WebSite\Retail.MultiNet\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Retail\Code\Retail.Teles.Web\Retail_Teles" 											/S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\Retail.MultiNet\%YYYYMMDD%\Client\Modules\Retail_Teles\"