<<<<<<< HEAD
USE [ElectionDB]
GO

/****** Object:  View [dbo].[Ueberhangmandate]    Script Date: 27.11.2015 14:15:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER View [dbo].[Ueberhangmandate] as

--number:= |Erststimmen-Seats| -  |Zweitstimmen-Seats|

select e.Election_Id as Election_Id, e.Bundesland_Id as Bundesland_Id, e.Party_Id as Party_Id, 
	iif(e.NumberOfVictories > z.seats, e.NumberOfVictories-z.seats, 0  ) as Number
from SeatsGainedByErststimme e, seatsGainedByZweitstimme z
where e.Election_Id = z.Election_Id
and e.Bundesland_Id = z.Bundesland_Id
and e.Party_Id = z.Party_Id



GO


=======
﻿create View [dbo].[Ueberhangmandate] as

--number:= |Erststimmen-Seats| -  |Zweitstimmen-Seats|

select e.Election_Id, 
	   e.Bundesland_Id, b.Name as Bundesland_Name,
	   e.Party_Id, p.Name as Party_Name,
	(Select Max(y) 
	from (
		values(e.NumberOfVictories), (z.seats)) as x(y)
		) - z.seats
	as Number
from SeatsGainedByErststimme e join seatsGainedByZweitstimme z on e.Election_Id = z.Election_Id
														      and e.Bundesland_Id = z.Bundesland_Id
															  and e.Party_Id = z.Party_Id
							   join Bundesland b on e.Bundesland_Id = b.Id
							   join Party p on e.Party_Id = p.Id
>>>>>>> 9256e2a50a9fb0100791566d9d4a22c396e03d58
