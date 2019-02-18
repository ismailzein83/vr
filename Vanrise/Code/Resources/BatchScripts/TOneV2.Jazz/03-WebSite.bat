ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO TOneV2.Jazz WebSite
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.WhS.Jazz.Web\WhS_Jazz" /S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\TOneV2.Jazz\%YYYYMMDD%\Client\Modules\WhS_Jazz\"
xcopy "C:\Publish\TOneV2\bin\TOne.WhS.Jazz*" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\TOneV2.Jazz\%YYYYMMDD%\Bin\"
del /s /q /f																			"\\192.168.110.185\Fixes\WebSite\TOneV2.Jazz\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																			"\\192.168.110.185\Fixes\WebSite\TOneV2.Jazz\%YYYYMMDD%\Bin\*.config"