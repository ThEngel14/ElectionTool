USE [ElectionDB]
GO

/****** Object:  View [dbo].[BasicWahlkreisOverview]    Script Date: 03.01.2016 13:19:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER View [dbo].[BasicWahlkreisOverview] as
--information about a certain voting district
select w.Election_Id, 
	w.Wahlkreis_Id,
	k.Name as Wahlkreis_Name, 
	p.Participation, 
	w.Person_Id, 
	w.Title, 
	w.Firstname, 
	w.Lastname, 
	w.Party_Id, 
	w.Party_Name
from WinnerErststimme w 
	join ElectionParticipation p 
		on	w.Election_Id = p.Election_Id 
		and w.Wahlkreis_Id = p.Wahlkreis_Id
	join Wahlkreis k 
		on w.Wahlkreis_Id = k.Id
GO


