-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [rules].[sp_Rule_Insert] 
	@TypeID INT,
	@RuleDetails varchar(MAX),
	@BED Datetime,
	@EED Datetime,
	@CreatedBy int,
	@LastModifiedBy int,
	@Id int out
	
AS
BEGIN
	Insert into [rules].[Rule] (TypeID, RuleDetails,BED,EED, CreatedBy, LastModifiedBy, LastModifiedTime)
	values(@TypeID, @RuleDetails,@BED,@EED, @CreatedBy, @LastModifiedBy, GETDATE())
	SET @Id = SCOPE_IDENTITY()
END