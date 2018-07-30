CREATE PROCEDURE [Analytic].[sp_VRReportGeneration_Insert]
	@Name nvarchar(255),
	@Description nvarchar(255),
	@Settings nvarchar(max),
	@AccessLevel int,
	@CreatedBy bigint,	
	@ID bigint out
AS
BEGIN
	if not exists(select 1 from VRReportGeneration where Name=@Name)
	BEGIN
		INSERT INTO [Analytic].[VRReportGeneration] (Name,[Description],[Settings],AccessLevel,CreatedBy,LastModifiedBy)
		VALUES(@Name,@Description,@Settings,@AccessLevel,@CreatedBy,@CreatedBy)
		SET @ID = SCOPE_IDENTITY();
	END
END