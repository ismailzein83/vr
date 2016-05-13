-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_CloudAuthServer_Update] 
	@Settings nvarchar(MAX),
	@ApplicationIdentification nvarchar(MAX)
AS
BEGIN
IF EXISTS(select 1 from sec.[CloudAuthServer])
	BEGIN
	update [sec].[CloudAuthServer] set 
			[Settings] =@Settings,
			[ApplicationIdentification] =@ApplicationIdentification
	END
END