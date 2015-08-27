-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE [BEntity].[sp_CarrierSummaryStats_CreateTempForFiltered]
	   @TempTableName varchar(200),
	   @CarrierType VARCHAR(10),
	   @fromDate datetime ,
	   @ToDate datetime,
	   @CustomerID varchar(15)=NULL,
	   @SupplierID varchar(15)=NULL,
	   @ZoneID VARCHAR(15) = NULL,
	   @TopRecord INT =NULL,
	   @GroupByProfile char(1) = 'N',
	   @CustomerAmuID int = NULL,
	   @SupplierAmuID int = NULL,
	   @Currency VARCHAR(3) = 'USD'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @SQL NVARCHAR(MAX) = ''
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	Begin
	SET @SQL = @SQL +'DECLARE @RESULTTBL table(GroupID varchar(100), Attempts float, SuccessfulAttempts float, DurationsInMinutes float, ASR float, ACD float, DeliveredASR float, AveragePDD float, NumberOfCalls float, PricedDuration float, Sale_Nets float, Cost_Nets float, Profit float, Percentage float, rownIndex int)'
	
	    BEGIN
			IF(@CarrierType = 'Supplier')
			 SET @SQL = @SQL + 'Insert into @RESULTTBL
			  EXEC SP_TrafficStats_SupplierSummary
			    @fromDate = '''+convert(varchar(10),@fromDate,126)+''',
	 			@ToDate = '''+convert(varchar(10),@ToDate,126)+''',
	 			@SupplierID = '+CASE WHEN @SupplierID IS NULL THEN 'NULL' ELSE ( '''' + CONVERT(varchar(15),@SupplierID) + '''') END+',
	 			@TopRecord = '+convert(varchar(5),@TopRecord)+',
	 			@GroupByProfile = '''+@GroupByProfile+''',
	 			@CustomerAmuID = '+isnull(Convert(varchar(10),@CustomerAmuID),'NULL')+',
			    @SupplierAmuID = '+isnull(Convert(varchar(10),@SupplierAmuID),'NULL')+',
				@Currency = '''+convert(varchar(3),@Currency)+''''
		
		    IF(@CarrierType = 'Customer')
			SET @SQL = @SQL + ' INSERT INTO @RESULTTBL
			 EXEC SP_TrafficStats_CustomerSummary
			    @fromDate = ''' + convert(varchar(10),@fromDate,126)+''',
	 			@ToDate = ''' + convert(varchar(10),@ToDate,126)+''',
	 			@CustomerID = ' + CASE WHEN @CustomerID IS NULL THEN 'NULL' ELSE ( '''' + CONVERT(varchar(15),@CustomerID) + '''') END+',
	 			@TopRecord = '+convert(varchar(5),@TopRecord)+',
	 			@GroupByProfile = '''+@GroupByProfile+''',
	 			@CustomerAmuID = '+isnull(Convert(varchar(10),@CustomerAmuID),'NULL')+',
			    @SupplierAmuID = '+isnull(Convert(varchar(10),@SupplierAmuID),'NULL')+',
				@Currency = '''+convert(varchar(3),@Currency)+''''
				
				
			IF(@CarrierType = 'Zone')
			 SET @SQL = @SQL + 'Insert into @RESULTTBL
			  EXEC SP_TrafficStats_ZoneSummary
			    @fromDate = '''+convert(varchar(10),@fromDate,126)+''',
	 			@ToDate = '''+convert(varchar(10),@ToDate,126)+''',
	 			@ZoneID = '+CASE WHEN @ZoneID IS NULL THEN 'NULL' ELSE ( '''' + CONVERT(varchar(15),@ZoneID) + '''') END+',	 			
				@TopRecord = '''+convert(varchar(5),@TopRecord)+''''
			SET @SQL = @SQL + ' SELECT * INTO ' + @TempTableName + ' FROM @RESULTTBL';
			EXEC(@SQL)
		END
	END
END