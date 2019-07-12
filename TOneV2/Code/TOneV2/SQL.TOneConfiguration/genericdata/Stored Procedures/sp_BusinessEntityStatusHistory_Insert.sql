﻿CREATE PROCEDURE [genericdata].[sp_BusinessEntityStatusHistory_Insert]
	@BusinessEntityDefinitionId uniqueidentifier,
	@BusinessEntityId varchar(50),
	@FieldName varchar(50),
	@StatusId uniqueidentifier,
	@PreviousStatusId uniqueidentifier,
	@moreInfo nvarchar(max),
	@previousMoreInfo nvarchar(max),
	@userId int
AS
BEGIN
	
	INSERT INTO [genericdata].[BusinessEntityStatusHistory] (BusinessEntityDefinitionId, BusinessEntityId,FieldName, StatusId, PreviousStatusID, StatusChangedDate, IsDeleted,MoreInfo,PreviousMoreInfo,CreatedBy)
	VALUES (@BusinessEntityDefinitionId, @BusinessEntityId, @FieldName, @StatusId, @PreviousStatusId, getdate(),0,@moreInfo,@previousMoreInfo,@userId)
	
END