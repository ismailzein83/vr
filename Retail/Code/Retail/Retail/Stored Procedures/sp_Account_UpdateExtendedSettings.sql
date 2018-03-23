-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Account_UpdateExtendedSettings]
	@ID bigint, 
	@ExtendedSettings nvarchar(MAX),
	@LastModifiedBy int
	
AS
BEGIN
 
	Update Retail.Account
	Set 
		 ExtendedSettings = @ExtendedSettings, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()		
	Where ID = @ID

END