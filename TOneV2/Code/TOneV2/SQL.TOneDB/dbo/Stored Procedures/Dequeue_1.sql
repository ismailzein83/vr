CREATE procedure [dbo].[Dequeue]
    @BatchSize int
AS

set nocount on

BEGIN TRANSACTION
    BEGIN TRY
        ;WITH TopResults AS
        (
            SELECT TOP (@BatchSize) ID AS ID FROM [PricelistEmailQueueMetaData]
            WITH (UPDLOCK, HOLDLOCK)
            WHERE [Status] IN (0,6,7) and ID in( select P.ID from 
                                                 [PricelistEmailQueueMetaData] P,[PricelistEmailQueueData] QD
                                                  WHERE ( QD.[MetaDataItemID] = P.ID)
                                                 )
            ORDER BY [DateTimeSentBySupplier] ASC
        )
       
       
            UPDATE P
            SET P.[STATUS] = (case when p.Status=0 then 1 else p.Status end)
            OUTPUT INSERTED.*,QD.[AttachmentData],QD.[FileName]
            FROM [PricelistEmailQueueMetaData] P,[PricelistEmailQueueData] QD, TopResults R
            WHERE (P.ID = R.ID AND QD.[MetaDataItemID] = P.ID)
    END TRY
   
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
    END CATCH;
   
    IF @@TRANCOUNT > 0
    BEGIN
        COMMIT TRANSACTION;
END