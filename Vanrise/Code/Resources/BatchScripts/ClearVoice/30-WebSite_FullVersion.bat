ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO ClearVoice WebSite Full Version

xcopy "C:\Publish\QualityMeasurement" /S /E /R /y /v /i /z /Q											"\\192.168.110.185\FixesFullVersion\WebSite\ClearVoice\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\ClearVoice\list-of-excluded-files.txt
xcopy "C:\TFS\QualityMeasurement\Code\QualityMeasurement.Web\Web.config.exclude" /y /v /z /i /Q /R		"\\192.168.110.185\FixesFullVersion\WebSite\ClearVoice\%YYYYMMDD%\"
rename "\\192.168.110.185\FixesFullVersion\WebSite\ClearVoice\%YYYYMMDD%\Web.config.exclude" "Web.config"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R							"\\192.168.110.185\FixesFullVersion\WebSite\ClearVoice\%YYYYMMDD%\Bin\"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\FixesFullVersion\WebSite\ClearVoice\%YYYYMMDD%\Client\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\FixesFullVersion\WebSite\ClearVoice\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\FixesFullVersion\WebSite\ClearVoice\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\FixesFullVersion\WebSite\ClearVoice\%YYYYMMDD%\Client\Modules\Runtime\"
::xcopy "C:\TFS\Vanrise\Code\Vanrise.BI.Web\BI" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\FixesFullVersion\WebSite\ClearVoice\%YYYYMMDD%\Client\Modules\BI\"

xcopy "C:\TFS\QualityMeasurement\Code\QM.BusinessEntity.Web\QM_BusinessEntity" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\FixesFullVersion\WebSite\ClearVoice\%YYYYMMDD%\Client\Modules\QM_BusinessEntity\"
xcopy "C:\TFS\QualityMeasurement\Code\QM.CLITester.Web\QM_CLITester" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\FixesFullVersion\WebSite\ClearVoice\%YYYYMMDD%\Client\Modules\QM_CLITester\"
