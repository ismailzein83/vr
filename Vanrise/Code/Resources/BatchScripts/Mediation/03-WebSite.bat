ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Mediation WebSite

xcopy "C:\Publish\Mediation" /S /E /R /y /v /i /z /Q															"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Mediation\list-of-excluded-files.txt
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q											"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\Common\Client-list-of-excluded-files.txt
xcopy "C:\TFS\Mediation\Code\Mediation.Web\Web.config.exclude" /y /v /z /i /Q /R								"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\"
rename																											"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Web.config.exclude" "Web.config"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R									"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Bin\"
::by default load flat theme
xcopy "C:\TFS\Mediation\Code\Mediation.Web\Images\flat-mediation-logoonheader.png" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Images\logoonheader.png*"
xcopy "C:\TFS\Mediation\Code\Mediation.Web\Images\flat-mediation-login.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Images\login.png*"
xcopy "C:\TFS\Mediation\Code\Mediation.Web\Images\flat-mediation-iconheader.ico" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-01.png" 		 /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-02.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-member.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-support.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-flat.cshtml" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-flat.cshtml" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\CSViews\Security\Login.cshtml*"
::default theme
xcopy "C:\TFS\Mediation\Code\Mediation.Web\Images\default-mediation-logoonheader.png" /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Default-theme\Images\logoonheader.png*"
xcopy "C:\TFS\Mediation\Code\Mediation.Web\Images\default-mediation-login.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Default-theme\Images\login.png*"
xcopy "C:\TFS\Mediation\Code\Mediation.Web\Images\default-mediation-iconheader.ico" /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Default-theme\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-01.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Default-theme\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-02.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Default-theme\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-member.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Default-theme\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-support.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Default-theme\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-default.cshtml" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Default-theme\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-default.cshtml" /S /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Default-theme\Client\CSViews\Security\Login.cshtml*"
::flat theme
xcopy "C:\TFS\Mediation\Code\Mediation.Web\Images\flat-mediation-logoonheader.png" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Flat-theme\Images\logoonheader.png*"
xcopy "C:\TFS\Mediation\Code\Mediation.Web\Images\flat-mediation-login.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Flat-theme\Images\login.png*"
xcopy "C:\TFS\Mediation\Code\Mediation.Web\Images\flat-mediation-iconheader.ico" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Flat-theme\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-01.png" 		 /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Flat-theme\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-02.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Flat-theme\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-member.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Flat-theme\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-support.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Flat-theme\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-flat.cshtml" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Flat-theme\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-flat.cshtml" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Flat-theme\Client\CSViews\Security\Login.cshtml*"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.BusinessProcess.Web\BusinessProcess" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Modules\BusinessProcess\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Modules\Runtime\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.Queueing.Web\Queueing" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Modules\Queueing\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Rules.Web\VR_Rules" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Modules\VR_Rules\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Integration.Web\Integration" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Modules\Integration\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Modules\VR_GenericData\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.DataParser.Web\VR_DataParser" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Modules\VR_DataParser\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Notification.Web\VR_Notification" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Modules\VR_Notification\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.ExcelConversion.Web\ExcelConversion" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Modules\ExcelConversion\"
xcopy "C:\TFS\Mediation\Code\Mediation.Generic.Web\Mediation_Generic" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Modules\Mediation_Generic\"
xcopy "C:\TFS\Mediation\Code\Mediation.Huawei.Web\Mediation_Huawei" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Client\Modules\Mediation_Huawei\"
del /s /q /f																									"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																									"\\192.168.110.185\Fixes\WebSite\Mediation\%YYYYMMDD%\Bin\*.config"
