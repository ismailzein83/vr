ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CarrierPortal.TOne WebSite

xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\images\flat-Ogero-logoonheader.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\CP.TOne\%YYYYMMDD%\Images\Ogero\logoonheader.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\images\flat-Ogero-login.png" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\CP.TOne\%YYYYMMDD%\Images\Ogero\login.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\images\flat-Ogero-iconheader.ico" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\CP.TOne\%YYYYMMDD%\Images\Ogero\iconheader.ico*"
xcopy "C:\TFS\CarrierPortal\Code\CP.WhS.Web\CP_WhS" /S /E /R /y /v /i /z /Q												"\\192.168.110.185\Fixes\WebSite\CP.TOne\%YYYYMMDD%\Client\Modules\CP_WhS\"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\bin\CP.WhS*" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\CP.TOne\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\CarrierPortal\Code\PartnerPortal.CustomerAccess.Web\PartnerPortal_CustomerAccess" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\CP.TOne\%YYYYMMDD%\Client\Modules\PartnerPortal_CustomerAccess\"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\bin\PartnerPortal.CustomerAccess*" /S /E /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\CP.TOne\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\bin\TOne*" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\CP.TOne\%YYYYMMDD%\Bin\"
del /s /q /f																											"\\192.168.110.185\Fixes\WebSite\CP.TOne\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																											"\\192.168.110.185\Fixes\WebSite\CP.TOne\%YYYYMMDD%\Bin\*.config"