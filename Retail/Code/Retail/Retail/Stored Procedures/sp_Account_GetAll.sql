-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Account_GetAll]
AS
BEGIN
	SELECT	ID, Name, [TypeID], Settings, [StatusID] ,ParentID, SourceID
	FROM	Retail.Account  with(nolock)
END