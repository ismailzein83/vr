-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
		
CREATE PROCEDURE [TOneWhS_Deal].[sp_Deal_Delete]
	@ID int 
AS
BEGIN
		Update TOneWhS_Deal.Deal
		Set IsDeleted = 1
		Where ID = @ID
END