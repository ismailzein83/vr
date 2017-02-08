-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE Retail.[sp_Account_UpdateExtendedSettings]
	@ID bigint, 
	@ExtendedSettings nvarchar(MAX)
	
AS
BEGIN
 
	Update Retail.Account
	Set 
		 ExtendedSettings = @ExtendedSettings		
	Where ID = @ID

END