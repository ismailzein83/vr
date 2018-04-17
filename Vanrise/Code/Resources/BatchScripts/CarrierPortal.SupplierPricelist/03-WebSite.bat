ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CarrierPortal.SupplierPricelist WebSite

xcopy "C:\TFS\CarrierPortal\Code\CP.SupplierPricelist.Web\CP_SupplierPricelist" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CarrierPortal.SupplierPricelist\%YYYYMMDD%\Client\Modules\CP_SupplierPricelist\"

xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\Web.config.SupplierPricelist.exclude" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\CarrierPortal.SupplierPricelist\%YYYYMMDD%\"
rename "\\192.168.110.185\Fixes\WebSite\CarrierPortal.SupplierPricelist\%YYYYMMDD%\Web.config.SupplierPricelist.exclude" "Web.Update.SupplierPricelist.config"