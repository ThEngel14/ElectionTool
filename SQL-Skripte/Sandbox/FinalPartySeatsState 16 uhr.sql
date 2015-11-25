create View FinalPartySeatsState (Election_Id, Bundesland_Id, Party_Id, AmountListSeats) as

--im Prinzip alle Sichten noch einmal, da der Ablauf hierachisch genauso aufgebaut ist wie die Sitzverteilung,
--lediglich am Schluss noch einen anderen Divisor benutzt, um die Sitze der PArtei(inkl Ausgliechsmandate)
-- auf die Landeslisten zu verteilen 

with Divisor as (select Id as Election_Id, dbo.Divisor(id) as Divisor1 from Election ),


 --logic from SeatsPerState, but not Population as Dividend but delivered Zweitstimmen
PartySeats as (select  z.Election_Id as Election_Id, z.Party_Id as Party_Id, cast(round(z.Votes*1.0/d.Divisor1, 0) as int) as Seats
				from    (select zs.Election_Id as Election_Id, zs.Party_Id as Party_Id, Sum(zs.Amount) as Votes
						from ZweitstimmenState zs 
						where zs.Party_Id in (select p.Id from Parties5 p
											where p.Election_Id=zs.Election_ID)
						group by Election_Id, Party_Id) z
				, Divisor d
					where z.Election_Id=d.Election_Id),				
				--left join Divisor d on z.Election_Id = d.Election_Id),

DivisorParty as (select Election_Id, Party_Id, dbo.DivisorParty(Election_Id, Party_Id) as Divisor2
				from PartySeats),

FinalSeats as ( 
select votes.Election_Id as Election_Id, votes.Bundesland_Id as Bundesland_Id, votes.party_Id as Party_Id, 
	round(1.0*votes.Amount/dbo.divisorParty(votes.Election_Id, votes.Party_ID),0)  as AmountListSeats
 from  ZweitstimmenState votes)



select * from FinalSeats