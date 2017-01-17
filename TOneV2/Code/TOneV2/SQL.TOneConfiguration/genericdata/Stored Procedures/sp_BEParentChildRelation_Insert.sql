
CREATE Procedure [genericdata].[sp_BEParentChildRelation_Insert]
    @RelationDefinitionID uniqueidentifier = null,
    @ParentBEID nvarchar(255) = null,
    @ChildBEID nvarchar(255) = null,
    @BED datetime = null,
    @EED datetime = null,
	@Id int out
AS
BEGIN
	IF NOT EXISTS(select 1 from [genericdata].[BEParentChildRelation] where ParentBEID = @ParentBEID and ChildBEID = @ChildBEID)
	BEGIN
		insert into [genericdata].[BEParentChildRelation] ([RelationDefinitionID],[ParentBEID],[ChildBEID],[BED],[EED])
		values(@RelationDefinitionID, @ParentBEID, @ChildBEID, @BED, @EED)
		
		set @Id = SCOPE_IDENTITY()
	END
END