-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [sec].[sp_SecurityProvider_GetDefault]
AS
BEGIN
	Select ID, Name, IsEnabled, Settings from sec.SecurityProvider
	WHERE IsDefault = 1
END