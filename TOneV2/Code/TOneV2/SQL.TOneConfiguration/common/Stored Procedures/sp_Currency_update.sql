-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_Currency_update]
    @ID INT,
    @Name nvarchar(255),
	@Symbol nvarchar(20),
	@LastModifiedBy int
	
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.[Currency] WHERE ID != @ID AND Symbol = @Symbol)
	BEGIN
		UPDATE common.Currency
		SET Name=@Name , Symbol = @Symbol, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
		WHERE ID = @ID
	END
END