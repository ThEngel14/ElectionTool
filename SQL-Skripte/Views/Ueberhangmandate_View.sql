create View [dbo].[Ueberhangmandate] as

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