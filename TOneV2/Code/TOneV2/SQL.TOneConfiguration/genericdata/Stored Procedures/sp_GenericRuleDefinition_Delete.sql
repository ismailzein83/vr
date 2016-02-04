-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE genericdata.[sp_GenericRuleDefinition_Delete]
	@Id INT
AS
BEGIN
	DELETE FROM genericdata.GenericRuleDefinition
	WHERE ID = @Id
END