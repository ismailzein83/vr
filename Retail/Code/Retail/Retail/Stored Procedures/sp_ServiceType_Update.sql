-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_ServiceType_Update]
	@ID INT,
	@Title NVARCHAR(255),
	@Settings NVARCHAR(MAX)
AS
BEGIN
	BEGIN
		UPDATE Retail.ServiceType
		SET Title = @Title, Settings = @Settings
		WHERE ID = @ID
	END
END