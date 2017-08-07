
Create PROCEDURE [genericdata].[sp_BEParentChildRelation_InsertMultiple]
	@BEParentChildRelations [genericdata].[BEParentChildRelationType] Readonly
AS
BEGIN
	INSERT [genericdata].[BEParentChildRelation] ([RelationDefinitionID], [ParentBEID], [ChildBEID], [BED], [EED]) 
	SELECT [RelationDefinitionID], [ParentBEID], [ChildBEID], [BED], [EED]
	FROM @BEParentChildRelations
END