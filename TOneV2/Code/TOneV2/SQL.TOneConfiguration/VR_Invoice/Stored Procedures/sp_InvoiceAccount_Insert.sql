-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [VR_Invoice].[sp_InvoiceAccount_Insert]
	-- Add the parameters for the stored procedure here
	@InvoiceTypeId uniqueidentifier,
	@PartnerId varchar(50),
	@BED datetime = NULL,
	@EED datetime = NULL,
	@Status int,
	@IsDeleted bit,
	@ID bigint out 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
IF NOT EXISTS(select 1 from [VR_Invoice].InvoiceAccount where InvoiceTypeId = @InvoiceTypeId and PartnerId = @PartnerId )
	BEGIN	
		INSERT INTO [VR_Invoice].InvoiceAccount(InvoiceTypeId,PartnerId,BED,EED,[Status],IsDeleted) 
		VALUES (@InvoiceTypeId,@PartnerId,@BED,@EED,@Status,@IsDeleted)
		SET @ID = SCOPE_IDENTITY()
	END
END