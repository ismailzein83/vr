
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_AlertAdvanceSearch] 
	-- Add the parameters for the stored procedure here
	(
		@Title nvarchar(50) = NULL,
		@Action nvarchar(50) = NULL,
		@AgreementID int = NULL,
		@Enabled char(1) = NULL,
		@Level int = NULL,
		@ApplyOn int = NULL,
		@ZoneID int = NULL,
		@Period int = NULL,
		@DataBasedOn int = NULL,
		@Type int = NULL,
		@Parameter1 int = NULL,
		@Parameter2 int = NULL,
		@Parameter3 int = NULL,
		@IsSale char(1) = NULL
) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT A.*
	 from BilateralAlert A 
	WHERE
	A.Title LIKE ISNULL('%'+@Title+'%',A.Title)
	AND A.[Action] LIKE ISNULL('%'+@Action+'%',A.[Action])
	AND A.AgreementsID = ISNULL(@AgreementID,A.AgreementsID)
	AND A.[Enabled] = ISNULL(@Enabled,A.[Enabled])
	AND A.[Level] = ISNULL(@Level,A.[Level] )
	AND A.ApplyOn = ISNULL(@ApplyOn,A.ApplyOn)
	-- AND A.ZoneID is null Or A.ZoneID = ISNULL(@ZoneID,A.ZoneID)
	AND A.Period = ISNULL(@Period,A.Period)
	AND A.DataBasedOn = ISNULL(@DataBasedOn,A.DataBasedOn)
	AND A.[Type] = ISNULL(@Type,A.[Type])
	AND A.Parameter1 = ISNULL(@Parameter1,A.Parameter1)
	AND A.Parameter2 = ISNULL(@Parameter2,A.Parameter2)
	AND A.Parameter3 = ISNULL(@Parameter3,A.Parameter3)
	AND A.IsSale = ISNULL(@IsSale,A.IsSale)
END