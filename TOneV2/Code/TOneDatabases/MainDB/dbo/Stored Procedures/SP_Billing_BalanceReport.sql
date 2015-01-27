
CREATE PROCEDURE [dbo].[SP_Billing_BalanceReport]
	@IsProfile char(1) = 'N',
	@ProfileID int = NULL,
	@CarrierAccountID varchar(5) = NULL,
	@From DateTime,
	@To DATETIME,
	@InvoiceIDs VARCHAR(250)
AS
BEGIN
	Declare  @result table (
		ID INT IDENTITY(1,1),
		[Partner] varchar(15),
		[Type] varchar(10),
		[DocN] varchar(50),
		[TrafficPeriod]  varchar(15),
		[Date]  varchar(12),
		[CurrencyID]  varchar(3),
		[ICS_Invoice] decimal(13,2),--6
		[Partner_Invoice] decimal(13,2),--6
		[DueDate] varchar(12),
		[ICS_Payment] decimal(13,6),
		[Partner_Payment] decimal(13,6)
		)
		
	IF(@IsProfile = 'Y')
	BEGIN

		DECLARE @CarrierAccountIDs TABLE (carrieraccountid VARCHAR(5))

		INSERT INTO @CarrierAccountIDs
		SELECT carrieraccountid FROM carrieraccount WHERE profileid=@ProfileID

		INSERT INTO @result
		([Partner],
		[Type],
		[DocN],
		[TrafficPeriod],
		[Date],
		CurrencyID,
		[ICS_Invoice],
		[Partner_Invoice],
		[DueDate],
		[ICS_Payment],
		[Partner_Payment]
		)
		SELECT	cp.name ,
				'Invoice',
				serialnumber , 
				CONVERT(varchar(2), BeginDate, 3)+'-' + CONVERT(varchar(2), EndDate, 3)+ ' ' + convert(char(3),BeginDate, 0)+'''' + Right(Cast(Year(BeginDate) As varchar(4)),2),
				convert(varchar(12),convert(datetime,IssueDate,103),101) ,
				bi.CurrencyID ,
				null ,
				Amount,
				convert(varchar(12),convert(datetime,DueDate,103),101),
				Null,
				Null
		from	billing_invoice bi 
				JOIN carrieraccount ca ON bi.supplierid=ca.carrieraccountID
				JOIN carrierprofile cp ON cp.profileid=ca.profileid
		where	begindate between @From AND @To 
				AND (@InvoiceIDs IS NULL OR bi.InvoiceID IN (SELECT value FROM dbo.ParseArray(@InvoiceIDs,',')))
				AND issent='Y' 
				and SupplierID IN (SELECT carrieraccountid FROM @CarrierAccountIDs)
		order by begindate



		INSERT INTO @result
		([Partner],
		[Type],
		[DocN],
		[TrafficPeriod],
		[Date],
		CurrencyID,
		[ICS_Invoice],
		[Partner_Invoice],
		[DueDate],
		[ICS_Payment],
		[Partner_Payment]
		)

		Select	cp.name,
				'Payment',
				null,
				null,
				convert(varchar(12),convert(datetime,[Date],103),101),
				pa.CurrencyID,
				null,
				null,
				null,
				null,
				-1*amount 
		from	postpaidamount pa 
				join Carrierprofile cp on pa.supplierprofileID=cp.profileid
		where	[date] BETWEEN @From AND @To 
				and supplierprofileid=@profileid 
				and [Type]=0
		ORDER BY [Date]

		insert into @result
		([Partner],
		[Type],
		[DocN],
		[TrafficPeriod],
		[Date],
		CurrencyID,
		[ICS_Invoice],
		[Partner_Invoice],
		[DueDate],
		[ICS_Payment],
		[Partner_Payment]
		)
		select	cp.name ,
				'Invoice',
				serialnumber , 
				CONVERT(varchar(2), BeginDate, 3)+'-' + CONVERT(varchar(2), EndDate, 3)+ ' ' + convert(char(3),BeginDate, 0)+'''' + Right(Cast(Year(BeginDate) As varchar(4)),2),
				convert(varchar(12),convert(datetime,IssueDate,103),101) ,
				bi.CurrencyID ,
				Amount ,
				null,
				convert(varchar(12),convert(datetime,DueDate,103),101),
				Null,
				Null
		from	billing_invoice bi 
				join carrieraccount ca ON bi.customerid=ca.carrieraccountID
				JOIN carrierprofile cp ON cp.profileid=ca.profileid
		where	begindate BETWEEN @From AND @To
				AND (@InvoiceIDs IS NULL OR bi.InvoiceID IN (SELECT value FROM dbo.ParseArray(@InvoiceIDs,',')))	
				and CustomerID IN (SELECT carrieraccountid FROM @carrieraccountids)
		order by begindate

		insert into @result
		([Partner],
		[Type],
		[DocN],
		[TrafficPeriod],
		[Date],
		CurrencyID,
		[ICS_Invoice],
		[Partner_Invoice],
		[DueDate],
		[ICS_Payment],
		[Partner_Payment]
		)

		Select	cp.name,
				'Payment',
				null,
				null,
				convert(varchar(12),convert(datetime,[Date],103),101),
				cp.CurrencyID,
				null,
				null,
				null,
				amount,
				null 
		from	postpaidamount pa 
				join CarrierProfile cp on pa.customerprofileID=cp.profileid
		where	[date] BETWEEN @From AND @To
				and customerprofileid=@profileid 
				and [Type]=0
		ORDER BY [Date]

		insert into @result
		([Partner],
		[Type],
		[DocN],
		[TrafficPeriod],
		[Date],
		CurrencyID,
		[ICS_Invoice],
		[Partner_Invoice],
		[DueDate],
		[ICS_Payment],
		[Partner_Payment]
		)
		select	'Total',
				null,
				null,
				null,
				null,
				null, 
				sum(ICS_Invoice),
				SUM(Partner_Invoice),
				null,
				SUM(ICS_Payment),
				SUM(Partner_Payment)
		from	@result
 
		insert into @result
		([Partner],
		[Type],
		[DocN],
		[TrafficPeriod],
		[Date],
		CurrencyID,
		[ICS_Invoice],
		[Partner_Invoice],
		[DueDate],
		[ICS_Payment],
		[Partner_Payment]
		)
		select	'Net Invoice',
				null,
				null,
				null,
				null,
				null, 
				ICS_Invoice - Partner_Invoice,
				null,
				null,
				null,
				null
		 from	@result
		 WHERE	[Partner]='Total'
		 
		 insert into @result
		([Partner],
		[Type],
		[DocN],
		[TrafficPeriod],
		[Date],
		CurrencyID,
		[ICS_Invoice],
		[Partner_Invoice],
		[DueDate],
		[ICS_Payment],
		[Partner_Payment]
		)
		select	'Net Balance',	
				null,
				null,
				null,
				null,
				null, 
				ISNULL(ICS_Invoice,0) - ISNULL(Partner_Invoice,0) - ISNULL(ICS_Payment,0) + ISNULL(Partner_Payment,0),
				null,
				null,
				null,
				null
		from	@result
		WHERE	[Partner]='Total' 
 
		 
	END
	ELSE
	BEGIN
		
		--DECLARE @ProfileID_ INT
		
		--SET @ProfileID = (SELECT ProfileID FROM CarrierAccount WHERE CarrierAccountID = @CarrierAccountID)


		INSERT INTO @result
		([Partner],
		[Type],
		[DocN],
		[TrafficPeriod],
		[Date],
		CurrencyID,
		[ICS_Invoice],
		[Partner_Invoice],
		[DueDate],
		[ICS_Payment],
		[Partner_Payment]
		)
		SELECT	cp.name ,
				'Invoice',
				serialnumber , 
				CONVERT(varchar(2), BeginDate, 3)+'-' + CONVERT(varchar(2), EndDate, 3)+ ' ' + convert(char(3),BeginDate, 0)+'''' + Right(Cast(Year(BeginDate) As varchar(4)),2),
				convert(varchar(12),convert(datetime,IssueDate,103),101) ,
				bi.CurrencyID ,
				null ,
				Amount,
				convert(varchar(12),convert(datetime,DueDate,103),101),
				Null,
				Null
		from	billing_invoice bi 
				JOIN carrieraccount ca ON bi.supplierid=ca.carrieraccountID
				JOIN carrierprofile cp ON cp.profileid=ca.profileid
		where	BeginDate between @From AND @To 
				AND (@InvoiceIDs IS NULL OR bi.InvoiceID IN (SELECT value FROM dbo.ParseArray(@InvoiceIDs,',')))
				AND IsSent = 'Y' 
				and SupplierID = @CarrierAccountID
		order by begindate



		INSERT INTO @result
		([Partner],
		[Type],
		[DocN],
		[TrafficPeriod],
		[Date],
		CurrencyID,
		[ICS_Invoice],
		[Partner_Invoice],
		[DueDate],
		[ICS_Payment],
		[Partner_Payment]
		)

		Select	ca.CarrierAccountID,
				'Payment',
				null,
				null,
				convert(varchar(12),convert(datetime,[Date],103),101),
				pa.CurrencyID,
				null,
				null,
				null,
				null,
				-1*amount 
		from	postpaidamount pa
				JOIN CarrierAccount ca ON pa.SupplierID = ca.CarrierAccountID 
				--join Carrierprofile cp on ca.profileID=cp.profileid
		where	pa.[date] BETWEEN @From AND @To 
				and pa.SupplierID = @CarrierAccountID
				and pa.[Type]=0
		ORDER BY [Date]

		insert into @result
		([Partner],
		[Type],
		[DocN],
		[TrafficPeriod],
		[Date],
		CurrencyID,
		[ICS_Invoice],
		[Partner_Invoice],
		[DueDate],
		[ICS_Payment],
		[Partner_Payment]
		)
		select	cp.name ,
				'Invoice',
				serialnumber , 
				CONVERT(varchar(2), bi.BeginDate, 3)+'-' + CONVERT(varchar(2), bi.EndDate, 3)+ ' ' + convert(char(3),bi.BeginDate, 0)+'''' + Right(Cast(Year(bi.BeginDate) As varchar(4)),2),
				convert(varchar(12),convert(datetime,bi.IssueDate,103),101) ,
				bi.CurrencyID ,
				bi.Amount ,
				null,
				convert(varchar(12),convert(datetime,bi.DueDate,103),101),
				Null,
				Null
		from	billing_invoice bi 
				left join carrieraccount ca ON bi.CustomerID=ca.CarrierAccountID
				left JOIN carrierprofile cp ON ca.profileid=cp.profileid
		where	bi.begindate BETWEEN @From AND @To
		        AND (@InvoiceIDs IS NULL OR bi.InvoiceID IN (SELECT value FROM dbo.ParseArray(@InvoiceIDs,',')))
				and bi.CustomerID = @CarrierAccountID --IN (SELECT carrieraccountid FROM @carrieraccountids)
		order by begindate

		insert into @result
		([Partner],
		[Type],
		[DocN],
		[TrafficPeriod],
		[Date],
		CurrencyID,
		[ICS_Invoice],
		[Partner_Invoice],
		[DueDate],
		[ICS_Payment],
		[Partner_Payment]
		)

		Select	ca.CarrierAccountID,
				'Payment',
				null,
				null,
				convert(varchar(12),convert(datetime,[Date],103),101),
				cp.CurrencyID,
				null,
				null,
				null,
				amount,
				null 
		from	postpaidamount pa 
				JOIN CarrierAccount ca ON pa.CustomerID = ca.CarrierAccountID 
				join CarrierProfile cp on ca.profileid = cp.profileid
		where	[date] BETWEEN @From AND @To
				and customerid=@CarrierAccountID 
				and [Type]=0
		ORDER BY [Date]

		insert into @result
		([Partner],
		[Type],
		[DocN],
		[TrafficPeriod],
		[Date],
		CurrencyID,
		[ICS_Invoice],
		[Partner_Invoice],
		[DueDate],
		[ICS_Payment],
		[Partner_Payment]
		)
		select	'Total',
				null,
				null,
				null,
				null,
				null, 
				sum(ICS_Invoice),
				SUM(Partner_Invoice),
				null,
				SUM(ICS_Payment),
				SUM(Partner_Payment)
		from	@result
 
		insert into @result
		([Partner],
		[Type],
		[DocN],
		[TrafficPeriod],
		[Date],
		CurrencyID,
		[ICS_Invoice],
		[Partner_Invoice],
		[DueDate],
		[ICS_Payment],
		[Partner_Payment]
		)
		select	'Net Invoice',
				null,
				null,
				null,
				null,
				null, 
				ICS_Invoice - Partner_Invoice,
				null,
				null,
				null,
				null
		 from	@result
		 WHERE	[Partner]='Total'
		 
		 insert into @result
		([Partner],
		[Type],
		[DocN],
		[TrafficPeriod],
		[Date],
		CurrencyID,
		[ICS_Invoice],
		[Partner_Invoice],
		[DueDate],
		[ICS_Payment],
		[Partner_Payment]
		)
		select	'Net Balance',	
				null,
				null,
				null,
				null,
				null, 
				ISNULL(ICS_Invoice,0) - ISNULL(Partner_Invoice,0) - ISNULL(ICS_Payment,0) + ISNULL(Partner_Payment,0),
				null,
				null,
				null,
				null
		from	@result
		WHERE	[Partner]='Total' 
 
	END
	
	SELECT [Partner],
				[Type],
				[DocN],
				[TrafficPeriod],
				[Date],
				CurrencyID,
				[ICS_Invoice],
				[Partner_Invoice],
				[DueDate],
				[ICS_Payment],
				[Partner_Payment] 
	FROM	@result 
	ORDER BY ID
END