ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO NtegraV2 Themes

xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\images\nTegra-logoonheader.png" /S /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\images\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\images\nTegra-login.png" /S /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\images\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\images\nTegra-iconheader.ico" /S /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\images\"
rename "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\images\nTegra-logoonheader.png" "logoonheader.png"
rename "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\images\nTegra-login.png" "login.png"
rename "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\images\nTegra-iconheader.ico" "iconheader.ico"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Themes\theme-extented-nTegra.css.exclude" /S /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Themes\"
rename "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Themes\theme-extented-nTegra.css.exclude" "theme-extented.css"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\nTegra-01.png" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Images\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\nTegra-02.png" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Images\"
rename "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Images\nTegra-01.png" "01.png"
rename "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Images\nTegra-02.png" "02.png"


