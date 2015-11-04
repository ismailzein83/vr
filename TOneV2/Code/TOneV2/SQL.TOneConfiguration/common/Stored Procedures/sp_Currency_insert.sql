-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_Currency_insert]
    @Name nvarchar(255),
	@Symbol nvarchar(20),
	@id INT OUT
AS
BEGIN

	INSERT INTO common.Currency(Name,Symbol)
	VALUES (@Name,@Symbol)
	
	SET @id = @@IDENTITY
END