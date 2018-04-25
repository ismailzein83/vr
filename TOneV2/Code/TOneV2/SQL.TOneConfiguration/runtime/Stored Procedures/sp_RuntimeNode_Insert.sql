-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [runtime].[sp_RuntimeNode_Insert] 
	@Id uniqueidentifier ,
	@RuntimeNodeConfigurationID uniqueidentifier,
	@Name Nvarchar(255),
	@Setting nvarchar(max)
AS
BEGIN
	Insert into runtime.RuntimeNode(Id,RuntimeNodeConfigurationID,[Name],Settings)
	values(@Id,@RuntimeNodeConfigurationID,@Name,@Setting)

END