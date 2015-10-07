-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_NormalizationRule_Update]
	@ID INT,
	@Criteria NVARCHAR(MAX),
	@Settings NVARCHAR(MAX)
AS
BEGIN
	UPDATE PSTN_BE.NormalizationRule
	SET Criteria = @Criteria,
		Settings = @Settings
	WHERE ID = @ID
END