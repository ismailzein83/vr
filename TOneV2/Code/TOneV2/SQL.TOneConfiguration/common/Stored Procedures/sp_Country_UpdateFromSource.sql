-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_Country_UpdateFromSource]
	@ID int,
	@Name nvarchar(255),
	@LastModifiedBy int
AS
BEGIN

	Update common.[Country]
		Set Name = @Name, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
		Where ID = @ID
END