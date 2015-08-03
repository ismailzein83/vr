

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_AccountManager_AssignCarriers]
	@UpdatedCarriers BEntity.[UpdatedAccountManagerCarriersType] READONLY
AS
BEGIN	

	-- insert new carriers
	Insert into [BEntity].AccountManager 
	([UserId], [CarrierAccountID], [RelationType])

	SELECT updated.[UserId], updated.[CarrierAccountID], updated.[RelationType] 
	FROM @UpdatedCarriers updated 
	LEFT JOIN BEntity.AccountManager AM
	on updated.CarrierAccountId = AM.CarrierAccountId AND updated.[RelationType] = AM.[RelationType]
	Where AM.CarrierAccountId is Null And updated.[Status] = 1
	
	 -- delete removed carriers
	DELETE [BEntity].AccountManager FROM [BEntity].AccountManager AM
	Right JOIN @UpdatedCarriers updated
	ON AM.CarrierAccountId = updated.CarrierAccountId AND AM.[RelationType] = updated.[RelationType]
	Where AM.CarrierAccountId is not Null And updated.[Status] = 0
	
END