-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Billing].[sp_BillingCDRCost_CreateTempBySupplierID]
	@TempTableName VARCHAR(200),
	@SupplierID VARCHAR(5),
	@From DATETIME,
	@To DATETIME
AS
BEGIN
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	BEGIN
		declare @SupplierGMTTime int
		set @SupplierGMTTime = (select GMTTime from dbo.CarrierAccount where CarrierAccountID = @SupplierID);

		declare @Result table
		(
			[Day] date,
			Amount float null,
			CurrencyID varchar(3) null,
			DurationInMinutes numeric(13, 4) null
		)

		insert into @Result ([Day], Amount, CurrencyID, DurationInMinutes)

		select
			CAST((DATEADD(M, -@SupplierGMTTime, main.Attempt)) as date) as [Day],
			SUM(cost.Net) as Amount,
			cost.CurrencyID,
			SUM(cost.DurationInSeconds) / 60 as DurationInMinutes
			
		from Billing_CDR_Main main with(nolock, index(IX_Billing_CDR_Main_Attempt, IX_Billing_CDR_Main_Supplier))
		left join Billing_CDR_Cost cost with(nolock) on cost.ID = main.ID

		where main.SupplierID = @SupplierID
			and main.Attempt >= @From
			and main.Attempt <= @To

		group by CAST((DATEADD(M, -@SupplierGMTTime, main.Attempt)) as date), cost.CurrencyID

		order by CAST((DATEADD(M, -@SupplierGMTTime, main.Attempt)) as date)
		
		select * into #RESULT from @Result
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
END