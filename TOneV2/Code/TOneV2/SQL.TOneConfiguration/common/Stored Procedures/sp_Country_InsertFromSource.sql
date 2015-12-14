-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [common].[sp_Country_InsertFromSource]
	@Name nvarchar(255),
	@Id int,
	@SourceID varchar(255)
AS

BEGIN
	INSERT INTO common.Country(ID,Name,SourceID)
	VALUES (@Id,@Name,@SourceID)
END