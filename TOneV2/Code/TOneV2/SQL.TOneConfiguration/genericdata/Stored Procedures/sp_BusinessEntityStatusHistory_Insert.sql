CREATE PROCEDURE [genericdata].[sp_BusinessEntityStatusHistory_Insert]
	@BusinessEntityDefinitionId uniqueidentifier,
	@BusinessEntityId varchar(50),
	@FieldName varchar(50),
	@StatusId uniqueidentifier,
	@PreviousStatusId uniqueidentifier
AS
BEGIN
	
	INSERT INTO [genericdata].[BusinessEntityStatusHistory] (BusinessEntityDefinitionId, BusinessEntityId,FieldName, StatusId, PreviousStatusID, StatusChangedDate, IsDeleted)
	VALUES (@BusinessEntityDefinitionId, @BusinessEntityId, @FieldName, @StatusId, @PreviousStatusId, getdate(),0)
	
END