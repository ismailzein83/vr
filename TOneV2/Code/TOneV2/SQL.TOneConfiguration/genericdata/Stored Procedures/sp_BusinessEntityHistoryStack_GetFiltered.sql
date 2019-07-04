﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_BusinessEntityHistoryStack_GetFiltered]
@BusinessEntityDefinitionId uniqueidentifier,
	@BusinessEntityId varchar(50)
AS
BEGIN
	SELECT	sh.ID,sh.BusinessEntityDefinitionID,sh.BusinessEntityID,sh.FieldName,sh.IsDeleted,sh.CreatedTime,sh.MoreInfo,sh.PreviousMoreInfo,sh.PreviousStatusID,sh.StatusChangedDate,sh.StatusID
	FROM	[genericdata].[BusinessEntityHistoryStack] sh WITH(NOLOCK) 
	where sh.BusinessEntityDefinitionID=@BusinessEntityDefinitionId AND sh.BusinessEntityID=@BusinessEntityId
END