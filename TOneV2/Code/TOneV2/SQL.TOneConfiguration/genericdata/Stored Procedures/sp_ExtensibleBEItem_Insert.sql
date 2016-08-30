-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_ExtensibleBEItem_Insert]
	@Details VARCHAR(MAX),
	@ID INT OUT
AS
BEGIN
	INSERT INTO genericdata.ExtensibleBEItem(Details)
	VALUES (@Details)
	SET @ID = SCOPE_IDENTITY() 
END