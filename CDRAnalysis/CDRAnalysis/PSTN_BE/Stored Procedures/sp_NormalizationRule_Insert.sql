-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_NormalizationRule_Insert]
	@Criteria NVARCHAR(MAX),
	@Settings NVARCHAR(MAX),
	@ID INT OUT
AS
BEGIN
	INSERT INTO PSTN_BE.NormalizationRule (Criteria, Settings, BED)
	VALUES (@Criteria, @Settings, GETDATE())
	
	SET @ID = @@IDENTITY
END