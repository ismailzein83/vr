




CREATE PROCEDURE [dbo].[sp_Traffic_CDR2]
		@CDROption     INT,-- 0 for all , 1 for validCDR , 2 For InvalidCDR
        @FromDuration  Numeric(13,5) = NULL,
        @ToDuration    Numeric(13,5) = NULL,
        @FromDate      DateTime ,
        @ToDate        DateTime ,
        @TopRecord     Int,
        @SwitchID      Tinyint =NULL,
        @SupplierID    varchar(10) = NULL,
        @CustomerID    varchar(10) = NULL,
        @OurZoneID     int = NULL,
        @Number       varchar(25)=NULL,
        @CLI VARCHAR(25)=NULL,
        @ReleaseCode VARCHAR(25)=NULL



        WITH Recompile 
AS

Declare @SQL1 VARCHAR(800),
        @SQL2 VARCHAR(800),
        @SQL3 VARCHAR(800),

		@NumberString VARCHAR(50),
		@CLIString VARCHAR(50),
        @SwitchIDString VARCHAR(50),
		@CustomerIDString VARCHAR(50),
		@SupplierIDString VARCHAR(50),
		@OurZoneIDString VARCHAR(50),
		@ReleaseCodeString VARCHAR(50),
		@FromDurationString VARCHAR(50),
		@ToDurationString VARCHAR(50),
		@IndexHint  VARCHAR(500),
		@InvalidIndexHint  VARCHAR(500),
		@CDRTable varchar(500)

SET NOCOUNT ON 

	if @Number is null
		Set    @NumberString=''
	else
		Set    @NumberString=' And CDPN Like '''+@Number+''''

	if @CLI is null
		Set    @CLIString=''
	else
		Set    @CLIString=' And CGPN Like '''+@CLI+''''

	if @CustomerID is null
		Begin
			Set		@CustomerIDString=''
			Set		@IndexHint  =''
			Set		@InvalidIndexHint=''
		End
	else
		Begin
			Set		@CustomerIDString=' And CustomerID = '''+@CustomerID+''''
			Set		@IndexHint  =',IX_Billing_CDR_Main_Customer'
			Set		@InvalidIndexHint=',IX_Billing_CDR_Invalid_Customer'
		End

	if @SupplierID is null
		Begin
			Set		@SupplierIDString=''
		End
	else
		Begin
			Set		@SupplierIDString=' And SupplierID = '''+@SupplierID+''''
			Set		@IndexHint  =',IX_Billing_CDR_Main_Supplier'
			Set		@InvalidIndexHint=@InvalidIndexHint+',IX_Billing_CDR_Invalid_Supplier'
		End


	if @OurZoneID is null
			Set    @OurZoneIDString=''
	else
		Begin
			Set    @OurZoneIDString=' And OurZoneID = '+convert(varchar(20),@OurZoneID)
			if len(@Number)>5
				Begin
					Set		@IndexHint  =',IX_Billing_CDR_Main_OurZoneID'+@IndexHint
					Set		@InvalidIndexHint=',IX_Billing_CDR_Invalid_OurZoneID'+@InvalidIndexHint
				End
		End
	if @ReleaseCode is null
		Set    @ReleaseCodeString=''
	else
		Set    @ReleaseCodeString=' And ReleaseCode = '''+@ReleaseCode+''''


	if @FromDuration is null
		Set    @FromDurationString=''
	else
		Set    @FromDurationString=' And DurationInSeconds >= '+convert(varchar(20),@FromDuration)

	if @ToDuration is null
		Set    @ToDurationString=''
	else
		Set    @ToDurationString=' And DurationInSeconds >= '+convert(varchar(20),@ToDuration)




--SET @Number=ISNULL(@Number,'%')

--SET ROWCOUNT @TopRecord
if @CDROption=0 or @CDROption=1
	Begin
		Set @SQL1 =
		'SELECT  top '+convert(varchar(20),@TopRecord)+'
				Attempt AS AttemptDateTime,
				CDPN,
				CGPN,
				ReleaseCode,
				ReleaseSource,
				DurationInSeconds AS Durations,
				SupplierZoneID,
				SupplierID,
				OurZoneID,
				CustomerID,
				SwitchCdrID,
				Tag
		 into ##tmp1            
		 FROM Billing_CDR_Main WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt'+@IndexHint+'))
		 WHERE                                   
			(Attempt Between '''+ convert(varchar(23),@FromDate) +''' And '''+ convert(varchar(23),@ToDate) +''')'

		Set @SQL2 =@OurZoneIDString+@CustomerIDString+@SupplierIDString+@NumberString+@CLIString+@ReleaseCodeString+@FromDurationString+@ToDurationString

		Set @SQL3=@SQL1+@SQL2+' ORDER BY Attempt DESC'   

		--select @SQL3
		exec (@SQL3)

	End

if @CDROption=0 or @CDROption=2
	Begin
		Set @SQL1 =
		'SELECT  top '+convert(varchar(20),@TopRecord)+'
				Attempt AS AttemptDateTime,
				CDPN,
				CGPN,
				ReleaseCode,
				ReleaseSource,
				DurationInSeconds AS Durations,
				SupplierZoneID,
				SupplierID,
				OurZoneID,
				CustomerID,
				SwitchCdrID,
				Tag
		 into ##tmp2            
		 FROM Billing_CDR_Invalid WITH(NOLOCK,INDEX(IX_Billing_CDR_invalid_Attempt'+@InvalidIndexHint+'))
		 WHERE                                   
			(Attempt Between '''+ convert(varchar(23),@FromDate) +''' And '''+ convert(varchar(23),@ToDate) +''')'

		Set @SQL2 =@OurZoneIDString+@CustomerIDString+@SupplierIDString+@NumberString+@CLIString+@ReleaseCodeString+@FromDurationString+@ToDurationString

		Set @SQL3=@SQL1+@SQL2+' ORDER BY Attempt DESC'   

--		select @SQL3
		 exec (@SQL3)
	End

set rowcount @TopRecord

if @CDROption=0 
	SELECT  ##tmp1.*
	from ##tmp1
	union all
	SELECT  ##tmp2.*
	from ##tmp2
    ORDER BY AttemptDateTime DESC   

if @CDROption=1
	SELECT  ##tmp1.*
	from ##tmp1
    ORDER BY AttemptDateTime DESC   

if @CDROption=2
	SELECT  ##tmp2.*
	from ##tmp2
    ORDER BY AttemptDateTime DESC