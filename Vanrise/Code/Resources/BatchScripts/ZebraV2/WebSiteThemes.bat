ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO ZebraV2 Themes

xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\images\Zebra-logoonheader.png" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\ZebraV2\%YYYYMMDD%\images\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\images\Zebra-login.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\ZebraV2\%YYYYMMDD%\images\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\images\Zebra-iconheader.ico" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\ZebraV2\%YYYYMMDD%\images\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Themes\theme-extented-Zebra.css.exclude" /S /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\ZebraV2\%YYYYMMDD%\Client\Themes\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\Zebra-01.png" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\ZebraV2\%YYYYMMDD%\Client\Images\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\Zebra-02.png" /S /E /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\ZebraV2\%YYYYMMDD%\Client\Images\"

rename "\\192.168.110.185\Fixes\WebSite\ZebraV2\%YYYYMMDD%\images\Zebra-logoonheader.png" "logoonheader.png"
rename "\\192.168.110.185\Fixes\WebSite\ZebraV2\%YYYYMMDD%\images\Zebra-login.png" "login.png"
rename "\\192.168.110.185\Fixes\WebSite\ZebraV2\%YYYYMMDD%\images\Zebra-iconheader.ico" "iconheader.ico"
rename "\\192.168.110.185\Fixes\WebSite\ZebraV2\%YYYYMMDD%\Client\Themes\theme-extented-Zebra.css.exclude" "theme-extented.css"
rename "\\192.168.110.185\Fixes\WebSite\ZebraV2\%YYYYMMDD%\Client\Images\Zebra-01.png" "01.png"
rename "\\192.168.110.185\Fixes\WebSite\ZebraV2\%YYYYMMDD%\Client\Images\Zebra-02.png" "02.png"


