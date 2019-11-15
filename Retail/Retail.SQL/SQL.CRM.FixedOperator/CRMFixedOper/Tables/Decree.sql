CREATE TABLE [CRMFixedOper].[Decree] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [DecreeNumber]     NVARCHAR (255) NULL,
    [DecreeDate]       DATE           NULL,
    [Documents]        NVARCHAR (MAX) NULL,
    [Notes]            NVARCHAR (MAX) NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [timestamp]        ROWVERSION     NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

