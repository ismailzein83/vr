-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CarrierAccount_UpdateExtendedSettings]
	@ID int, 
	@ExtendedSettings nvarchar(MAX)
	
AS
BEGIN
 
	Update TOneWhS_BE.CarrierAccount
	Set 
		 ExtendedSettings = @ExtendedSettings		
	Where ID = @ID

END