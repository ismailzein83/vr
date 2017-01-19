CREATE Procedure [genericdata].[sp_BEParentChildRelation_Insert]
    @RelationDefinitionID uniqueidentifier,
    @ParentBEID nvarchar(255),
    @ChildBEID nvarchar(255),
    @BED datetime,
    @EED datetime,
	@Id int out
AS
BEGIN
	IF NOT EXISTS(select 1 from [genericdata].[BEParentChildRelation] 
			      where RelationDefinitionID = @RelationDefinitionID and ParentBEID = @ParentBEID and ChildBEID = @ChildBEID)
	BEGIN
		insert into [genericdata].[BEParentChildRelation] ([RelationDefinitionID],[ParentBEID],[ChildBEID],[BED],[EED])
		values(@RelationDefinitionID, @ParentBEID, @ChildBEID, @BED, @EED)
		
		set @Id = SCOPE_IDENTITY()
	END
END