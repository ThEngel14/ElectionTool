USE [ElectionDB]
GO

/****** Object:  View [dbo].[Ueberhangmandate]    Script Date: 03.01.2016 13:52:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER View [dbo].[Ueberhangmandate] as

--number:= |Erststimmen-Seats| -  |Zweitstimmen-Seats|

select e.Election_Id, 
	   e.Bundesland_Id, b.Name as Bundesland_Name,
	   e.Party_Id, p.Name as Party_Name,
		iif(e.NumberOfVictories > z.seats, e.NumberOfVictories-z.seats, 0  ) as Number
from SeatsGainedByErststimme e 
	join seatsGainedByZweitstimme z 
		on e.Election_Id = z.Election_Id
		and e.Bundesland_Id = z.Bundesland_Id
		and e.Party_Id = z.Party_Id
	join Bundesland b on e.Bundesland_Id = b.Id
	join Party p on e.Party_Id = p.Id





GO


