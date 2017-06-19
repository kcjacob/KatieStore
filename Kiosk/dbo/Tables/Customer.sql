CREATE TABLE [dbo].[Customer] (
    [ID]    INT            IDENTITY (1, 1) NOT NULL,
    [Name]  NVARCHAR (100) NULL,
    [Email] NVARCHAR (255) NULL,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID] ASC)
);

