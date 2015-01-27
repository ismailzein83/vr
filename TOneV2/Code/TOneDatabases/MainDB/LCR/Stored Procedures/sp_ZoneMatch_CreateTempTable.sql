CREATE PROCEDURE [LCR].[sp_ZoneMatch_CreateTempTable]
	@IsFuture bit
AS
BEGIN
	IF @IsFuture = 0
	BEGIN
		IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[ZoneMatchCurrent_old]') AND type in (N'U'))
		DROP TABLE [LCR].[ZoneMatchCurrent_old]

		IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[ZoneMatchCurrent_temp]') AND type in (N'U'))
		DROP TABLE [LCR].[ZoneMatchCurrent_temp]	
		
		CREATE TABLE [LCR].[ZoneMatchCurrent_temp](
			[OurZoneID] [int] NOT NULL,
			[SupplierZoneID] [int] NOT NULL,
			[SupplierID] [varchar](5) NOT NULL
			PRIMARY KEY CLUSTERED 
			(
				[OurZoneID] ASC,
				[SupplierZoneID] ASC
			)
		) ON [PRIMARY]
	END
	ELSE
	BEGIN
		IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[ZoneMatchFuture_old]') AND type in (N'U'))
		DROP TABLE [LCR].[ZoneMatchFuture_old]

		IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[ZoneMatchFuture_temp]') AND type in (N'U'))
		DROP TABLE [LCR].[ZoneMatchFuture_temp]	
		
		CREATE TABLE [LCR].[ZoneMatchFuture_temp](
			[OurZoneID] [int] NOT NULL,
			[SupplierZoneID] [int] NOT NULL,
			[SupplierID] [varchar](5) NOT NULL
			PRIMARY KEY CLUSTERED 
			(
				[OurZoneID] ASC,
				[SupplierZoneID] ASC
			)
		) ON [PRIMARY]
	END

END