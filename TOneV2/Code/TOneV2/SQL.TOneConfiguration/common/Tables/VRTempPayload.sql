CREATE TABLE [common].[VRTempPayload] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Settings]    VARCHAR (MAX)    NULL,
    [CreatedBy]   INT              NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_VRTempPayload_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_VRTempPayload] PRIMARY KEY CLUSTERED ([ID] ASC)
);

