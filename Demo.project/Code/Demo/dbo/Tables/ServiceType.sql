CREATE TABLE [dbo].[ServiceType] (
    [ID]          INT            NOT NULL,
    [Description] NVARCHAR (255) NOT NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_ServiceType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

