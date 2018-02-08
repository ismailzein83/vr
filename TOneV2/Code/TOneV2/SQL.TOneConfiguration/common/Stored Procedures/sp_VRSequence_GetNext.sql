CREATE PROCEDURE common.[sp_VRSequence_GetNext]
	@SequenceGroup varchar(255),
	@SequenceDefinitionID uniqueidentifier,
	@SequenceKey nvarchar(255),
	@InitialValue bigint
AS
BEGIN

	DECLARE @NextSequence bigint

	IF NOT EXISTS (SELECT TOP 1 NULL FROM common.[VRSequence] WITH(NOLOCK) WHERE SequenceDefinitionID = @SequenceDefinitionID AND SequenceKey = @SequenceKey AND SequenceGroup = @SequenceGroup)
	BEGIN
		BEGIN TRY
			DECLARE @EffectiveInitialValue varchar(255);
			SET @EffectiveInitialValue = CASE WHEN NOT EXISTS(SELECT TOP 1 NULL FROM common.[VRSequence] WITH(NOLOCK) WHERE SequenceDefinitionID = @SequenceDefinitionID  AND SequenceGroup = @SequenceGroup)
			THEN @InitialValue ELSE 1 END
			
			INSERT INTO common.[VRSequence] 
			(SequenceGroup,SequenceDefinitionID, SequenceKey, InitialValue, LastValue) 
			VALUES(@SequenceGroup,@SequenceDefinitionID, @SequenceKey , @EffectiveInitialValue, @EffectiveInitialValue)
			
			SELECT @NextSequence = @EffectiveInitialValue		
		END TRY
		BEGIN CATCH
		END CATCH
	END
	
	IF @NextSequence IS NULL
	BEGIN
		UPDATE common.[VRSequence]
		SET @NextSequence = LastValue + 1,
			LastValue =  LastValue + 1	
		WHERE SequenceDefinitionID = @SequenceDefinitionID AND SequenceKey = @SequenceKey AND SequenceGroup = @SequenceGroup
	END

	SELECT @NextSequence
	
END