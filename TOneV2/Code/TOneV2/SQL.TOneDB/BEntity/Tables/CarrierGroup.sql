CREATE TABLE [BEntity].[CarrierGroup] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (255) NOT NULL,
    [ParentID]  INT            NULL,
    [timestamp] ROWVERSION     NULL,
    CONSTRAINT [PK_CarrierGroup_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);

