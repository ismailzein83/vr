-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE  [TOneWhS_BE].[sp_CarrierProfile_UpdateExtendedSettings]
	@ID int, 
	@ExtendedSettings nvarchar(MAX)
AS
 
BEGIN
	Update TOneWhS_BE.CarrierProfile
    SET
	 ExtendedSettings=@ExtendedSettings
	Where ID = @ID 
END