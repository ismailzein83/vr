ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CarrierPortal.MultinetPortal WebSite

xcopy "C:\TFS\CarrierPortal\Code\PartnerPortal.CustomerAccess.Web\PartnerPortal_CustomerAccess" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Client\Modules\PartnerPortal_CustomerAccess\"
xcopy "C:\TFS\CarrierPortal\Code\PartnerPortal.Invoice.Web\PartnerPortal_Invoice" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Client\Modules\PartnerPortal_Invoice\"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\bin\PartnerPortal*" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\CarrierPortal\Code\CP.MultiNet.Web\CP_MultiNet" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Client\Modules\CP_MultiNet\"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\bin\CP.MultiNet*" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\bin\Retail*" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Bin\"
del /s /q /f																											"\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Bin\Retail.Ringo.*"
del /s /q /f																											"\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																											"\\192.168.110.185\Fixes\WebSite\CP.Multinet\%YYYYMMDD%\Bin\*.config"