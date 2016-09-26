
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [Retail].sp_Distributor_GetAll
AS
BEGIN
	SELECT	ID, Name, Type, Settings, SourceID
	FROM	Retail.Distributor  with(nolock)
END