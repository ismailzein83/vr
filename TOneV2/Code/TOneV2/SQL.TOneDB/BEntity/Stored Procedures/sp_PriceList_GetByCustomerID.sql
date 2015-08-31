-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [BEntity].[sp_PriceList_GetByCustomerID] 
	-- Add the parameters for the stored procedure here
	@TempTableName VARCHAR(200),
	@CustomerID VARCHAR(5)
AS
BEGIN
		SET NOCOUNT ON;
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
			BEGIN
				 SELECT p.PriceListID,
						p.[Description],
						p.SourceFileName,
						p.UserID,
						p.BeginEffectiveDate,
						Name
				  INTO #RESULT
	              FROM PriceList p with(nolock)
                  INNER JOIN [User] u with(nolock) on P.UserID= u.ID
	              WHERE p.SupplierID='SYS'  AND p.CustomerID= @CustomerID
	              DECLARE @sql VARCHAR(1000)
				  SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
				  EXEC(@sql)
			END
END