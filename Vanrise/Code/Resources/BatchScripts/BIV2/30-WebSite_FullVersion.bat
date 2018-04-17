ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO BIV2 WebSite Full Version

xcopy "C:\Publish\TOneV2" 													/S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\BIV2\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\BIV2\list-of-excluded-files.txt
xcopy "C:\Publish\TOneV2\Bin\TOne.Web.dll" 									/y /v /z /i /Q /R		"\\192.168.110.185\FixesFullVersion\WebSite\BIV2\%YYYYMMDD%\Bin\"

xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\WebBI.config.exclude" 			/y /v /z /i /Q /R		"\\192.168.110.185\FixesFullVersion\WebSite\BIV2\%YYYYMMDD%\"
rename "\\192.168.110.185\FixesFullVersion\WebSite\BIV2\%YYYYMMDD%\WebBI.config.exclude" "Web.config"
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" 				/y /v /z /i /Q /R		"\\192.168.110.185\FixesFullVersion\WebSite\BIV2\%YYYYMMDD%\Bin\"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" 					/S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\BIV2\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" 						/S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\BIV2\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.ExcelConversion.Web\ExcelConversion" 	/S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\BIV2\%YYYYMMDD%\Client\Modules\ExcelConversion\"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" 								/S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\BIV2\%YYYYMMDD%\Client\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\BIV2\Client-list-of-excluded-files.txt

xcopy "C:\TFS\Vanrise\Code\Vanrise.BI.Web\BI" 								/S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\BIV2\%YYYYMMDD%\Client\Modules\BI\"