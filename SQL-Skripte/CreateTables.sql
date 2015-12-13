-- Basic relations

CREATE TABLE [dbo].[Bundesland] (
    [Id]   INT            NOT NULL,
    [Name] NVARCHAR (127) NOT NULL UNIQUE,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Election] (
    [Id]             INT      IDENTITY (1, 1) NOT NULL,
    [Date]           DATETIME NOT NULL,
    [SeatsBundestag] INT      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[InvalidTokenRequest] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [IP]        VARCHAR (127) NOT NULL,
    [Token]     VARCHAR (127) NOT NULL,
    [Timestamp] DATETIME    NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Party] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (127) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Name] ASC)
);

CREATE TABLE [dbo].[Person] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Title]     VARCHAR (31)   NULL,
    [Firstname] NVARCHAR (127) NULL,
    [Lastname]  NVARCHAR (127) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[UsedToken]
(
	[TokenString] VARCHAR(127) NOT NULL PRIMARY KEY
)

CREATE TABLE [dbo].[Wahlkreis] (
    [Id]            INT            NOT NULL,
    [Name]          NVARCHAR (127) NOT NULL UNIQUE,
    [Bundesland_Id] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([Bundesland_Id]) REFERENCES [dbo].[Bundesland] ([Id])
);

-- Based on basic relations

CREATE TABLE [dbo].[AllowedToElect] (
    [Election_Id]  INT NOT NULL,
    [Person_Id]    INT NOT NULL,
    [Wahlkreis_Id] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Election_Id] ASC, [Person_Id] ASC),
    FOREIGN KEY ([Election_Id]) REFERENCES [dbo].[Election] ([Id]),
    FOREIGN KEY ([Person_Id]) REFERENCES [dbo].[Person] ([Id]),
    FOREIGN KEY ([Wahlkreis_Id]) REFERENCES [dbo].[Wahlkreis] ([Id])
);

CREATE TABLE [dbo].[AllowedToElectAmount] (
    [Election_Id]  INT NOT NULL,
    [Wahlkreis_Id] INT NOT NULL,
    [Amount]       INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Election_Id] ASC, [Wahlkreis_Id] ASC),
    FOREIGN KEY ([Election_Id]) REFERENCES [dbo].[Election] ([Id]),
    FOREIGN KEY ([Wahlkreis_Id]) REFERENCES [dbo].[Wahlkreis] ([Id])
);

CREATE TABLE [dbo].[CandidateList] (
    [Person_Id]     INT NOT NULL,
    [Election_Id]   INT NOT NULL,
    [Bundesland_Id] INT NOT NULL,
    [Position]      INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Person_Id] ASC, [Election_Id] ASC),
    FOREIGN KEY ([Person_Id]) REFERENCES [dbo].[Person] ([Id]),
    FOREIGN KEY ([Election_Id]) REFERENCES [dbo].[Election] ([Id]),
    FOREIGN KEY ([Bundesland_Id]) REFERENCES [dbo].[Bundesland] ([Id])
);

CREATE TABLE [dbo].[Erststimme] (
    [Id]           INT NOT NULL,
    [Election_Id]  INT NOT NULL,
    [Wahlkreis_Id] INT NOT NULL,
    [Person_Id]    INT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([Election_Id]) REFERENCES [dbo].[Election] ([Id]),
    FOREIGN KEY ([Person_Id]) REFERENCES [dbo].[Person] ([Id]),
    FOREIGN KEY ([Wahlkreis_Id]) REFERENCES [dbo].[Wahlkreis] ([Id]),
    FOREIGN KEY ([Person_Id]) REFERENCES [dbo].[Person] ([Id])
);

CREATE TABLE [dbo].[ErststimmeAmount] (
    [Id]           INT IDENTITY (1, 1) NOT NULL,
    [Election_Id]  INT NOT NULL,
    [Wahlkreis_Id] INT NOT NULL,
    [Person_Id]    INT NULL,
    [Amount]       INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([Election_Id]) REFERENCES [dbo].[Election] ([Id]),
    FOREIGN KEY ([Person_Id]) REFERENCES [dbo].[Person] ([Id]),
    FOREIGN KEY ([Wahlkreis_Id]) REFERENCES [dbo].[Wahlkreis] ([Id])
);

