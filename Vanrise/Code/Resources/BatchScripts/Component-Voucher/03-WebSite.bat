ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Component-Voucher WebSite

xcopy "C:\TFS\Vanrise\Code\Output\Vanrise.Voucher.Web.dll" 		/y /v /z /i /Q /R  		"\\192.168.110.185\Fixes\WebSite\Component-Voucher\%YYYYMMDD%\bin\"
xcopy "C:\TFS\Vanrise\Code\Output\Vanrise.Voucher.Entities.dll" /y /v /z /i /Q /R  		"\\192.168.110.185\Fixes\WebSite\Component-Voucher\%YYYYMMDD%\bin\"
xcopy "C:\TFS\Vanrise\Code\Output\Vanrise.Voucher.Business.dll" /y /v /z /i /Q /R  		"\\192.168.110.185\Fixes\WebSite\Component-Voucher\%YYYYMMDD%\bin\"
xcopy "C:\TFS\Vanrise\Code\Output\Vanrise.Voucher.Data.dll" 	/y /v /z /i /Q /R  		"\\192.168.110.185\Fixes\WebSite\Component-Voucher\%YYYYMMDD%\bin\"
xcopy "C:\TFS\Vanrise\Code\Output\Vanrise.Voucher.Data.SQL.dll" /y /v /z /i /Q /R  		"\\192.168.110.185\Fixes\WebSite\Component-Voucher\%YYYYMMDD%\bin\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Voucher.Web\VR_Voucher" /S /E /R /y /v /i /z /Q		"\\192.168.110.185\Fixes\WebSite\Component-Voucher\%YYYYMMDD%\Client\Modules\VR_Voucher\"
