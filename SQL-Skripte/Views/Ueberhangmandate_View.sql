USE [ElectionDB]
GO

/****** Object:  View [dbo].[Ueberhangmandate]    Script Date: 22.11.2015 16:10:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER View [dbo].[Ueberhangmandate] as

--number:= |Erststimmen-Seats| -  |Zweitstimmen-Seats|

select e.Election_Id as Election_Id, e.Bundesland_Id as Bundesland_Id, e.Party_Id as Party_Id, 
	(Select Max(y) 
	from (
		values(e.NumberOfVictories), (z.seats)) as x(y)
		) - z.seats
	as Number
from SeatsGainedByErststimme e, seatsGainedByZweitstimme z
where e.Election_Id = z.Election_Id
and e.Bundesland_Id = z.Bundesland_Id
and e.Party_Id = z.Party_Id
GO


