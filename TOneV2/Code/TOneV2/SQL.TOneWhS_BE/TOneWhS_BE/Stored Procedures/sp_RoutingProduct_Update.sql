-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_RoutingProduct_Update]
	@ID int,
	@Name nvarchar(255),
	@Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.RoutingProduct WHERE [Name] = @Name and ID != @ID)
	BEGIN
		Update TOneWhS_BE.RoutingProduct Set Name = @Name, Settings = @Settings
		Where ID = @ID
		
		Set @Id = @@IDENTITY
	END
END