-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].[sp_RecurringChargeDefinition_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS
	(
		SELECT 1 FROM Retail_BE.RecurringChargeDefinition
		WHERE ID != @ID AND Name = @Name
	)
	BEGIN
		UPDATE Retail_BE.RecurringChargeDefinition
		SET Name = @Name,Settings = @Settings
		WHERE ID = @ID
	END
END