﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE genericdata.sp_GenericBusinessEntity_Insert 
	@BusinessEntityDefinitionID INT,
	@Details VARCHAR(MAX),
	@ID BigInt out
	
AS
BEGIN
	Insert into genericdata.GenericBusinessEntity (BusinessEntityDefinitionID, Details)
	values(@BusinessEntityDefinitionID, @Details)
	SET @ID = @@IDENTITY

END