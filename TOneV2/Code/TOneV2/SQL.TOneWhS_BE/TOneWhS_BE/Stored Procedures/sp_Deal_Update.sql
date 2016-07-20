-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_Deal_Update]
	@ID int,
	@Settings nvarchar(MAX)
AS
BEGIN


	Update TOneWhS_BE.Deal
	Set Settings=@Settings
	Where ID = @ID


END