CREATE TABLE [Retail].[ServiceType] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Title]       NVARCHAR (255) NULL,
    [Settings]    NVARCHAR (MAX) NOT NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_ServiceType_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_ServiceType] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_ServiceType`_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

