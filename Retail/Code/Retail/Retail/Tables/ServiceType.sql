CREATE TABLE [Retail].[ServiceType] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [OldID]       INT              NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Title]       NVARCHAR (255)   NULL,
    [Settings]    NVARCHAR (MAX)   NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_ServiceType_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_ServiceType] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ServiceType_ServiceType] FOREIGN KEY ([ID]) REFERENCES [Retail].[ServiceType] ([ID]),
    CONSTRAINT [IX_ServiceType`_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);



