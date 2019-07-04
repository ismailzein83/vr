CREATE PROCEDURE [genericdata].[sp_BusinessEntityHistoryStack_Insert]
	@BusinessEntityDefinitionId uniqueidentifier,
	@BusinessEntityId varchar(50),
	@FieldName varchar(50),
	@StatusId uniqueidentifier,
	@PreviousStatusId uniqueidentifier,
	@moreInfo nvarchar(max),
	@previousMoreInfo nvarchar(max)
AS
BEGIN
	
	INSERT INTO [genericdata].[BusinessEntityHistoryStack] (BusinessEntityDefinitionId, BusinessEntityId,FieldName, StatusId, PreviousStatusID, StatusChangedDate, IsDeleted,MoreInfo,PreviousMoreInfo)
	VALUES (@BusinessEntityDefinitionId, @BusinessEntityId, @FieldName, @StatusId, @PreviousStatusId, getdate(),0,@moreInfo,@previousMoreInfo)
	
END