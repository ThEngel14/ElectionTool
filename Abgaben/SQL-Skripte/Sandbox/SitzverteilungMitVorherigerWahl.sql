ALTER view [dbo].[Sitzverteilung](Election_Id, OverallSeats, Party_Id, PartyName, SeatsParty, PercentParty, PreviousDiff) as 


with Seats as (
	select * 
	from Election as E cross apply dbo.IncreasingSeats_Ausgleichsmandate(E.Id)),

	 Verteilung as (
	select  s.Id as Election_Id, 
	        (Select sum(temp_Seats) 
				from Seats s1 
				where s.Id=s1.Id) as OverallSeats, s.Party_Id as Party_Id, p.Name as Party_Name, s.temp_Seats as SeatsParty,
			1.0*s.temp_Seats/(select sum(x.temp_Seats)
				from Seats x
				where s.Id = x.Id) as PercentParty
	from Seats s, Party p
	where p.Id = s.Party_Id)

select v1.*, v1.PercentParty - v2.PercentParty as PreviousDiff
from Verteilung v1 left join Verteilung v2 on v1.Party_Id = v2.Party_Id and v1.Election_Id = v2.Election_Id + 1