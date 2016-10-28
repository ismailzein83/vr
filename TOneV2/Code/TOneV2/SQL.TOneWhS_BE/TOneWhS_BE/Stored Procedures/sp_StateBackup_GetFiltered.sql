-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_StateBackup_GetFiltered]
@ConfigID as UniqueIdentifier,
@From DateTime,
@To DateTime
AS
BEGIN
	SELECT	[ID], ConfigID, [Info], [BackupDate], [RestoreDate]
    FROM [TOneV2_Dev].[TOneWhS_BE].[StateBackup] with(nolock)
	Where (@ConfigID is null or ConfigID = @ConfigID) and (@From is null or BackupDate >= @From) and 
		  (@To is null or BackupDate <= @To)
	
END