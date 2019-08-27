ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Retail.NtegraV2 WebSite

::xcopy "C:\TFS\Retail\Code\Retail.Web\Images\nTegra-logoonheader.png" /S /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Images\"
::xcopy "C:\TFS\Retail\Code\Retail.Web\Images\nTegra-login.png" /S /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Images\"
::xcopy "C:\TFS\Retail\Code\Retail.Web\Images\nTegra-iconheader.ico" /S /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Images\"
::rename "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Images\nTegra-logoonheader.png" "logoonheader.png"
::rename "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Images\nTegra-login.png" "login.png"
::rename "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Images\nTegra-iconheader.ico" "iconheader.ico"
::
::xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Themes\theme-extented-nTegra.css.exclude" /S /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Themes\"
::rename "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Themes\theme-extented-nTegra.css.exclude" "theme-extented.css"
::
::xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\nTegra-01.png" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Images\"
::xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\nTegra-02.png" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Images\"
::rename "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Images\nTegra-01.png" "01.png"
::rename "\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Images\nTegra-02.png" "02.png"


::by default load flat theme
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\nTegra-flat-logoonheader.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Images\logoonheader.png*"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\nTegra-flat-login.png" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Images\login.png*"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\nTegra-flat-iconheader.ico" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-01.png" 		 /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-02.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-member.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-support.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-flat.cshtml" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-flat.cshtml" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Client\CSViews\Security\Login.cshtml*"
::default theme
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\nTegra-logoonheader.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Default-theme\Images\logoonheader.png*"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\nTegra-login.png" /S /R /y /v /i /z /Q									"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Default-theme\Images\login.png*"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\nTegra-iconheader.ico" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Default-theme\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-01.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Default-theme\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-02.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Default-theme\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-member.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Default-theme\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\default-support.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Default-theme\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-default.cshtml" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Default-theme\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-default.cshtml" /S /R /y /v /i /z /Q			"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Default-theme\Client\CSViews\Security\Login.cshtml*"
::flat theme
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\nTegra-flat-logoonheader.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Flat-theme\Images\logoonheader.png*"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\nTegra-flat-login.png" /S /R /y /v /i /z /Q								"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Flat-theme\Images\login.png*"
xcopy "C:\TFS\Retail\Code\Retail.Web\Images\nTegra-flat-iconheader.ico" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Flat-theme\Images\iconheader.ico*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-01.png" 		 /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Flat-theme\Client\Images\01.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-02.png" /S /R /y /v /i /z /Q							"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Flat-theme\Client\Images\02.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-member.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Flat-theme\Client\Images\member.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\Images\new-flat-support.png" /S /R /y /v /i /z /Q						"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Flat-theme\Client\Images\support.png*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Home\Index-flat.cshtml" /S /R /y /v /i /z /Q					"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Flat-theme\Client\CSViews\Home\Index.cshtml*"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client\CSViews\Security\Login-flat.cshtml" /S /R /y /v /i /z /Q				"\\192.168.110.185\Fixes\WebSite\Retail.Ntegra\%YYYYMMDD%\Flat-theme\Client\CSViews\Security\Login.cshtml*"