-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_OperatorDeclaredInfo_Update]		
	@ID  int,
	@Settings NVARCHAR(MAX)
AS

BEGIN

	UPDATE dbo.OperatorDeclaredInfo
	SET  Settings = @Settings
	WHERE ID = @ID
	
END