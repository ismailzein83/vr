-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [PSTN_BE].[sp_NormalizationRule_Delete]
	@ID INT
AS
BEGIN
	DELETE FROM PSTN_BE.NormalizationRule WHERE ID = @ID
END