CREATE TABLE [dbo].[IsElectableCandidate] (
    [Election_Id]  INT NOT NULL,
    [Person_Id]    INT NOT NULL,
    [Wahlkreis_Id] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Election_Id] ASC, [Person_Id] ASC),
    FOREIGN KEY ([Election_Id]) REFERENCES [dbo].[Election] ([Id]),
    FOREIGN KEY ([Person_Id]) REFERENCES [dbo].[Person] ([Id]),
    FOREIGN KEY ([Wahlkreis_Id]) REFERENCES [dbo].[Wahlkreis] ([Id])
);

CREATE TABLE [dbo].[IsElectableParty] (
    [Id]            INT IDENTITY (1, 1) NOT NULL,
    [Election_Id]   INT NOT NULL,
    [Party_Id]      INT NOT NULL,
    [Bundesland_Id] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([Election_Id]) REFERENCES [dbo].[Election] ([Id]),
    FOREIGN KEY ([Party_Id]) REFERENCES [dbo].[Party] ([Id]),
    FOREIGN KEY ([Bundesland_Id]) REFERENCES [dbo].[Bundesland] ([Id])
);

CREATE TABLE [dbo].[Participation] (
    [Election_Id] INT NOT NULL,
    [Person_Id]   INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Election_Id] ASC, [Person_Id] ASC),
    FOREIGN KEY ([Election_Id]) REFERENCES [dbo].[Election] ([Id]),
    FOREIGN KEY ([Person_Id]) REFERENCES [dbo].[Person] ([Id])
);

CREATE TABLE [dbo].[ParticipationAmount] (
    [Election_Id]  INT NOT NULL,
    [Wahlkreis_Id] INT NOT NULL,
    [Amount]       INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Election_Id] ASC, [Wahlkreis_Id] ASC),
    FOREIGN KEY ([Election_Id]) REFERENCES [dbo].[Election] ([Id]),
    FOREIGN KEY ([Wahlkreis_Id]) REFERENCES [dbo].[Wahlkreis] ([Id])
);

CREATE TABLE [dbo].[PartyAffiliation] (
    [Person_Id]   INT NOT NULL,
    [Election_Id] INT NOT NULL,
    [Party_Id]    INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Person_Id] ASC, [Election_Id] ASC),
    FOREIGN KEY ([Person_Id]) REFERENCES [dbo].[Person] ([Id]),
    FOREIGN KEY ([Election_Id]) REFERENCES [dbo].[Election] ([Id]),
    FOREIGN KEY ([Party_Id]) REFERENCES [dbo].[Party] ([Id])
);

CREATE TABLE [dbo].[PopulationBundesland] (
    [Election_Id]   INT NOT NULL,
    [Bundesland_Id] INT NOT NULL,
    [Count]         INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Election_Id] ASC, [Bundesland_Id] ASC),
    FOREIGN KEY ([Election_Id]) REFERENCES [dbo].[Election] ([Id]),
    FOREIGN KEY ([Bundesland_Id]) REFERENCES [dbo].[Bundesland] ([Id])
);

CREATE TABLE [dbo].[Zweitstimme] (
    [Id]           INT NOT NULL,
    [Election_Id]  INT NOT NULL,
    [Party_Id]     INT NULL,
    [Wahlkreis_Id] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([Election_Id]) REFERENCES [dbo].[Election] ([Id]),
    FOREIGN KEY ([Wahlkreis_Id]) REFERENCES [dbo].[Wahlkreis] ([Id]),
    FOREIGN KEY ([Party_Id]) REFERENCES [dbo].[Party] ([Id])
);

CREATE TABLE [dbo].[ZweitstimmeAmount] (
    [Id]           INT IDENTITY (1, 1) NOT NULL,
    [Election_Id]  INT NOT NULL,
    [Party_Id]     INT NULL,
    [Wahlkreis_Id] INT NOT NULL,
    [Amount]       INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([Election_Id]) REFERENCES [dbo].[Election] ([Id]),
    FOREIGN KEY ([Party_Id]) REFERENCES [dbo].[Party] ([Id]),
    FOREIGN KEY ([Wahlkreis_Id]) REFERENCES [dbo].[Wahlkreis] ([Id])
);

