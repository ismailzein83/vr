

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [rules].[sp_Rule_InsertRuleAndRuleChanged] 
	@RuleTypeID INT,
	@RuleDetails varchar(MAX),
	@BED Datetime,
	@EED Datetime,
	@ActionType INT,
	@Id int out
AS
BEGIN
	Begin Try
	    Begin Transaction
			Insert into [rules].[Rule] (TypeID, RuleDetails, BED, EED)
			values(@RuleTypeID, @RuleDetails, @BED, @EED)
			SET @Id = SCOPE_IDENTITY()

			Insert into [rules].[RuleChanged] ([RuleID], [RuleTypeID], [ActionType])
			values(@Id, @RuleTypeID, @ActionType)
	    Commit Transaction
	End Try
	Begin CATCH
	    declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
	    select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
	    rollback Transaction;
	    raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState);
	End CATCH
END