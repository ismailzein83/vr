




-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[prCollectNormalizedCDR]
AS

declare @MaxRecordID INT
SELECT @MaxRecordID=ISNULL(MAX(SwitchRecordId),0) FROM NormalCDR 

INSERT INTO [NormalCDR] ([ConnectDateTime] ,[DisconnectDateTime]  ,[DurationInSeconds]  ,[In_Trunk]  ,[Out_Trunk]  ,[In_Type] ,[Out_Type] ,[A_Temp] ,[B_Temp] ,[Switch]  ,[SwitchId] ,[SwitchRecordId])
    SELECT                   [ConnectDateTime] ,[DisconnectDateTime]  ,[DurationInSeconds]  ,[IN_TRUNK]  ,[OUT_TRUNK]  ,[in_type] ,[out_type] ,[A_temp]  ,[B_temp],[switch]  ,[SourceID] ,[ID]  FROM [CDR]
      where ignore='0' AND ID > @MaxRecordID
   

ALTER INDEX ALL ON NormalCDR
REBUILD WITH (FILLFACTOR = 80, SORT_IN_TEMPDB = ON,
              STATISTICS_NORECOMPUTE = ON);