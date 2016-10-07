-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_ExtensibleBEItem_Insert]
	@ID uniqueidentifier,
	@Details VARCHAR(MAX)

AS
BEGIN
	INSERT INTO genericdata.ExtensibleBEItem(Details)
	VALUES (@Details)

END