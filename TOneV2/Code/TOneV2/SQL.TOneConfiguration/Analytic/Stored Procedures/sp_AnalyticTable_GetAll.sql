﻿

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Analytic].[sp_AnalyticTable_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	at.[ID],at.DevProjectID,at.[Name],at.[Settings],at.[MeasureStyles],at.[PermanentFilter]      
    FROM	[Analytic].[AnalyticTable] at WITH(NOLOCK) 
END