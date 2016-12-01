-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Agent_Update]
	@ID INT,
	@Name NVARCHAR(255),
	@Type nvarchar(255),
	@Settings NVARCHAR(MAX),
	@SourceID nvarchar(255)
AS
BEGIN

		UPDATE Retail.Agent
		SET Name = @Name, [Type] = @Type, Settings = @Settings, SourceID = @SourceID
		WHERE ID = @ID

END