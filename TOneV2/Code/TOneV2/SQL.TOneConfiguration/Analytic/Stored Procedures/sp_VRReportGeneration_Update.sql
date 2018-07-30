CREATE PROCEDURE [Analytic].[sp_VRReportGeneration_Update]
	@ReportId bigint,
	@Name nvarchar(255),
	@Description nvarchar(1000),
	@AccessLevel int,
	@LastModifiedBy bigint,
	@Settings nvarchar(max)
AS
BEGIN
		UPDATE [Analytic].[VRReportGeneration] 
		SET  Name=@Name,[Description]=@Description,Settings=@Settings,AccessLevel=@AccessLevel,LastModifiedBy=@LastModifiedBy,LastModifiedTime=GETDATE() WHERE ID = @ReportId;
	
END