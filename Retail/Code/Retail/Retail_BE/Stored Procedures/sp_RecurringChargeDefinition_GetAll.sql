
CREATE PROCEDURE [Retail_BE].[sp_RecurringChargeDefinition_GetAll]
AS
BEGIN
	SELECT ID, Name, Settings
	FROM Retail_BE.RecurringChargeDefinition with(nolock)
END