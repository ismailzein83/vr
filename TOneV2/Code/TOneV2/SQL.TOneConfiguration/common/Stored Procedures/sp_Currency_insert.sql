-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_Currency_insert]
    @Name nvarchar(255),
	@Symbol nvarchar(20),
	@SourceID varchar(20),
	@CreatedBy int,
	@LastModifiedBy int,
	@id int OUT
AS
BEGIN
SET @id =0;
IF NOT EXISTS(SELECT 1 FROM common.[Currency] WHERE Symbol = @Symbol)
	BEGIN
	INSERT INTO common.Currency(Name,Symbol,SourceID, CreatedBy, LastModifiedBy, LastModifiedTime)
	VALUES (@Name,@Symbol,@SourceID, @CreatedBy, @LastModifiedBy, GETDATE())
	
	SET @id = SCOPE_IDENTITY()
	END
END