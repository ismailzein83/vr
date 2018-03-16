

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [rules].[sp_Rule_UpdateRuleAndRuleChanged]
	@RuleID INT,
	@RuleTypeID INT,
	@RuleDetails VARCHAR(MAX),
	@BED Datetime,
	@EED Datetime,
	@LastModifiedBy int,
	@ActionType INT,
	@InitialRule varchar(MAX),
	@AdditionalInformation varchar(MAX)

AS
BEGIN
	Begin Try
	    Begin Transaction
			Update rules.[Rule]
			Set   TypeID = @RuleTypeID,
				  RuleDetails = @RuleDetails,
				  BED = @BED,
				  EED = @EED,
				  LastModifiedBy = @LastModifiedBy,
				  LastModifiedTime = GETDATE()
			WHERE ID = @RuleID

			IF NOT EXISTS(SELECT 1 FROM  [rules].[RuleChanged]  WHERE RuleID = @RuleID and RuleTypeId = @RuleTypeID)
				Begin
					Insert into [rules].[RuleChanged] ([RuleID], [RuleTypeID], [ActionType], [InitialRule], [AdditionalInformation])
					values(@RuleID, @RuleTypeID, @ActionType, @InitialRule, @AdditionalInformation)
				END
			ELSE
				BEGIN
					Update [rules].[RuleChanged] 
					Set  AdditionalInformation = @AdditionalInformation
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