-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
		
CREATE PROCEDURE [common].[sp_VRRule_Delete]
		@VRRuleIds varchar(max)
AS
BEGIN
	    DECLARE @VRRuleIdsTable TABLE (VRRuleId BIGINT)
		INSERT INTO @VRRuleIdsTable (VRRuleId)
		select Convert(BIGINT, ParsedString) from [common].[ParseStringList] (@VRRuleIds)

		Update [common].[VRRule] 
		Set IsDeleted = 1
		Where ID in (select VRRuleId from @VRRuleIdsTable) 
END