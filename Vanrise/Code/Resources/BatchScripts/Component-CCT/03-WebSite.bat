ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO Component-CCT WebSite
xcopy "C:\TFS\Xbooster\Code\CDRComparison.Web\CDRComparison"						/S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\Component-CCT\%YYYYMMDD%\Client\Modules\CDRComparison\"
xcopy "C:\TFS\Xbooster\Code\Xbooster.Web\Images\menu-icons\CDR Compare Tool.png"	/S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\Component-CCT\%YYYYMMDD%\Images\menu-icons\"
xcopy "C:\TFS\Xbooster\Code\Xbooster.Web\Web.config.CCT.exclud"						/y /v /z /i /Q /R		"\\192.168.110.185\Fixes\WebSite\Component-CCT\%YYYYMMDD%\Web.CCT.config*"
xcopy "C:\Publish\XBooster\bin\CDRComparison.*"										/S /E /R /y /v /i /z /Q	"\\192.168.110.185\Fixes\WebSite\Component-CCT\%YYYYMMDD%\Bin"
del /s /q /f																								"\\192.168.110.185\Fixes\WebSite\Component-CCT\%YYYYMMDD%\Bin\*.pdb"
del /s /q /f																								"\\192.168.110.185\Fixes\WebSite\Component-CCT\%YYYYMMDD%\Bin\*.config"