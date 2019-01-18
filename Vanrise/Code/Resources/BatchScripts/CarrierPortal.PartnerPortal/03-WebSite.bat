ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO CarrierPortal.PartnerPortal WebSite

xcopy "C:\TFS\CarrierPortal\Code\PartnerPortal.CustomerAccess.Web\PartnerPortal_CustomerAccess" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\Client\Modules\PartnerPortal_CustomerAccess\"
xcopy "C:\TFS\CarrierPortal\Code\PartnerPortal.Invoice.Web\PartnerPortal_Invoice" /S /E /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\Client\Modules\PartnerPortal_Invoice\"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\bin\PartnerPortal*" /S /E /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\CarrierPortal\Code\CarrierPortal.Web\bin\Retail*" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\Bin\"
del /s /q /f																											"\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\Bin\Retail.Ringo.*"
del /s /q /f																											"\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\Bin\Retail.MultiNet.*"
del /s /q /f																											"\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																											"\\192.168.110.185\Fixes\WebSite\CP.Partner\%YYYYMMDD%\Bin\*.config"