
CREATE PROCEDURE [dbo].[sp_Traffic_CDR]
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
SET NOCOUNT ON 
SET ROWCOUNT @TopRecord
if @CustomerID is null and @SupplierID is null 
    SELECT 
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
     FROM Billing_CDR_Main M WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt))
     WHERE 1=1
        AND (@CDROption=0 OR @CDROption=1)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
        AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
UNION ALL
   SELECT 
		Attempt AS AttemptDateTime,
		CDPN,
		CGPN,
		ReleaseCode,
		ReleaseSource,
	    DurationInSeconds,
		SupplierZoneID,
		SupplierID,
		OurZoneID,
		CustomerID,
		SwitchCdrID,
		Tag
	FROM Billing_CDR_Invalid WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt))
	WHERE 1=1
        AND (@CDROption  =0 OR @CDROption =2)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
	    AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
    ORDER BY Attempt DESC   



if @CustomerID is not null and @SupplierID is null 
    SELECT 
		    Attempt AS AttemptDateTime,
			CDPN,
			CGPN,
			ReleaseCode,
	        ReleaseSource,
		    DurationInSeconds,
			SupplierZoneID,
			SupplierID,
			OurZoneID,
			CustomerID,
			SwitchCdrID,
			Tag            
     FROM Billing_CDR_Main M WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt),INDEX(IX_Billing_CDR_Main_Customer))
     WHERE 1=1
        AND (@CDROption=0 OR @CDROption=1)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (CustomerID = @CustomerID)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
        AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
UNION ALL
   SELECT 
		Attempt AS AttemptDateTime,
		CDPN,
		CGPN,
		ReleaseCode,
		ReleaseSource,
	    DurationInSeconds,
		SupplierZoneID,
		SupplierID,
		OurZoneID,
		CustomerID,
		SwitchCdrID,
		Tag
	FROM Billing_CDR_Invalid WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt),INDEX(IX_Billing_CDR_InValid_Customer))
	WHERE 1=1
        AND (@CDROption  =0 OR @CDROption =2)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (CustomerID = @CustomerID)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
	    AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
    ORDER BY Attempt DESC   


if @CustomerID is null and @SupplierID is not null 
    SELECT 
		    Attempt AS AttemptDateTime,
			CDPN,
			CGPN,
			ReleaseCode,
	        ReleaseSource,
		    DurationInSeconds,
			SupplierZoneID,
			SupplierID,
			OurZoneID,
			CustomerID,
			SwitchCdrID,
			Tag            
     FROM Billing_CDR_Main M WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt),INDEX(IX_Billing_CDR_Main_Supplier))
     WHERE 1=1
        AND (@CDROption=0 OR @CDROption=1)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (SupplierID = @SupplierID)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
        AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
UNION ALL
   SELECT 
		Attempt AS AttemptDateTime,
		CDPN,
		CGPN,
		ReleaseCode,
		ReleaseSource,
	    DurationInSeconds,
		SupplierZoneID,
		SupplierID,
		OurZoneID,
		CustomerID,
		SwitchCdrID,
		Tag
	FROM Billing_CDR_Invalid WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt),INDEX(IX_Billing_CDR_InValid_Supplier))
	WHERE 1=1
        AND (@CDROption  =0 OR @CDROption =2)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (SupplierID = @SupplierID)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
	    AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
    ORDER BY Attempt DESC   


if @CustomerID is not null and @SupplierID is not null 
    SELECT 
		    Attempt AS AttemptDateTime,
			CDPN,
			CGPN,
			ReleaseCode,
	        ReleaseSource,
		    DurationInSeconds,
			SupplierZoneID,
			SupplierID,
			OurZoneID,
			CustomerID,
			SwitchCdrID,
			Tag            
     FROM Billing_CDR_Main M WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt),INDEX(IX_Billing_CDR_Main_Customer),INDEX(IX_Billing_CDR_Main_Supplier))
     WHERE 1=1
        AND (@CDROption=0 OR @CDROption=1)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (CustomerID = @CustomerID)
		AND (SupplierID = @SupplierID)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
        AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
UNION ALL
   SELECT 
		Attempt AS AttemptDateTime,
		CDPN,
		CGPN,
		ReleaseCode,
		ReleaseSource,
	    DurationInSeconds,
		SupplierZoneID,
		SupplierID,
		OurZoneID,
		CustomerID,
		SwitchCdrID,
		Tag
	FROM Billing_CDR_Invalid WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt),INDEX(IX_Billing_CDR_InValid_Customer),INDEX(IX_Billing_CDR_InValid_Supplier))
	WHERE 1=1
        AND (@CDROption  =0 OR @CDROption =2)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (CustomerID = @CustomerID)
		AND (SupplierID = @SupplierID)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
	    AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
    ORDER BY Attempt DESC   


  OPTION(RECOMPILE)