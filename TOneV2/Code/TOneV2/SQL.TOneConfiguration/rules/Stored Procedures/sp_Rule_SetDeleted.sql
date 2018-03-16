-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE [rules].[sp_Rule_SetDeleted]
	@RuleIds VARCHAR(MAX),
	@LastModifiedBy int
AS
BEGIN
	DECLARE @RuleIdsTable TABLE (RuleId int)
	INSERT INTO @RuleIdsTable (RuleId)
	SELECT ParsedString FROM [Common].[ParseStringList](@RuleIds)

	Update rules.[Rule]
	Set  [IsDeleted] = 1, [LastModifiedBy] = @LastModifiedBy, [LastModifiedTime] = GETDATE()
	WHERE ID in  (select RuleId from @RuleIdsTable)

END