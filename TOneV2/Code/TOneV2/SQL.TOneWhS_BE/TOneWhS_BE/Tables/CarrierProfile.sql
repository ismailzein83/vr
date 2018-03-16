CREATE TABLE [TOneWhS_BE].[CarrierProfile] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [Settings]         NVARCHAR (MAX) NULL,
    [ExtendedSettings] NVARCHAR (MAX) NULL,
    [timestamp]        ROWVERSION     NULL,
    [SourceID]         VARCHAR (50)   NULL,
    [IsDeleted]        BIT            NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_CarrierProfile_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    CONSTRAINT [PK_CarrierProfile] PRIMARY KEY CLUSTERED ([ID] ASC)
);









