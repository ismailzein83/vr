-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Code_GetBySupplier]
	-- Add the parameters for the stored procedure here
	@When datetime,
	@SupplierId varchar(10),
	@Code char
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
   SELECT 
                        C.ID, 
                        C.Code, 
                        C.BeginEffectiveDate, 
                        C.EndEffectiveDate,
                        C.ZoneID, 
                        Z.CodeGroup, 
                        Z.Name,
                        Z.SupplierID,
                        Z.ServicesFlag,
                        Z.BeginEffectiveDate as ZoneBED,
                        Z.EndEffectiveDate as ZoneEED
                    FROM Code C WITH(NOLOCK), Zone Z WITH(NOLOCK),CarrierAccount CA
                    WHERE C.ZoneID = Z.ZoneID 
                        AND Z.SupplierID = CA.CarrierAccountID AND CA.ActivationStatus = 2
                         AND (C.BeginEffectiveDate<=@When and (C.EndEffectiveDate  IS NULL or C.EndEffectiveDate>@When))
                           AND (z.BeginEffectiveDate<=@When and (z.EndEffectiveDate  IS NULL or z.EndEffectiveDate>@When))
                      --  AND (C.EndEffectiveDate IS NULL OR (C.EndEffectiveDate > '01/01/2013' And C.BeginEffectiveDate<>C.EndEffectiveDate)) 
                       -- AND (Z.EndEffectiveDate IS NULL OR (Z.EndEffectiveDate >'01/01/2013'And Z.BeginEffectiveDate<>Z.EndEffectiveDate)) 
                        AND SupplierID = @SupplierId
                        AND c.Code like @Code +'%'           
                    ORDER BY Z.SupplierID, C.Code, C.BeginEffectiveDate DESC
END