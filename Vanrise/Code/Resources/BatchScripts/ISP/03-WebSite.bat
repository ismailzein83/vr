ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO ISP WebSite

xcopy "C:\Publish\Retail" /S /E /R /y /v /i /z /Q														"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\ISP\list-of-excluded-files.txt
xcopy "C:\TFS\Retail\Code\Retail.Web\Web.config.ISP.exclude" /y /v /z /i /Q /R							"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\"
rename																									"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Web.config.ISP.exclude" "Web.config"												 
del /s /q /f																							"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Images\iconheader.ico"
del /s /q /f																							"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Images\login.png"
del /s /q /f																							"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Images\logoonheader.png"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\ISP-logoonheader.png" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Images\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\ISP-login.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Images\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\ISP-iconheader.ico" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Images\"
rename																									"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Images\ISP-logoonheader.png" "logoonheader.png"
rename																									"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Images\ISP-login.png" "login.png"
rename																									"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Images\ISP-iconheader.ico" "iconheader.ico"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R							"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Client\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\ISP\Client-list-of-excluded-files.txt
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Client\Modules\VR_GenericData\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Client\Modules\Analytic\"
xcopy "C:\TFS\Retail\Code\Retail.BusinessEntity.Web\Retail_BusinessEntity" /S /E /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\ISP\%YYYYMMDD%\Client\Modules\Retail_BusinessEntity\"