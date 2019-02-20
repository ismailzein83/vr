ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO TOneV2.SMS WebSite
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Web.config.SMS.exclude" /y /v /z /i /Q /R								"\\192.168.110.185\Fixes\WebSite\TOneV2.SMS\%YYYYMMDD%\"
rename																											"\\192.168.110.185\Fixes\WebSite\TOneV2.SMS\%YYYYMMDD%\Web.config.SMS.exclude" "Web.Update.SMS.config"
xcopy "C:\TFS\Vanrise\Code\Vanrise.MobileNetwork.Web\VR_MobileNetwork" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TOneV2.SMS\%YYYYMMDD%\Client\Modules\VR_MobileNetwork\"
xcopy "C:\Publish\Retail\bin\Vanrise.MobileNetwork*" /S /E /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\TOneV2.SMS\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.SMSBusinessEntity.Web\WhS_SMSBusinessEntity" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\TOneV2.SMS\%YYYYMMDD%\Client\Modules\WhS_SMSBusinessEntity\"
xcopy "C:\Publish\TOneV2\bin\TOne.WhS.SMSBusinessEntity*" /S /E /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\TOneV2.SMS\%YYYYMMDD%\Bin\"
del /s /q /f																									"\\192.168.110.185\Fixes\WebSite\TOneV2.SMS\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																									"\\192.168.110.185\Fixes\WebSite\TOneV2.SMS\%YYYYMMDD%\Bin\*.config"