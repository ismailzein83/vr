-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_Currency_update]
    @ID INT,
    @Name nvarchar(255),
	@Symbol nvarchar(20)
	
AS
BEGIN
	UPDATE common.Currency
	SET Name=@Name , Symbol = @Symbol
	WHERE ID = @ID
END