CREATE PROCEDURE [VR_Invoice].[sp_InvoiceSequence_GetNext]
	@InvoiceTypeID uniqueidentifier,
	@SequenceKey nvarchar(255),
	@InitialValue bigint
AS
BEGIN

	DECLARE @NextSequence bigint

	IF NOT EXISTS (SELECT TOP 1 NULL FROM [VR_Invoice].[InvoiceSequence] WITH(NOLOCK) WHERE InvoiceTypeID = @InvoiceTypeID AND SequenceKey = @SequenceKey)
	BEGIN
		BEGIN TRY		
			INSERT INTO [VR_Invoice].[InvoiceSequence] 
			(InvoiceTypeID, SequenceKey, InitialValue, LastValue) 
			VALUES(@InvoiceTypeID, @SequenceKey , @InitialValue, @InitialValue)

			SELECT @NextSequence = @InitialValue		
		END TRY
		BEGIN CATCH
		END CATCH
	END
	
	
	IF @NextSequence IS NULL
	BEGIN
		UPDATE [VR_Invoice].[InvoiceSequence]
		SET @NextSequence = LastValue + 1,
			LastValue =  LastValue + 1	
		WHERE InvoiceTypeID = @InvoiceTypeID AND SequenceKey = @SequenceKey
	END

	SELECT @NextSequence
	
END