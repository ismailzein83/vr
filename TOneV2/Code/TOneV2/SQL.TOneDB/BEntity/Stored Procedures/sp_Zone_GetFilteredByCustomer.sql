-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Zone_GetFilteredByCustomer]
	@CustomerId varchar(5),
	@NameFilter nvarchar(255),
	@WhenDate Datetime
AS
BEGIN
	SET NOCOUNT ON;
	
    SELECT Z.ZoneID
		  ,Z.Name
    FROM Zone Z WITH(NOLOCK)
    INNER JOIN Rate r ON z.ZoneID=r.ZoneID
    INNER JOIN PriceList p ON r.PriceListID=p.PriceListID    
    WHERE   
     p.CustomerID = @CustomerId and Z.Name like('%'+ @NameFilter+'%')
     and z.BeginEffectiveDate <= @WhenDate and (z.EndEffectiveDate is NULL or z.EndEffectiveDate > @WhenDate)
     ORDER BY Z.Name
END