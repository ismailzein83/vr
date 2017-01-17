﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_ServiceType_Update]
	@ID uniqueidentifier,
	@Title NVARCHAR(255),
	@AccountBEDefinitionId Uniqueidentifier,
	@Settings NVARCHAR(MAX)
AS
BEGIN
	BEGIN
		UPDATE Retail.ServiceType
		SET Title = @Title, Settings = @Settings,AccountBEDefinitionId=@AccountBEDefinitionId
		WHERE ID = @ID
	END
END