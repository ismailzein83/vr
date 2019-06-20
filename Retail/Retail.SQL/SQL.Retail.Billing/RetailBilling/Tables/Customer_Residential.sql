CREATE TABLE [RetailBilling].[Customer_Residential] (
    [ID]            BIGINT         NOT NULL,
    [MotherName]    NVARCHAR (255) NULL,
    [PersonalID]    NVARCHAR (255) NULL,
    [IDReleaseDate] DATETIME       NULL,
    [BirthPlace]    NVARCHAR (255) NULL,
    [BirthDate]     DATETIME       NULL,
    [NationalityID] INT            NULL,
    [CareerID]      INT            NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

