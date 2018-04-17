ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Mediation WebSite Full Version

xcopy "C:\Publish\Mediation" /S /E /R /y /v /i /z /Q													"\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Mediation\list-of-excluded-files.txt
xcopy "C:\TFS\Mediation\Code\Mediation.Web\Web.config.exclude" /y /v /z /i /Q /R						"\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\"
rename "\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\Web.config.exclude" "Web.config"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R							"\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\Bin\"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\Client\"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.BusinessProcess.Web\BusinessProcess" /S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\Client\Modules\BusinessProcess\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\Client\Modules\Runtime\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.Queueing.Web\Queueing" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\Client\Modules\Queueing\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Rules.Web\VR_Rules" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\Client\Modules\VR_Rules\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Integration.Web\Integration" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\Client\Modules\Integration\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\Client\Modules\VR_GenericData\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.DataParser.Web\VR_DataParser" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\Client\Modules\VR_DataParser\"
																														   
xcopy "C:\TFS\Mediation\Code\Mediation.Generic.Web\Mediation_Generic" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\Client\Modules\Mediation_Generic\"
xcopy "C:\TFS\Mediation\Code\Mediation.Huawei.Web\Mediation_Huawei" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\FixesFullVersion\WebSite\Mediation\%YYYYMMDD%\Client\Modules\Mediation_Huawei\"