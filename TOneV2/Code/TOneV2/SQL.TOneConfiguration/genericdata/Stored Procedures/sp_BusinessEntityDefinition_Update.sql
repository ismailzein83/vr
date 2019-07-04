-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_BusinessEntityDefinition_Update] 
	@ID uniqueidentifier ,
	@Name nvarchar(900),
	@Title  nvarchar(1000),
	@DevProjectId uniqueidentifier,
	@Settings nvarchar(max)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
IF NOT EXISTS(select 1 from genericdata.BusinessEntityDefinition where Name = @Name and ID!=@ID) 
	BEGIN
	UPDATE	genericdata.BusinessEntityDefinition
	SET	    Name = @Name,
			Title =@Title,
			DevProjectId=@DevProjectId,
			Settings = @Settings,
	
			LastModifiedTime = GETDATE()
	WHERE	ID = @ID
	END
END