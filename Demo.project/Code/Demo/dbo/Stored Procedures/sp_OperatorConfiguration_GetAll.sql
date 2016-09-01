



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_OperatorConfiguration_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT config.[ID]
		  ,config.[OperatorID]
		  ,config.[Volume]
		  ,config.[CDRDirection]
		  ,config.[Percentage]
		  ,config.[Amount]
		  ,config.[Currency]
		  ,config.[FromDate]
		  ,config.[ToDate]
		  ,config.[Notes]
		  ,config.[ServiceSubTypeSettings]
		  ,config.[DestinationGroup]
		  ,config.[InterconnectOperator]

	FROM	[dbo].OperatorConfiguration  as config WITH(NOLOCK) 
END