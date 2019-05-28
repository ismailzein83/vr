CREATE TABLE [CRMFixedOper].[Customer_Residential] (
    [ID]            BIGINT         NOT NULL,
    [MotherName]    NVARCHAR (255) NULL,
    [PersonalID]    NVARCHAR (255) NULL,
    [IDReleaseDate] DATETIME       NULL,
    [BirthPlace]    NVARCHAR (255) NULL,
    [BirthDate]     DATETIME       NULL,
    [NationalityID] INT            NULL,
    [CareerID]      INT            NULL,
    CONSTRAINT [PK__Resident__3214EC2703317E3D] PRIMARY KEY CLUSTERED ([ID] ASC)
);

