-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [sec].[sp_SecurityProvider_SetDefault]
	@SecurityProviderId uniqueidentifier
AS
BEGIN
	UPDATE sec.SecurityProvider
	SET IsDefault = CASE WHEN ID = @SecurityProviderId THEN 1 ELSE 0 END
	WHERE (IsDefault = 1 OR ID =@SecurityProviderId)
END