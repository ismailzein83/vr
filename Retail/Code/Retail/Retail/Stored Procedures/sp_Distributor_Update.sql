-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Distributor_Update]
	@ID INT,
	@Name NVARCHAR(255),
	@Type nvarchar(255),
	@Settings NVARCHAR(MAX),
	@SourceID nvarchar(255)
AS
BEGIN
		UPDATE Retail.Distributor
		SET Name = @Name, [Type] = @Type, Settings = @Settings, SourceID = @SourceID
		WHERE ID = @ID
END