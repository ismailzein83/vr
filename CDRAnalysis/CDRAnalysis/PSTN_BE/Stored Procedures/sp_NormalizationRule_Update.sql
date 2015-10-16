-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_NormalizationRule_Update]
	@ID INT,
	@Criteria NVARCHAR(MAX),
	@Settings NVARCHAR(MAX),
	@Description NVARCHAR(MAX) = NULL,
	@BED DATETIME,
	@EED DATETIME = NULL
AS
BEGIN
	UPDATE PSTN_BE.NormalizationRule
	SET Criteria = @Criteria,
		Settings = @Settings,
		[Description] = @Description,
		BED = @BED,
		EED = @EED
	WHERE ID = @ID
END