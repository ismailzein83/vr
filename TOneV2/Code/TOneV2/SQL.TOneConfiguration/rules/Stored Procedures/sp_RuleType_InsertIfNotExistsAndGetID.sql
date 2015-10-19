-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE rules.sp_RuleType_InsertIfNotExistsAndGetID
	@RuleType varchar(255)
AS
BEGIN	
	
	--INSERT Rule Type if not exists
	INSERT INTO rules.RuleType ([Type])
	SELECT @RuleType WHERE NOT EXISTS (SELECT NULL FROM rules.RuleType WHERE [Type] = @RuleType)
	
	SELECT ID FROM rules.RuleType WHERE [Type] = @RuleType
END