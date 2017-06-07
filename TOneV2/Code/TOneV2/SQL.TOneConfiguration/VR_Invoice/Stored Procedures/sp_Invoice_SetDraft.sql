create PROCEDURE [VR_Invoice].[sp_Invoice_SetDraft]
	@ID bigint,
	@IsDraft  bit
AS
BEGIN
	
	UPDATE [VR_Invoice].[Invoice]
    SET IsDraft = @IsDraft
    WHERE ID = @ID
END