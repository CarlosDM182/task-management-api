USE TaskManagementDb;
GO

CREATE TABLE Users
(
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(150) NOT NULL,
    Email NVARCHAR(150) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,

    CONSTRAINT PK_Users
        PRIMARY KEY (Id),

    CONSTRAINT UQ_Users_Email
        UNIQUE (Email)
);
GO