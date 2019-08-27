ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO NtegraV2 Themes

::xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\images\nTegra-logoonheader.png" /S /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\images\"
::xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\images\nTegra-login.png" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\images\"
::xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\images\nTegra-iconheader.ico" /S /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\images\"
::rename "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\images\nTegra-logoonheader.png" "logoonheader.png"
::rename "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\images\nTegra-login.png" "login.png"
::rename "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\images\nTegra-iconheader.ico" "iconheader.ico"

::xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Themes\theme-extented-nTegra.css.exclude" /S /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Themes\"
::rename "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Themes\theme-extented-nTegra.css.exclude" "theme-extented.css"

::xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\nTegra-01.png" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Images\"
::xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\nTegra-02.png" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Images\"
::rename "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Images\nTegra-01.png" "01.png"
::rename "\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Images\nTegra-02.png" "02.png"


::by default load flat theme
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\nTegra-flat-logoonheader.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Images\logoonheader.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\nTegra-flat-login.png" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Images\login.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\nTegra-flat-iconheader.ico" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-01.png" 		 /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-02.png" /S /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-member.png" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-support.png" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-flat.cshtml" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-flat.cshtml" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Client\CSViews\Security\Login.cshtml*"
::default theme
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\nTegra-logoonheader.png" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Default-theme\Images\logoonheader.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\nTegra-login.png" /S /R /y /v /i /z /Q										"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Default-theme\Images\login.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\nTegra-iconheader.ico" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Default-theme\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-01.png" /S /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Default-theme\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-02.png" /S /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Default-theme\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-member.png" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Default-theme\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-support.png" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Default-theme\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-default.cshtml" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Default-theme\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-default.cshtml" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Default-theme\Client\CSViews\Security\Login.cshtml*"
::flat theme
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\nTegra-flat-logoonheader.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Flat-theme\Images\logoonheader.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\nTegra-flat-login.png" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Flat-theme\Images\login.png*"
xcopy "C:\TFS\TOneV2\Code\TOneV2\TOne.Web\Images\nTegra-flat-iconheader.ico" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Flat-theme\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-01.png" 		 /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Flat-theme\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-02.png" /S /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Flat-theme\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-member.png" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Flat-theme\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-support.png" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Flat-theme\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-flat.cshtml" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Flat-theme\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-flat.cshtml" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\NtegraV2\%YYYYMMDD%\Flat-theme\Client\CSViews\Security\Login.cshtml*"