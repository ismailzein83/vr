-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_AccountPackage_Insert]
	@AccountID BIGINT,
	@PackageID INT,
	@AccountBEDefinitionId uniqueidentifier,
	@BED DATETIME,
	@EED DATETIME,
	@ID BIGINT OUT
AS
BEGIN
	
	BEGIN
		INSERT INTO Retail.AccountPackage (AccountID, PackageID, AccountBEDefinitionId, BED, EED)
		VALUES (@AccountID, @PackageID, @AccountBEDefinitionId, @BED, @EED)
		SET @ID = SCOPE_IDENTITY()
	END
END