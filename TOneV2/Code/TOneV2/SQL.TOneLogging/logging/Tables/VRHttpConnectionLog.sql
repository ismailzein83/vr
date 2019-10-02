CREATE TABLE [logging].[VRHttpConnectionLog] (
    [Id]                 BIGINT           NOT NULL,
    [VRHttpConnectionId] UNIQUEIDENTIFIER NULL,
    [BaseURL]            NVARCHAR (MAX)   NULL,
    [Path]               NVARCHAR (MAX)   NULL,
    [Parameters]         NVARCHAR (MAX)   NULL,
    [RequestHeaders]     NVARCHAR (MAX)   NULL,
    [RequestBody]        NVARCHAR (MAX)   NULL,
    [RequestTime]        DATETIME         NOT NULL,
    [ResponseHeaders]    NVARCHAR (MAX)   NULL,
    [Response]           NVARCHAR (MAX)   NULL,
    [ResponseTime]       DATETIME         NULL,
    [ResponseStatusCode] INT              NULL,
    [IsSucceded]         BIT              NULL,
    [Exception]          NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_VRHttpConnectionLog] PRIMARY KEY NONCLUSTERED ([Id] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_VRHttpConnectionLog_RequestTime]
    ON [logging].[VRHttpConnectionLog]([RequestTime] ASC);

