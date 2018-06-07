-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CarrierProfile_Insert]
	@Name nvarchar(255),
	@Settings nvarchar(MAX),
	@CreatedBy int,
	@LastModifiedBy int,
	@Id int out
AS
BEGIN
IF NOT EXISTS(select 1 from  TOneWhS_BE.CarrierProfile where Name = @Name and ISNULL(IsDeleted,0) = 0)
BEGIN
	Insert into TOneWhS_BE.CarrierProfile([Name],[Settings],[CreatedBy],[LastModifiedBy],[LastModifiedTime])
	Values(@Name,@Settings,@CreatedBy, @LastModifiedBy, GETDATE())
	
	Set @Id = SCOPE_IDENTITY()
	END
END