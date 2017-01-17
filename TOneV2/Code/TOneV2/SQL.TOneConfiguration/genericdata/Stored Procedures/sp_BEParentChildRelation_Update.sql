Create Procedure [genericdata].[sp_BEParentChildRelation_Update]
	@Id int ,
    @RelationDefinitionID uniqueidentifier,
    @ParentBEID nvarchar(255),
    @ChildBEID nvarchar(255),
    @BED datetime,
    @EED datetime
AS
BEGIN
	IF NOT EXISTS(SELECT 1 from [genericdata].[BEParentChildRelation] where ID != @ID and ParentBEID = @ParentBEID and ChildBEID = @ChildBEID)
	BEGIN
		update [Retail_BE].[BEParentChildRelation]
		set RelationDefinitionID = @RelationDefinitionID, ParentBEID = @ParentBEID, ChildBEID = @ChildBEID, BED = @BED, EED =@EED
		where ID = @ID
	END
END