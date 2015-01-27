-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [LCR].[sp_CodeMatch_CreateTempTable]
	@IsFuture bit
AS
BEGIN

	IF @IsFuture = 0
	BEGIN
		IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[CodeMatchCurrent_old]') AND type in (N'U'))
		DROP TABLE [LCR].[CodeMatchCurrent_old]

		IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[CodeMatchCurrent_temp]') AND type in (N'U'))
		DROP TABLE [LCR].[CodeMatchCurrent_temp]
		

		CREATE TABLE [LCR].[CodeMatchCurrent_temp](
			[Code] [varchar](30) NOT NULL,
			[SupplierID] [varchar](5) NOT NULL,
			[SupplierCode] [varchar](30) NOT NULL,
			[SupplierCodeID] [bigint] NOT NULL,
			[SupplierZoneID] [int] NOT NULL
		) ON [PRIMARY]
	END
	ELSE
	BEGIN
		IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[CodeMatchFuture_old]') AND type in (N'U'))
		DROP TABLE [LCR].[CodeMatchFuture_old]

		IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[CodeMatchFuture_temp]') AND type in (N'U'))
		DROP TABLE [LCR].[CodeMatchFuture_temp]
		

		CREATE TABLE [LCR].[CodeMatchFuture_temp](
			[Code] [varchar](30) NOT NULL,
			[SupplierID] [varchar](5) NOT NULL,
			[SupplierCode] [varchar](30) NOT NULL,
			[SupplierCodeID] [bigint] NOT NULL,
			[SupplierZoneID] [int] NOT NULL
		) ON [PRIMARY]	
	END

END