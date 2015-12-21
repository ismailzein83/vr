

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_AccountManager_AssignCarriers]
	@UpdatedCarriers [TOneWhS_BE].[UpdatedAccountManagerCarriersType] READONLY
AS
BEGIN	

	-- insert new carriers
	Insert into [TOneWhS_BE].AccountManager 
	([UserId], [CarrierAccountID], [RelationType])

	SELECT updated.[UserId], updated.[CarrierAccountID], updated.[RelationType] 
	FROM @UpdatedCarriers updated 
	LEFT JOIN [TOneWhS_BE].AccountManager AM
	on updated.CarrierAccountId = AM.CarrierAccountId AND updated.[RelationType] = AM.[RelationType]
	Where AM.CarrierAccountId is Null And updated.[Status] = 1
	
	 -- delete removed carriers
	DELETE [TOneWhS_BE].AccountManager FROM [TOneWhS_BE].AccountManager AM
	Right JOIN @UpdatedCarriers updated
	ON AM.CarrierAccountId = updated.CarrierAccountId AND AM.[RelationType] = updated.[RelationType]
	Where AM.CarrierAccountId is not Null And updated.[Status] = 0
	
END