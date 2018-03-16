-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [rules].[sp_Rule_GetByType] 
	@TypeID INT
AS
BEGIN
	SELECT	r.ID, r.TypeID, r.RuleDetails, r.BED, r.EED, r.IsDeleted, r.CreatedTime, r.CreatedBy, r.LastModifiedBy, r.LastModifiedTime
	FROM	[rules].[Rule] r  WITH(NOLOCK) 
	WHERE	r.TypeID = @TypeID --and ISNULL(IsDeleted, 0) = 0
END