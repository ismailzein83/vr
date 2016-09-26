-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [Retail].[sp_Distributor_GetAll]
	@ID INT,
	@Name NVARCHAR(255),
	@Type nvarchar(255),
	@Settings NVARCHAR(MAX),
	@SourceID nvarchar(255)
AS
BEGIN
	IF NOT EXISTS
	(
		SELECT 1 FROM Retail.Distributor
		WHERE ID != @ID
			AND Name = @Name
	)
	BEGIN
		UPDATE Retail.Distributor
		SET Name = @Name, [Type] = @Type, Settings = @Settings, SourceID = @SourceID
		WHERE ID = @ID
	END
END