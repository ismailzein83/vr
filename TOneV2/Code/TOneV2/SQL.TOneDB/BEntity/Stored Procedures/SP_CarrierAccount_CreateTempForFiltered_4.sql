
CREATE PROCEDURE [BEntity].[SP_CarrierAccount_CreateTempForFiltered]
	@TempTableName varchar(200),	
	@Name VARCHAR(30) =  NULL,
	@CompanyName VARCHAR(50) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
	
			SELECT ca.CarrierAccountId,
			cp.ProfileId ,
			cp.Name AS ProfileName,
			cp.CompanyName AS ProfileCompanyName,
			ca.ActivationStatus,
			ca.RoutingStatus,
			ca.AccountType,
			ca.CustomerPaymentType,
			ca.SupplierPaymentType,
			ca.NameSuffix
			INTO #RESULT
			FROM CarrierAccount ca
			INNER JOIN CarrierProfile cp on ca.ProfileID = cp.ProfileID
			WHERE 
					(@Name IS NULL OR cp.Name like '%' + @Name + '%')
				AND (@CompanyName IS NULL OR cp.CompanyName like '%' + @CompanyName + '%' )
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END
			
END