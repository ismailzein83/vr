

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [rules].[sp_Rule_DeleteRuleAndRuleChanged] 
	@RuleID INT,
	@RuleTypeID INT,
	@LastModifiedBy INT,
	@ActionType INT,
	@InitialRule varchar(MAX)
AS
BEGIN
	Begin Try
	    Begin Transaction
			Update [rules].[Rule]
			set [IsDeleted] = 1, [LastModifiedBy] = @LastModifiedBy, [LastModifiedTime] = GETDATE()
			Where Id = @RuleID and TypeID = @RuleTypeID
	    
	    	IF NOT EXISTS(SELECT 1 FROM  [rules].[RuleChanged]  WHERE RuleID = @RuleID and RuleTypeId = @RuleTypeID)
				Begin
					Insert into [rules].[RuleChanged] ([RuleID], [RuleTypeID], [ActionType], [InitialRule])
					values(@RuleID, @RuleTypeID, @ActionType, @InitialRule)
				END
			ELSE
				BEGIN
					Update [rules].[RuleChanged] 
					Set ActionType = @ActionType, InitialRule = @InitialRule
					WHERE RuleID = @RuleID and RuleTypeId = @RuleTypeID
				END
	    Commit Transaction
	End Try
	Begin CATCH
	    declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
	    select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
	    rollback Transaction;
	    raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState);
	End CATCH
END