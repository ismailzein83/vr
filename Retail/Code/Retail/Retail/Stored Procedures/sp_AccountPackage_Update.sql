-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_AccountPackage_Update]
@AccountPackgeID BIGINT,
	@BED DATETIME,
	@EED DATETIME
AS
BEGIN
		UPDATE Retail.AccountPackage
		SET  BED = @BED,EED=@EED
		WHERE ID = @AccountPackgeID

END