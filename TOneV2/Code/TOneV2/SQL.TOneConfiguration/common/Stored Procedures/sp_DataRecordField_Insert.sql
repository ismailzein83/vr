-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_DataRecordField_Insert]
	@TypeID int,
	@Details varchar(max),
	@Id int out
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	Insert into [common].DataRecordField ([TypeID],[Details])
	values(@TypeID, @Details)	
	SET @Id = SCOPE_IDENTITY()

END