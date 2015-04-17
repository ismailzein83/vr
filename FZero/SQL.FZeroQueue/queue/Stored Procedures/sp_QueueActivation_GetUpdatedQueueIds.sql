
CREATE PROCEDURE queue.sp_QueueActivation_GetUpdatedQueueIds
	@AfterActivationID bigint
AS
BEGIN
	SET NOCOUNT ON;

    SELECT QueueID, MAX(ID) MaxID
    FROM queue.QueueActivation
    WHERE ID > @AfterActivationID
    GROUP BY QueueID
END