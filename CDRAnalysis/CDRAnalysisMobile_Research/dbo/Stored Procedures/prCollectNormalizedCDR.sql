-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


CREATE PROCEDURE [dbo].[prCollectNormalizedCDR]
AS

declare @MaxRecordID INT
SELECT @MaxRecordID=ISNULL(MAX(SwitchRecordId),0) FROM NormalCDR 


INSERT INTO [NormalCDR] (
            [MSISDN]
           ,[IMSI]
           ,[ConnectDateTime]
           ,[Destination]
           ,[DurationInSeconds]
           ,[DisconnectDateTime]
           ,[Call_Class]
           ,[IsOnNet]
           ,[Call_Type]
           ,[Sub_Type]
           ,[IMEI]
           ,[BTS_Id]
           ,[Cell_Id]
           ,[Up_Volume]
           ,[Down_Volume]
           ,[Cell_Latitude]
           ,[Cell_Longitude]
           ,[In_Trunk]
           ,[Out_Trunk]
           ,[Service_Type]
           ,[Service_VAS_Name]
           ,[SwitchRecordId])
    SELECT  [MSISDN]
           ,[IMSI]
           ,[ConnectDateTime]
           ,[Destination]
           ,[DurationInSeconds]
           ,[DisconnectDateTime]
           ,[Call_Class]
           ,NULL
           ,[Call_Type]
           ,[Sub_Type]
           ,[IMEI]
           ,[BTS_Id]
           ,[Cell_Id]
           ,[Up_Volume]
           ,[Down_Volume]
           ,[Cell_Latitude]
           ,[Cell_Longitude]
           ,[In_Trunk]
           ,[Out_Trunk]
           ,[Service_Type]
           ,[Service_VAS_Name], ID  FROM [CDR]
      where (ignore='0' or IGNORE is null) AND ID > @MaxRecordID
   

ALTER INDEX ALL ON NormalCDR
REBUILD WITH (FILLFACTOR = 80, SORT_IN_TEMPDB = ON,
              STATISTICS_NORECOMPUTE = ON);