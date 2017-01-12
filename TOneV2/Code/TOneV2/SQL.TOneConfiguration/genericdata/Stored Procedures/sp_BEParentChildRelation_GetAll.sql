
Create Procedure [genericdata].[sp_BEParentChildRelation_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	select	[ID], [RelationDefinitionID], [ParentBEID], [ChildBEID], [BED], [EED]
	from	[genericdata].[BEParentChildRelation] WITH(NOLOCK)
END