-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_OperatorDeclaredInfo_Insert]	
	@Settings NVARCHAR(MAX),
	@ID INT OUT
AS

BEGIN
	INSERT INTO dbo.OperatorDeclaredInfo ( Settings, CreatedTime)
	VALUES (@Settings, GETDATE())
	SET @ID = @@IDENTITY
END