-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_AccountPackage_Insert]
	@AccountID INT,
	@PackageID INT,
	@BED DATETIME,
	@EED DATETIME,
	@ID INT OUT
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM Retail.AccountPackage WHERE AccountID = @AccountID AND PackageID = @PackageID)
	BEGIN
		INSERT INTO Retail.AccountPackage (AccountID, PackageID, BED, EED)
		VALUES (@AccountID, @PackageID, @BED, @EED)
		SET @ID = SCOPE_IDENTITY()
	END
END