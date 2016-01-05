-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_Country_InsertFromSource]
	@Id int,
	@Name nvarchar(255),
	@SourceID varchar(255)
AS

BEGIN
	INSERT INTO common.Country(ID,Name,SourceID)
	VALUES (@Id,@Name,@SourceID)
END