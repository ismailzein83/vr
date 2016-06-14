-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [Retail].[sp_ServiceType_GetAll]
AS
BEGIN
	SELECT ID, Name, Title, Settings
	FROM Retail.ServiceType
END