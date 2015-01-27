-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [LCR].[sp_ZoneMatch_SwapTableWithTemp]
	@IsFuture bit
AS
BEGIN
	DECLARE @TableNamePart varchar(50), 
			@TableName varchar(50), @TableName_Temp varchar(50), @TableName_Old varchar(50),
			@TableName_New varchar(50)
	
	IF @IsFuture = 1 SET @TableNamePart = 'Future' ELSE SET @TableNamePart = 'Current'
	
	SELECT	@TableName = 'LCR.ZoneMatch' + @TableNamePart, 
			@TableName_Temp = 'LCR.ZoneMatch' + @TableNamePart + '_temp',
			@TableName_Old = 'ZoneMatch' + @TableNamePart + '_old',
			@TableName_New = 'ZoneMatch' + @TableNamePart
			
	BEGIN TRY
		BEGIN TRANSACTION

        EXEC sp_rename @TableName, @TableName_Old
		EXEC sp_rename @TableName_Temp, @TableName_New

		COMMIT TRANSACTION -- Transaction Success!
	END TRY
	BEGIN CATCH
		declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
		select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
		rollback TRANSACTION;
		raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END