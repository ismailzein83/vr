-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_NormalizationRule_Insert]
	@Criteria NVARCHAR(MAX),
	@Settings NVARCHAR(MAX),
	@Description NVARCHAR(MAX) = NULL,
	@BED DATETIME,
	@EED DATETIME = NULL,
	@ID INT OUT
AS
BEGIN
	INSERT INTO PSTN_BE.NormalizationRule (Criteria, Settings, [Description], BED, EED)
	VALUES (@Criteria, @Settings, @Description, @BED, @EED)
	
	SET @ID = @@IDENTITY
END