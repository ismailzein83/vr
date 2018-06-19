CREATE TABLE [common].[VRPop3ReadMessageId] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [ConnectionID]     UNIQUEIDENTIFIER NOT NULL,
    [SenderIdentifier] VARCHAR (255)    NULL,
    [MessageId]        NVARCHAR (400)   NOT NULL,
    [MessageTime]      DATETIME         NOT NULL,
    [timestamp]        ROWVERSION       NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_VRPop3ReadMessageId_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [IX_VRPop3ReadMessageId_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);






GO
CREATE CLUSTERED INDEX [IX_VRPop3ReadMessageId_ConnSenderTime]
    ON [common].[VRPop3ReadMessageId]([ConnectionID] ASC, [SenderIdentifier] ASC, [MessageTime] ASC);

