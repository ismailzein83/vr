-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[bp_RT_Part_InsertRouteBatch]
	 @CheckToD BIT = 1 
	,@CheckSpecialRequests BIT = 1 
	,@CheckRouteBlocks BIT = 1
	,@IncludeBlockedZones BIT = 1
	,@Targets VARCHAR(500)
	,@TargetsType VARCHAR(500)
	,@TargetCustomers VARCHAR(500)
	,@BatchID INT OUTPUT
	,@MaxSuppliersPerRoute INT  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
INSERT INTO [RouteBuildBatch]
           ([CheckSpecialRequests]
           ,[CheckRouteBlocks]
           ,[CheckTOD]
           ,[IncludeBlockedZones]
           ,[Type]
           ,[Targets]
           ,[TargetCustomers]
           ,[IsSynched]
           ,MaxSuppliersPerRoute)
     VALUES
           (@CheckSpecialRequests
           ,@CheckRouteBlocks
           ,@CheckToD
           ,@IncludeBlockedZones
           ,@TargetsType
           ,@Targets
           ,@TargetCustomers
           ,0
           ,@MaxSuppliersPerRoute)
          SET @BatchID = SCOPE_IDENTITY()
END