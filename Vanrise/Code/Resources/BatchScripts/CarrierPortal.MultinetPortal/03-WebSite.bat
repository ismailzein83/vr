ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CarrierPortal.MultinetPortal WebSite

xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Client\Modules\Analytic\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Client\Modules\VR_GenericData\"

::Robocopy "C:\TFS\CarrierPortal\Code\PartnerPortal.CustomerAccess.Web\PartnerPortal_CustomerAccess"  "\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Client\Modules\PartnerPortal_CustomerAccess" /SEC /S /E /v /z
xcopy "C:\TFS\CarrierPortal\Code\PartnerPortal.CustomerAccess.Web\PartnerPortal_CustomerAccess" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Client\Modules\PartnerPortal_CustomerAccess\"
xcopy "C:\TFS\CarrierPortal\Code\PartnerPortal.Invoice.Web\PartnerPortal_Invoice" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Client\Modules\PartnerPortal_Invoice\"

xcopy "C:\TFS\CarrierPortal\Code\CP.MultiNet.Web\CP_MultiNet" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Client\Modules\CP_MultiNet\"

xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Web.config.MultinetPortal.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Web.config.MultinetPortal.exclude" "Web.Update.MultinetPortal.config"

ECHO CarrierPortal.MultinetPortal WebSite Full Version

xcopy "C:\TFS\Vanrise\Code\Vanrise.Analytic.Web\Analytic" /S /E /R /y /v /i /z /Q										"\\192.168.110.185\FixesFullVersion\WebSite\CP.Multinet\%YYYYMMDD%\Client\Modules\Analytic\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\FixesFullVersion\WebSite\CP.Multinet\%YYYYMMDD%\Client\Modules\VR_GenericData\"

::Robocopy "C:\TFS\CarrierPortal\Code\PartnerPortal.CustomerAccess.Web\PartnerPortal_CustomerAccess"					"\\192.168.110.185\FixesFullVersion\WebSite\CP.Multinet\%YYYYMMDD%\Client\Modules\PartnerPortal_CustomerAccess" /SEC /S /E /v /z
xcopy "C:\TFS\CarrierPortal\Code\PartnerPortal.CustomerAccess.Web\PartnerPortal_CustomerAccess" /S /E /R /y /v /i /z /Q "\\192.168.110.185\FixesFullVersion\WebSite\CP.Multinet\%YYYYMMDD%\Client\Modules\PartnerPortal_CustomerAccess\"
xcopy "C:\TFS\CarrierPortal\Code\PartnerPortal.Invoice.Web\PartnerPortal_Invoice" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\FixesFullVersion\WebSite\CP.Multinet\%YYYYMMDD%\Client\Modules\PartnerPortal_Invoice\"

xcopy "C:\TFS\CarrierPortal\Code\CP.MultiNet.Web\CP_MultiNet" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\FixesFullVersion\WebSite\CP.Multinet\%YYYYMMDD%\Client\Modules\CP_MultiNet\"

xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Web.config.MultinetPortal.exclude" /y /v /z /i /Q /R					"\\192.168.110.185\FixesFullVersion\WebSite\CP.Multinet\%YYYYMMDD%\"
rename "\\192.168.110.185\FixesFullVersion\WebSite\CP.Multinet\%YYYYMMDD%\Web.config.MultinetPortal.exclude" "Web.Update.MultinetPortal.config"

