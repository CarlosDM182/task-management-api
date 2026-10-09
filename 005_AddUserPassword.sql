USE TaskManagementDb;
GO

ALTER TABLE Users
ADD PasswordHash NVARCHAR(255) NOT NULL
    CONSTRAINT DF_Users_PasswordHash
    DEFAULT '';
GO