
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_OperatorAccount_Insert]
	@Name nvarchar(255),
	@OperatorProfileId INT,
	@CustomerSettings nvarchar(MAX),
	@SupplierSettings nvarchar(MAX),
	@OperatorAccountSettings nvarchar(MAX),
	@Id int out
AS
BEGIN
SET @id =0;
IF NOT EXISTS(SELECT 1 FROM dbo.OperatorAccount WHERE [NameSuffix] = @Name and [OperatorProfileID]=@OperatorProfileId)
	BEGIN
		Insert into dbo.OperatorAccount([NameSuffix],[OperatorProfileID],[SupplierSettings] ,[CustomerSettings],[OperatorAccountSettings] )
		Values(@Name,@OperatorProfileId,  @SupplierSettings,@CustomerSettings,@OperatorAccountSettings)
	
		Set @Id = @@IDENTITY
	END
END