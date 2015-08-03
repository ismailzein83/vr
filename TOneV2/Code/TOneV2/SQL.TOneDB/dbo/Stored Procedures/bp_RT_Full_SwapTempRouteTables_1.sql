
CREATE PROCEDURE [dbo].[bp_RT_Full_SwapTempRouteTables]

WITH RECOMPILE
AS
BEGIN
DECLARE @Message varchar(500) 
	BEGIN TRANSACTION	
	IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[Route]') AND s.type in (N'U'))
	BEGIN
		 DROP TABLE [Route]
		 	SET @Message = CONVERT(varchar, getdate(), 121)
			EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Route was dropped', @Message
	END	
		
		EXEC sp_rename 'TempRoutes', 'Route'
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: [TempRoutes] Renamed to [Route]', @Message
		
		IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RouteOption]') AND s.type in (N'U'))
	BEGIN
			DROP TABLE [RouteOption]
			SET @Message = CONVERT(varchar, getdate(), 121)
			EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: RouteOption was dropped', @Message
	END	
		
		EXEC sp_rename 'TempRouteOptions', 'RouteOption'
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: [TempRouteOptions] Renamed to [RouteOptions]', @Message
	COMMIT TRANSACTION

      
      EXEC sp_rename N'Route.PK_TempRouteID', N'PK_RouteID', N'INDEX';
      EXEC sp_rename N'Route.IX_TempRoutes_Zone', N'IX_Route_Zone', N'INDEX';
      EXEC sp_rename N'Route.IX_TempRoutes_Updated', N'IX_Route_Updated', N'INDEX';
      EXEC sp_rename N'Route.IX_TempRoutes_Customer', N'IX_Route_Customer', N'INDEX';
      EXEC sp_rename N'Route.IX_TempRoutes_Code', N'IX_Route_Code', N'INDEX';
      
      
	 	SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Start building INDEXES for RouteOptions', @Message	  
		    			      
				  CREATE NONCLUSTERED INDEX [IDX_RouteOption_Updated] ON [dbo].[RouteOption] 
			(
				[Updated] DESC
			)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

			CREATE NONCLUSTERED INDEX [IDX_RouteOption_SupplierZoneID] ON [dbo].[RouteOption] 
			(
				[SupplierZoneID] ASC
			)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

			CREATE CLUSTERED INDEX [IDX_RouteOption_RouteID] ON [dbo].[RouteOption] 
			(
				[RouteID] ASC
			)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

	 	SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: END building INDEXES for RouteOption', @Message	  

END