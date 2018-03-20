-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_Country_InsertFromSource]
	@Id int,
	@Name nvarchar(255),
	@SourceID varchar(255),
	@CreatedBy int,
	@LastModifiedBy int
AS

BEGIN
	INSERT INTO common.Country(ID,Name,SourceID, CreatedBy, LastModifiedBy, LastModifiedTime)
	VALUES (@Id,@Name,@SourceID, @CreatedBy, @LastModifiedBy, GETDATE())
END