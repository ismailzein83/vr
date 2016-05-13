-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_CloudAuthServer_Insert] 
	@Settings nvarchar(MAX),
	@ApplicationIdentification nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(select 1 from sec.[CloudAuthServer])
	BEGIN
		Insert into [sec].[CloudAuthServer] ([Settings], [ApplicationIdentification] )
		values(@Settings, @ApplicationIdentification)
	END
END