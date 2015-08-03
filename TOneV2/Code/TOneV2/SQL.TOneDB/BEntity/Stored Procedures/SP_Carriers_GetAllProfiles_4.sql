-- =============================================
-- Author:		<Rayyan Kazma>
-- Create date: <2015-07-21>
-- Description:	<procedure for retrieving all profiles>
-- =============================================
CREATE PROCEDURE BEntity.SP_Carriers_GetAllProfiles
	@Name VARCHAR(30) =  NULL,
	@CompanyName VARCHAR(50) = NULL,
	@BillingEmail VARCHAR(150) = NULL,
	@From INT = 0,
	@To INT = 50
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	;WITH CarrierProfileData AS
	(
			SELECT 
				cp.ProfileID, 
				cp.Name, 
				cp.CompanyName, 
				cp.BillingEmail, 
				COUNT(cp.ProfileID) AS Accounts,
				ROW_NUMBER() OVER(ORDER BY cp.ProfileID ASC) as  RowNumber
			FROM CarrierProfile cp
			INNER JOIN CarrierAccount ca ON cp.ProfileID = ca.ProfileID
			WHERE cp.IsDeleted = 'N'
			AND (@Name IS NULL OR cp.Name = @Name)
			AND (@CompanyName IS NULL OR cp.CompanyName = @CompanyName)
			AND (@BillingEmail IS NULL OR cp.BillingEmail = @BillingEmail)
			GROUP BY cp.ProfileID, cp.Name, cp.CompanyName, cp.BillingEmail
	 )
	 SELECT * FROM CarrierProfileData cp
	 WHERE cp.RowNumber BETWEEN @From AND @To
			
END