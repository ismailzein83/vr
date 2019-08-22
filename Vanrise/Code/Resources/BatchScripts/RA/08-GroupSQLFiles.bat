ECHO OFF
ECHO Retail Group SQL Scripts
start /b /w /D"C:\TFS\Vanrise\Code\Vanrise.HelperTools\bin\Release" Vanrise.HelperTools.exe "GRPSQL" "RA" "Revenue Assurance"
start /b /w /D"C:\TFS\Vanrise\Code\Vanrise.HelperTools\bin\Release" Vanrise.HelperTools.exe "GRPSQLOverridden" "RA.ICX" "Interconnect"
start /b /w /D"C:\TFS\Vanrise\Code\Vanrise.HelperTools\bin\Release" Vanrise.HelperTools.exe "GRPSQLOverridden" "RA.INTL" "International"
start /b /w /D"C:\TFS\Vanrise\Code\Vanrise.HelperTools\bin\Release" Vanrise.HelperTools.exe "GRPSQLOverridden" "RA.Retail" "On-net"