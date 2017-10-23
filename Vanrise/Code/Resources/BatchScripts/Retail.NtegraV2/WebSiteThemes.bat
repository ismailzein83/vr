ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Retail.NtegraV2 WebSite

xcopy "C:\TFS\Retail\Code\Retail.Web\Images\nTegra-logoonheader.png" /S /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Images\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\nTegra-login.png" /S /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Images\"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\nTegra-iconheader.ico" /S /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Images\"
rename "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Images\nTegra-logoonheader.png" "logoonheader.png"
rename "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Images\nTegra-login.png" "login.png"
rename "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Images\nTegra-iconheader.ico" "iconheader.ico"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Themes\theme-extented-nTegra.css.exclude" /S /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Themes\"
rename "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Themes\theme-extented-nTegra.css.exclude" "theme-extented.css"

xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\nTegra-01.png" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Images\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\nTegra-02.png" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Images\"
rename "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Images\nTegra-01.png" "01.png"
rename "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Images\nTegra-02.png" "02.png"