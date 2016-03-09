﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE genericdata.sp_GenericBusinessEntity_GetByDefinition
		@BusinessEntityDefinitionID INT
AS
BEGIN
	SELECT gbe.ID,
		   gbe.BusinessEntityDefinitionID,
		   gbe.Details 
	FROM genericdata.GenericBusinessEntity gbe 
	WHERE gbe.BusinessEntityDefinitionID=@BusinessEntityDefinitionID

END