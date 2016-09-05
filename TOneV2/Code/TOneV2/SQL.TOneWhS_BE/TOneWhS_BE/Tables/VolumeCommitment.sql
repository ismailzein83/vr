CREATE TABLE [TOneWhS_BE].[VolumeCommitment] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Settings]    NVARCHAR (MAX) NOT NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_VolumeCommitment_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_VolumeCommitment] PRIMARY KEY CLUSTERED ([ID] ASC)
);

