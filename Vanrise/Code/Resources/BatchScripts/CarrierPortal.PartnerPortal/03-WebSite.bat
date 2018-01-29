ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CarrierPortal.PartnerPortal WebSite

xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\Client\Modules\Analytic\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\Client\Modules\VR_GenericData\"

xcopy "C:\TFS\CarrierPortal\Code\PartnerPortal.CustomerAccess.Web\PartnerPortal_CustomerAccess" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\Client\Modules\PartnerPortal_CustomerAccess\"
xcopy "C:\TFS\CarrierPortal\Code\PartnerPortal.Invoice.Web\PartnerPortal_Invoice" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\Client\Modules\PartnerPortal_Invoice\"

xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Web.config.PartnerPortal.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\Web.config.PartnerPortal.exclude" "Web.Update.PartnerPortal.config"

ECHO CarrierPortal.PartnerPortal WebSite Full Version

xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\FixesFullVersion\WebSite\CP.Partner\%YYYYMMDD%\Client\Modules\Analytic\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\FixesFullVersion\WebSite\CP.Partner\%YYYYMMDD%\Client\Modules\VR_GenericData\"

xcopy "C:\TFS\CarrierPortal\Code\PartnerPortal.CustomerAccess.Web\PartnerPortal_CustomerAccess" /S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\CP.Partner\%YYYYMMDD%\Client\Modules\PartnerPortal_CustomerAccess\"
xcopy "C:\TFS\CarrierPortal\Code\PartnerPortal.Invoice.Web\PartnerPortal_Invoice" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\FixesFullVersion\WebSite\CP.Partner\%YYYYMMDD%\Client\Modules\PartnerPortal_Invoice\"

xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Web.config.PartnerPortal.exclude" /y /v /z /i /Q /R					"\\192.168.110.185\FixesFullVersion\WebSite\CP.Partner\%YYYYMMDD%\"
rename "\\192.168.110.185\FixesFullVersion\WebSite\CP.Partner\%YYYYMMDD%\Web.config.PartnerPortal.exclude" "Web.Update.PartnerPortal.config"