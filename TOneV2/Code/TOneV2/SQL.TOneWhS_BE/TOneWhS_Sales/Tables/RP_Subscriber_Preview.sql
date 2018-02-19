CREATE TABLE [TOneWhS_Sales].[RP_Subscriber_Preview] (
    [ID]                          BIGINT IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID]           BIGINT NOT NULL,
    [SubscriberID]                INT    NOT NULL,
    [Status]                      INT    NULL,
    [SubscriberProcessInstanceID] BIGINT NOT NULL,
    CONSTRAINT [PK_RP_Subscriber_Preview] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_RP_Subscriber_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_Subscriber_Preview]([ProcessInstanceID] ASC);

