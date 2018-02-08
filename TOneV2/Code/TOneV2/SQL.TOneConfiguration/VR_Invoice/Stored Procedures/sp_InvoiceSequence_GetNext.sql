CREATE PROCEDURE [VR_Invoice].[sp_InvoiceSequence_GetNext]
	@SequenceGroup varchar(255),
	@InvoiceTypeID uniqueidentifier,
	@SequenceKey nvarchar(255),
	@InitialValue bigint
AS
BEGIN

	DECLARE @NextSequence bigint

	IF NOT EXISTS (SELECT TOP 1 NULL FROM [VR_Invoice].[InvoiceSequence] WITH(NOLOCK) WHERE InvoiceTypeID = @InvoiceTypeID AND SequenceKey = @SequenceKey AND SequenceGroup = @SequenceGroup)
	BEGIN
		BEGIN TRY
			DECLARE @EffectiveInitialValue varchar(255);
			SET @EffectiveInitialValue = CASE WHEN NOT EXISTS(SELECT TOP 1 NULL FROM [VR_Invoice].[InvoiceSequence] WITH(NOLOCK) WHERE InvoiceTypeID = @InvoiceTypeID  AND SequenceGroup = @SequenceGroup)
			THEN @InitialValue ELSE 1 END
			
			INSERT INTO [VR_Invoice].[InvoiceSequence] 
			(SequenceGroup,InvoiceTypeID, SequenceKey, InitialValue, LastValue) 
			VALUES(@SequenceGroup,@InvoiceTypeID, @SequenceKey , @EffectiveInitialValue, @EffectiveInitialValue)
			
			SELECT @NextSequence = @EffectiveInitialValue		
		END TRY
		BEGIN CATCH
		END CATCH
	END
	
	IF @NextSequence IS NULL
	BEGIN
		UPDATE [VR_Invoice].[InvoiceSequence]
		SET @NextSequence = LastValue + 1,
			LastValue =  LastValue + 1	
		WHERE InvoiceTypeID = @InvoiceTypeID AND SequenceKey = @SequenceKey AND SequenceGroup = @SequenceGroup
	END

	SELECT @NextSequence
	
END