-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[SP_Switch_GetFilteredSwitches](@switchName nvarchar(50)=null,@rowFrom int =0,@rowTo int=50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	
	
;WITH 
SwitchsData AS 
(
SELECT S.SwitchID, 
	S.Name,
	S.Symbol,
	S.Description,
	S.LastCDRImportTag,
	S.LastImport,
	S.LastAttempt,
	S.Enable_CDR_Import,
	S.Enable_Routing,
	S.LastRouteUpdate,
	ROW_NUMBER() OVER(ORDER BY S.SwitchID ASC) as  RowNumber
	From Switch S 
	where  S.Name like '%' + isnull(@switchName,S.Name) + '%' 
)

   SELECT * from SwitchsData S
	where 
	 S.RowNumber between @rowFrom and @rowTo
	
END