-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [CDR].[sp_CDRIDManager_ReserveIDRange]
	@IsMain bit,
	@IsNegative bit,
	@NumberOfIDs int
AS
BEGIN

	IF NOT EXISTS (SELECT TOP 1 NULL FROM CDR.CDRIDManager)
	BEGIN
		DECLARE @MinInvalid bigint,@MaxInvalid bigint,@MinMain bigint,@MaxMain bigint
		SELECT @MinInvalid = MIN(ID), @MaxInvalid = MAX(ID) FROM dbo.Billing_CDR_Invalid WITH (NOLOCK)
		SELECT @MinMain = MIN(ID), @MaxMain = MAX(ID) FROM dbo.Billing_CDR_Main WITH (NOLOCK)
		
		BEGIN TRY
			IF NOT EXISTS (SELECT TOP 1 NULL FROM CDR.CDRIDManager)
			BEGIN
				INSERT INTO CDR.CDRIDManager ([IsMain] ,[IsNegative] ,[LastTakenID]) VALUES (1,0, @MaxMain)
				INSERT INTO CDR.CDRIDManager ([IsMain] ,[IsNegative] ,[LastTakenID]) VALUES (1,1, @MinMain)
				INSERT INTO CDR.CDRIDManager ([IsMain] ,[IsNegative] ,[LastTakenID]) VALUES (0,0, @MaxInvalid)
				INSERT INTO CDR.CDRIDManager ([IsMain] ,[IsNegative] ,[LastTakenID]) VALUES (0,1, @MinInvalid)
			END
		END TRY
		BEGIN CATCH
		
		END CATCH
	
	
	END

	DECLARE @StartingID bigint
	UPDATE CDR.CDRIDManager 
	SET @StartingID = CASE WHEN @IsNegative = 0 THEN LastTakenID + 1 ELSE LastTakenID - 1 END,
		LastTakenID = CASE WHEN @IsNegative = 0 THEN LastTakenID + @NumberOfIDs ELSE LastTakenID - @NumberOfIDs END
	WHERE IsMain = @IsMain AND IsNegative = @IsNegative
	
	SELECT @StartingID
END