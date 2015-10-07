-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [PSTN_BE].[sp_SwitchTrunk_GetBySymbol]
	@Symbol varchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT t.ID, t.Name
	FROM PSTN_BE.SwitchTrunk t
	WHERE t.Symbol = @Symbol
	
	SET NOCOUNT	OFF;
END