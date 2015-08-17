

CREATE PROCEDURE [FraudAnalysis].[sp_Dashboard_CreateTempForTopTenCell]
(
	@TempTableName varchar(200),	
	@FromDate datetime,
	@ToDate datetime
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
	    
			select top 10 count(distinct sc.AccountNumber) as CountCases, cdr.cell_id Cell_Id 
			into #Result
			from FraudAnalysis.NormalCDR cdr 
			inner join FraudAnalysis.AccountCase sc on cdr.MSISDN=sc.AccountNumber
			where cdr.connectdatetime between @FromDate and @ToDate and sc.StatusID = 3
			group by cell_id
			order by count(distinct sc.AccountNumber) desc
			
			
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END