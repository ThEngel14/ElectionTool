--Create views and create procedures. 


--please mind: insert ERststimmen, insert Zweitstimmen are created by this scirpt, but have to be called separately. 


/*authors: Thomas Engel | Markus Schnappinger

ET, The ELection Tool. 
Project based on the lecture Databasesystems in the Elite graduate program Software Engineering hosted by University Augsburg, LMU Munich, and TU Munich

*/


USE [ElectionDB]
GO


Create  FUNCTION [dbo].[Divisor](@Election_ID int) RETURNS int with schemabinding AS
BEGIN
	declare @Seats int = 598;
	declare @SumOfSeats int;
	declare @TempDivisor int;

	--initial value of Divisor by naive Divison: Population/available_seats
	set @TempDivisor = (Select sum(poB.Count) from dbo.PopulationBundesland poB
				where poB.Election_Id = @Election_ID
			  ) / @Seats;


	--temporary seats of each state, added up for comparison with actual amount of seats
	/*this is a clone-father*/
	set @SumOfSeats = (select Sum(temp.seats)
						from (select round(1.0* poB.Count/@TempDivisor, 0) as seats
								from dbo.PopulationBundesland poB 
								where poB.Election_Id= @Election_ID) temp); 


	while @SumOfSeats != @Seats
	begin
		if @SumOfSeats > @Seats
		begin
			set @TempDivisor = @TempDivisor + 3; --if there are to many seats given to the states, the divisor has to be incremented in order to reduce to quotient (which are the single seats) 	
		end --if
		else
		begin
			set @TempDivisor = @TempDivisor -250; --above comment ^^ vice versa	
		end --else

		--repeat computation of seats for every state with new temporary Divisor
		/*this is a clone from above*/
	set @SumOfSeats = (select Sum(temp.seats)
						from (select round(1.0* poB.Count/@TempDivisor, 0) as seats
								from dbo.PopulationBundesland poB 
								where poB.Election_Id= @Election_ID) temp
			);
	end--while

	return @TempDivisor;
END;--function

GO

/*-----------*/

Create View [dbo].[ErststimmeAggregated] as


/* used for special mode: 
comment first command out if computation based on single votes is demanded, 

otherwise use first function*/

-- Based on already aggregated table
select Election_Id, Wahlkreis_Id, Person_Id, Amount from ErststimmeAmount

-- Based on single votes
--select Election_Id, Wahlkreis_Id, Person_Id, count(*) from Erststimme group by Election_Id, Wahlkreis_Id, Person_Id
GO

/*-----------*/
Create View [dbo].[ZweitstimmeAggregated] as

/*used for special mode: select data based on single votes: open this view, comment first function out, un-comment second function*/

-- Based on already aggregated table
select Election_Id, Party_Id, Wahlkreis_Id, Amount from ZweitstimmeAmount

-- Based on single votes
--select Election_Id, Party_Id, Wahlkreis_Id, count(*) from Zweitstimme group by Election_Id, Party_Id, Wahlkreis_Id

GO

/*-----------*/


Create View [dbo].[WinnerErststimme] as

With ExtendedErststimmeAggregated (Election_Id, Wahlkreis_Id, Person_Id, RankAmount) as (
		select Election_Id, 
			Wahlkreis_Id,
			 Person_Id, 
			 rank() over (partition by election_id, wahlkreis_id order by amount desc)
		from ErststimmeAggregated
)

select e.Election_Id,
        e.Wahlkreis_Id,
	    k.Name as Wahlkreis_Name, 
        e.Person_Id,
	    p.Title, 
		p.Firstname,
		p.Lastname, 
        pa.Id as Party_Id,
	    pa.Name as Party_Name
from ExtendedErststimmeAggregated e 
	left join Person p 
		on e.Person_Id = p.Id
	join PartyAffiliation af 
		on p.Id = af.Person_Id 
		and e.Election_Id = af.Election_Id
	join Party pa 
		on af.Party_Id = pa.Id
	join Wahlkreis k 
		on e.Wahlkreis_Id = k.Id
where e.RankAmount = 1
GO


/*-----------*/

create View [dbo].[WinnerZweitstimme] as

With ExtendedZweitstimmeAggregated (Election_Id, Wahlkreis_Id, Party_Id, RankAmount) as (
		select Election_Id, Wahlkreis_Id, Party_Id, rank() over (partition by election_id, wahlkreis_id order by amount desc)
		from ZweitstimmeAggregated
)

select e.Election_Id, 
	   e.Wahlkreis_Id,
	   k.Name as Wahlkreis_Name,
	   e.Party_Id,
	   p.Name as Party_Name
from ExtendedZweitstimmeAggregated e 
	left join Party p 
		on e.Party_Id = p.Id
	join Wahlkreis k 
		on e.Wahlkreis_Id = k.Id
where e.RankAmount = 1
GO


/*-----------*/

create view [dbo].[WinnerFirstAndSecondVotes] as

select e.Election_Id,
	   b.Id as Bundesland_Id, b.Name as Bundesland_Name,
	   e.Wahlkreis_Id, e.Wahlkreis_Name,
	   e.Person_Id, e.Title, e.Firstname, e.Lastname,
	   e.Party_Id as FirstVote_Party_Id, e.Party_Name as FirstVote_Party_Name,
	   z.Party_Id as SecondVote_Party_Id, z.Party_Name as SecondVote_Party_Name
from WinnerErststimme e 
	join WinnerZweitstimme z 
		on e.Election_Id = z.Election_Id 
		and e.Wahlkreis_Id = z.Wahlkreis_Id
	join Wahlkreis k 
		on e.Wahlkreis_Id = k.Id
	join Bundesland b 
		on k.Bundesland_Id = b.Id
GO

/*-----------*/


Create View [dbo].[ElectionParticipation] as

Select p.Election_Id, p.Wahlkreis_Id, 1.0*p.Amount / a.Amount as Participation
from ParticipationAmount p 
	join AllowedToElectAmount a 
		on p.Election_Id = a.Election_Id 
		and p.Wahlkreis_Id = a.Wahlkreis_Id
GO


/*-----------*/


Create View [dbo].[BasicWahlkreisOverview] as
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

/*-----------*/

Create View [dbo].[ZweitstimmenState] as

 ( select votes.Election_Id as Election_Id, 
		wk.Bundesland_Id as Bundesland_Id,
		votes.Party_Id as Party_ID, 
		coalesce(sum(votes.Amount),0) as Amount
	from ZweitstimmeAmount votes, Wahlkreis wk 
	where votes.Wahlkreis_Id = wk.Id
	group by votes.Election_Id, wk.Bundesland_Id, votes.Party_Id
		); 
						
GO

/*-----------*/


create  FUNCTION [dbo].[DivisorParty](@Election_ID int, @Party_ID int, @Seats int) RETURNS int AS

/*THIS FUNCTION IS A CLONE | APPLY CHANGES IN THE TEMPLATE TO ALL OTHER DIVISOR FUNCTIONS*/

BEGIN
	 
	declare @SumOfSeats int;
	declare @TempDivisor int;

	--initial value of Divisor by naive Divison: Votes/available_seats
	set @TempDivisor = (Select Sum(z.Amount) /@Seats
						from ZweitstimmeAmount z
						where z.Election_Id=@Election_ID
						and z.Party_Id=@Party_ID);


	--temporary seats for each state added up for later comparison with actual amount of seats for this party
	/*this is a clone, apply changes also to the computation in the while-loop*/

	set @SumOfSeats = (select Sum(temp.seats)
						from (select round(1.0* votes.Amount/@TempDivisor, 0) as seats
								from ZweitstimmenState votes 
								where votes.Election_Id= @Election_ID
								and votes.Party_ID= @Party_ID
								) temp);

	while @SumOfSeats != @Seats
	begin
		if @SumOfSeats > @Seats
		begin
			set @TempDivisor = @TempDivisor + 251; --if there are to many seats given to the states, the divisor has to be incremented in order to reduce to quotient (which are the single seats) 	
		end --if
		else
		begin
			set @TempDivisor = @TempDivisor *0.45 * 2 ; --above comment ^^ vice versa	
		end --else

		--repeat computation of seats for every state with new temporary Divisor
		/*this is a clone from above*/
	set @SumOfSeats = (select Sum(temp.seats)
						from (select --votes.Bundesland_Id as b, 
								round(1.0* votes.Amount/@TempDivisor, 0) as seats
							from ZweitstimmenState votes 
							where votes.Election_Id= @Election_ID
								and votes.Party_ID= @Party_ID
								) temp
								--group by b
								);
					
	end--while

	return @TempDivisor;
END;--function

GO
/*-----------*/


create View [dbo].[ZweitstimmeWahlkreisOverview] as

With OverallVotes (Election_Id, Wahlkreis_Id, Overall) as (
		select election_id, 
			wahlkreis_id, 
			sum(amount) as Overall
		from ZweitstimmeAggregated
		group by election_id, wahlkreis_id),

	 Overview as (
		select z.Election_Id, 
			   z.Wahlkreis_Id, 
			   k.Name as Wahlkreis_Name,
			   z.Party_Id, 
			   p.Name as Party_Name,
			   z.Amount as Votes, 
			   o.Overall,
			    1.0*z.Amount / o.Overall as PercentVotes
		from ZweitstimmeAggregated z 
			join OverallVotes o 
				on z.Election_Id = o.Election_Id 
				and z.Wahlkreis_Id = o.Wahlkreis_Id
			join Wahlkreis k 
				on z.Wahlkreis_Id = k.Id
			left join Party p 
				on z.Party_Id = p.Id
)

select o.*,
	 o.PercentVotes - o2.PercentVotes as Previous
from Overview o 
	left join Overview o2 
		on o.Election_Id - 1 = o2.Election_Id 
		and o.Wahlkreis_Id = o2.Wahlkreis_Id 
		and o.Party_Id = o2.Party_Id
GO

/*-----------*/


Create view [dbo].[Parties5]  as

--parties above 5% threshold u parties with 3 or more Direktmandate
(
select votes.Election_Id, p.Id  
from Party p, ZweitstimmeAmount votes
where 1.00*(select sum(votes1.Amount) as PartyVotes
		from  ZweitstimmeAmount votes1
		where votes1.Party_Id=p.Id 
		and votes1.Election_Id=votes.Election_Id
		group by votes1.Election_Id, votes1.Party_Id) 
	/
		(select sum(votes2.Amount) as OverallVotes
		from ZweitstimmeAmount votes2
		where votes2.Election_Id = votes.Election_Id
		group by votes2.Election_Id)

	>=0.05
)


union
 ( 
	select wks.ElectionID, wks.MemberOfParty
	from Wahlkreissieger wks
	group by wks.ElectionID, wks.MemberOfParty
	having count(*) > 2 
	and wks.MemberOfParty!= 36 --36:= dummy party_Id for electable firstvote candidates who are not party affiliates.
)

GO

/*-----------*/
CREATE TYPE [dbo].[PartyListParam] AS TABLE(
	[Party_Id] [int] NULL
)
GO
/*-----------*/


create  FUNCTION [dbo].[DivisorState](@Election_ID int, @Bundesland_ID int, @initDivisor int, @Seats int ) RETURNS int AS




/*THIS FUNCTION IS A CLONE | APPLY CHANGES IN THE TEMPLATE TO ALL OTHER DIVISOR FUNCTIONS*/

BEGIN

	declare @SumOfSeats int;
	declare @TempDivisor int;

	declare @parties5 PartyListParam;
	insert into @parties5 
	select id from Parties5 where Election_Id = @Election_ID;
	


	--initial value of Divisor by naive Divison: Population/available_seats
	set @TempDivisor = @initDivisor ; 

	--temporary seats for each party that is above the 5% threshold, added up for later comparison with actual amount of seats for this state
	/*this is a clone, apply changes also to the computation in the while-loop*/

	set @SumOfSeats = (select Sum(temp.seats)
						from (select votes.Party_ID as Party_Id, round(1.0* votes.Amount/@TempDivisor, 0) as seats
								from ZweitstimmenState votes 
								where votes.Election_Id= @Election_ID
								and votes.Bundesland_Id= @Bundesland_ID
								and votes.Party_ID in (select * from @parties5)
														) temp
								);

	while @SumOfSeats != @Seats
	begin
		if @SumOfSeats > @Seats
		begin
		--evtl + 500
			set @TempDivisor = @TempDivisor + 999; --if there are to many seats given to the states, the divisor has to be incremented in order to reduce to quotient (which are the single seats) 	
		end --if
		else
		begin
			set @TempDivisor = @TempDivisor *0.9 ; --above comment ^^ vice versa	
		end --else

		--repeat computation of seats for every state with new temporary Divisor
		/*this is a clone from above*/
	set @SumOfSeats = (select Sum(temp.seats)
						from (select votes.Party_ID as Party_Id, round(1.0* votes.Amount/@TempDivisor, 0) as seats
								from ZweitstimmenState votes 
								where votes.Election_Id= @Election_ID
								and votes.Bundesland_Id= @Bundesland_ID
								and votes.Party_ID in (select * from @parties5)
														) temp
						);
						
	end--while

	return @TempDivisor;
END;--function
GO

/*-----------*/



create View [dbo].[ClosestErststimmeResult] as

With ExtendedErststimmeAggregated (Election_Id, Wahlkreis_Id, Person_Id, Amount, RankAmount) as (
		select Election_Id, Wahlkreis_Id, Person_Id, Amount, rank() over (partition by election_id, wahlkreis_id order by amount desc)
		from ErststimmeAggregated),

	 BaseErststimmeAggregated (Election_Id, Wahlkreis_Id, Person_Id, Amount, RankAmount, RankCompare) as (
		select *, case when RankAmount = 1 then 2 else 1 end
		from ExtendedErststimmeAggregated
)

select e.Election_Id, 
	   e.Wahlkreis_Id, k.Name as Wahlkreis_Name,
	   e.Person_Id, p.Title, p.Firstname, p.Lastname,
	   af.Party_Id, pa.Name as Party_Name,
	   e.Amount, ex.Amount as PreviousAmount, 
	   e.Amount - ex.Amount as Diff,
	   abs(e.Amount - ex.Amount) as AbsDiff
from BaseErststimmeAggregated e 
	join ExtendedErststimmeAggregated ex 
		on e.Election_Id = ex.Election_Id 
			and e.Wahlkreis_Id = ex.Wahlkreis_Id 
			and ex.RankAmount = e.RankCompare
    join Wahlkreis k 
		on e.Wahlkreis_Id = k.Id
	join Person p 
		on e.Person_Id = p.Id
	left join PartyAffiliation af 
		on p.Id = af.Person_Id 
		and e.Election_Id = af.Election_Id
	left join Party pa 
		on af.Party_Id = pa.Id
GO

/*-----------*/

Create View [dbo].[ClosestZweitstimmeResult] as

With ExtendedZweitstimmeAggregated (Election_Id, Wahlkreis_Id, Party_Id, Amount, RankAmount) as (
		select Election_Id, 
			Wahlkreis_Id, 
			Party_Id,
			Amount,
			rank() over (partition by election_id, wahlkreis_id order by amount desc)
		from zweitstimmeAggregated),


	 BaseZweitstimmeAggregated (Election_Id, Wahlkreis_Id, Party_Id, Amount, RankAmount, RankCompare) as (
		select *, case when RankAmount = 1 then 2 else 1 end
		from ExtendedZweitstimmeAggregated
)

select z.Election_Id, 
	   z.Wahlkreis_Id, k.Name as Wahlkreis_Name,
	   z.Party_Id, p.Name as Party_Name,
	   z.Amount, e.Amount as PreviousAmount, 
	   z.Amount - e.Amount as Diff,
	   abs(z.Amount - e.Amount) as AbsDiff
from BaseZweitstimmeAggregated z 
	join ExtendedZweitstimmeAggregated e 
		on z.Election_Id = e.Election_Id 
		and z.Wahlkreis_Id = e.Wahlkreis_Id 
		and e.RankAmount = z.RankCompare
	join Wahlkreis k 
		on z.Wahlkreis_Id = k.Id
	join Party p 
		on z.Party_Id = p.Id
GO


/*-----------*/


create View [dbo].[ErststimmeWahlkreisOverview] as

With OverallVotes (Election_Id, Wahlkreis_Id, Overall) as (
		select election_id, 
			wahlkreis_id, 
			sum(amount)
		from ErststimmeAggregated
		group by election_id, wahlkreis_id),

	 Overview as (
		select e.election_id,
			   e.wahlkreis_id, k.name as Wahlkreis_Name,
			   e.person_id, p.title, 
			   p.firstname, p.lastname,
			   pa.id as Party_Id, 
			   pa.name as Party_Name,
			   e.amount as Votes, 
			   o.overall, 
			   1.0*e.amount / o.overall as PercentVotes
		from ErststimmeAggregated e 
			join OverallVotes o 
				on e.Election_Id = o.Election_Id 
				and e.Wahlkreis_Id = o.Wahlkreis_Id
			join Wahlkreis k 
				on e.Wahlkreis_Id = k.Id
			left join Person p 
				on e.Person_Id = p.Id
			left join PartyAffiliation af 
				on p.Id = af.Person_Id 
				and e.Election_Id = af.Election_Id
			left join Party pa 
				on af.Party_Id = pa.Id
)

select o.*, o.PercentVotes - o2.PercentVotes as Previous
from Overview o 
	left join Overview o2 
		on o.Election_Id - 1 = o2.Election_Id 
		and o.Wahlkreis_Id = o2.Wahlkreis_Id 
		and o.Person_Id = o2.Person_Id
GO


/*-----------*/

create view [dbo].[SeatsPerState] WITH SCHEMABINDING as
--output: which ElectionID, which state, the right to fill how many seats

select poB.Election_Id as Election_Id, 
	 poB.Bundesland_Id as Bundesland_ID,
	 cast(round(poB.Count*1.0/dbo.Divisor(poB.Election_Id), 0) as int) as Seats  -- round to full int, no decimals
from dbo.PopulationBundesland poB

GO


/*-----------*/

Create view [dbo].[Wahlkreissieger] as
--actually computed from Erststimme, But a table ERststimmeAmount is created seperatly for performance reasons (no looping here to count votes) 


--output: Election, Winnerperson, district, partyAffiliation in this election <-- can be null!
select votes.Election_Id as ElectionID, 
	votes.Person_Id as Person_Id,
	votes.Wahlkreis_Id as Wahlkreis_Id, 
	(select party.Party_Id 
		from PartyAffiliation party 
		where party.Person_Id=votes.Person_Id 
		and party.Election_Id=votes.Election_Id)
	as MemberOfParty
from ErststimmeAmount votes 
 where votes.Amount = (select max(alle.Amount)
						from ErststimmeAmount alle
						where votes.Election_Id = alle.Election_Id 
						and	votes.Wahlkreis_Id = alle.Wahlkreis_Id)
 

GO

/*-----------*/

Create view [dbo].[SeatsGainedByErststimme] as
(  

select wks.ElectionId as Election_Id, 
	wk.Bundesland_Id as Bundesland_Id,
	wks.MemberOfParty as Party_Id,
	coalesce(count(wks.Person_Id),0) as numberOfVictories
from Wahlkreissieger wks, Wahlkreis wk
where wks.Wahlkreis_Id= wk.Id
group by wks.ElectionId, wk.Bundesland_Id, wks.MemberOfParty


)
GO

/*-----------*/


Create view [dbo].[seatsGainedByZweitstimme] as 

(select votes.Election_Id as Election_Id, 
	votes.Bundesland_Id as Bundesland_Id, 
	votes.party_Id as Party_Id, 
	cast(round(1.0*votes.Amount / dbo.divisorState(
													votes.Election_Id,
													votes.Bundesland_Id,
													 (Select poB.Count/s.Seats
															from PopulationBundesland poB
															where poB.Bundesland_Id = votes.Bundesland_Id 
															and poB.Election_Id=votes.Election_Id), --starting value of @tempDivisor
													s.Seats)
		,0) as int) 
	as seats
 from  ZweitstimmenState votes, SeatsPerState s
where 
	s.Election_Id = votes.Election_Id
	and s.Bundesland_ID = votes.Bundesland_Id
	and votes.Party_ID in (select p.Id from Parties5 p
							where p.Election_Id=votes.Election_ID))

GO


/*-----------*/

Create View [dbo].[DistributionWithinStates] as
--stored as "MinSeatsForPartyPerState.sql
--output max(seats gained by zweitstimme, seats gained by erststimme

select distinct
	z.Election_Id as Election_Id, 
	z.Bundesland_Id as Bundesland_Id,
	iif(coalesce(e.NumberOfVictories,0) > coalesce(z.Seats,0), e.NumberOfVictories, z.seats ) as Seats,
	z.Party_Id as Party_Id
 from seatsGainedByZweitstimme z 
		left join  SeatsGainedByErststimme e
			on e.Election_Id=z.Election_Id 
			and e.Bundesland_Id=z.Bundesland_Id
			and e.Party_Id = z.Party_Id
 
GO

/*-----------*/


create View [dbo].[minSeatsPerParty_Bundeslevel] as

select Election_Id, Party_Id, sum(Seats) as minSeatsBund
from DistributionWithinStates 
group by Election_Id,Party_Id

GO

/*-----------*/


Create View [dbo].[Ueberhangmandate] as

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

/*-----------*/

create view [dbo].[Sitzverteilung](Election_Id, OverallSeats, Party_Id, PartyName, SeatsParty, PercentParty) as 

with Seats as (select * from Election as E cross apply dbo.IncreasingSeats_Ausgleichsmandate(E.Id))  
--increasingSeats function can not be called in the same way as the scalar divisor functions, because it is a tablevalue function.

select  s.Id as Election_Id, 
	(Select sum(temp_Seats) 
		from Seats s1 
		where s.Id=s1.Id) as OverallSeats, 
	s.Party_Id as Party_Id, 
	p.Name as Party_Name, 
	s.temp_Seats as SeatsParty,
	(Select 1.0*s.temp_Seats	/ 	(select sum(x.temp_Seats) from Seats x
											where s.Id = x.Id)
			)
	 as PercentParty
from Seats s, Party p
where p.Id = s.Party_Id

GO


/*-----------*/

create  FUNCTION [dbo].[IncreasingSeats_Ausgleichsmandate](@Election_ID int) RETURNS @tempSeats table(Party_Id int, temp_Seats int, minSeats int, ZweitstimmenCount int) AS

/*THIS FUNCTION IS A CLONE | APPLY CHANGES IN THE TEMPLATE TO ALL OTHER DIVISOR FUNCTIONS*/

BEGIN
		
		INSERT INTO @tempSeats (Party_Id, temp_Seats, minSeats,ZweitstimmenCount)
		SELECT msppB.Party_Id, 0, msppB.minSeatsBund, (select sum(zsA.Amount)
													from ZweitstimmeAmount zsA
													where zsA.Election_Id=@Election_ID 
													and zsa.Party_Id = msppB.Party_Id)
		from minSeatsPerParty_Bundeslevel msppB
		where msppB.Election_Id=@Election_ID


		declare @MinSeats int = (Select Sum(minSeats)
							from @tempSeats);

		declare @SumPopulation int = (select sum(poB.Count)
								from PopulationBundesland poB
								where poB.Election_Id=@Election_ID);
								 
		declare @counter int = (select count(*) from @tempSeats where temp_Seats < minSeats); --counts for how many parties the invariant is not fullfilled

		

	while @counter>0
	begin
		
				set @MinSeats = @MinSeats +1; --if there are to less seats distributed to the parties, increase number of seats and update seat amount 

				update @tempSeats
				set temp_Seats =  round( 1.0*ZweitstimmenCount/(1.0*@SumPopulation/@MinSeats),0)

				set @counter= (select count(*) from @tempSeats where temp_Seats < minSeats);

		
						
	end--while


	return; -- intended: return @tempSeats, but this is handled in the function's header
END--function


GO


/*-----------*/




Create View [dbo].[ParliamentMembers] (Election_Id, Person_Id, Title, Firstname, Lastname, HowCome, Party_Id, Party_Name, Bundesland_Id, Bundesland_Name, Wahlkreis_Id, Wahlkreis_Name) as 


with SeatsGainedByZweitstimme1 as  (
/*clone from SeatsGainedByZweitstimme-View. variying DivisorFunction**/

	select votes.Election_Id as Election_Id, 
		votes.Bundesland_Id as Bundesland_Id,
		 votes.party_Id as Party_Id, 
		cast(round(1.0*votes.Amount/dbo.divisorParty(votes.Election_Id, votes.Party_ID, s.SeatsParty),0)as int)  as seats
	from  ZweitstimmenState votes, Sitzverteilung s 
	where s.Election_Id=votes.Election_Id 
		and s.Party_Id = votes.Party_ID
		and votes.Party_ID in (select p.Id from Parties5 p
								where p.Election_Id=votes.Election_ID)),

SeatsGainedByErststimme1 as (
	select * from SeatsGainedbyErststimme

	union
	
	--Parties, who did not gain seats by erststimme with numberOfVictories 0
	select p5.Election_Id as Election_Id, b.Id as Bundesland_Id, p5.Id as Party_Id, 0 as numberOfVictories
	from Parties5 p5, Bundesland b
	where p5.Id not in (select Party_Id from SeatsGainedByErststimme winners 
						where p5.Election_Id=winners.Election_Id 
						and winners.Bundesland_Id=b.Id)),


fillUp(Number, Election_Id, Bundesland_Id, Party_Id) as (
		select
			 iif(z.seats> e.numberOfVictories, z.seats-e.numberOfVictories, 0),
			 z.Election_Id, z.Bundesland_Id, z.Party_Id
		from SeatsGainedByZweitstimme1 z, SeatsGainedByErststimme1 e
		where z.Election_Id = e.Election_Id
		and z.Bundesland_Id = e.Bundesland_Id
		and z.Party_Id = e.Party_Id),


--temp view for all candidates for each party in each state, where candidate not a firstvote winner
CandidateTemp(Election_Id, Bundesland_Id, Party_Id, Person_Id, Rang) as (
	Select cand.Election_Id,
		cand.Bundesland_Id, 
		partyAff.Party_Id, 
		partyAff.Person_Id, 
		rank() over (partition by cand.Election_Id, cand.Bundesland_Id, partyAff.Party_Id order by cand.Position) as Rang
from CandidateList cand, PartyAffiliation partyAff
where cand.Election_Id = partyAff.Election_Id
	and cand.Person_Id = partyAff.Person_Id
	and partyAff.Person_Id not in (select wks.Person_Id 
									from Wahlkreissieger wks 
									where wks.ElectionID = partyAff.Election_Id)

)



--alle firstvote winners

select wks.ElectionID as Election_Id, 
	   pers.Id as Person_Id,
	   pers.Title,
		pers.Firstname,
		pers.Lastname,
       'Erststimme' as HowCome, 
	   p.Id as Party_Id,
	   p.Name as Party_Name,
	   b.Id as Bundesland_Id,
	   b.Name as Bundesland_Name,
	   wk.Id as Wahlkreis_Id,
	   wk.Name as Wahlkreis_Name
 from Wahlkreissieger wks, Person pers, Party p, Wahlkreis wk, Bundesland b
where pers.Id=wks.Person_Id
	and wks.MemberOfParty=p.Id
	and wks.Wahlkreis_Id = wk.Id
	and wk.Bundesland_Id= b.Id

union

--|Seats for a Party in a state| - |seats not available anymore, because already demanded by bistrict winner|
  

Select  candidates.Election_Id as Election_Id, 
        pers.Id as Person_Id, pers.Title, pers.Firstname, pers.Lastname, 
	    'Listenplatz' as HowCome, 
		part.Id as Party_Id, part.Name as Party_Name,
		b.Id as Bundesland_Id, b.Name as Bundesland_Name,
		null as Wahlkreis_Id, null as Wahlkreis_Name
	from fillUp fu, CandidateTemp candidates, Person pers, Bundesland b, Party part
	where --join
		fu.Election_Id = candidates.Election_Id
	and fu.Bundesland_Id = candidates.Bundesland_Id
	and fu.Party_Id = candidates.Party_Id
	--join for select information 
	and pers.Id= candidates.Person_Id
	and part.Id= candidates.Party_Id
	and b.Id = candidates.Bundesland_Id
	--logic
	and candidates.Rang <= fu.Number

GO


/*-----------*/


CREATE PROCEDURE InsertErststimmen
AS

Declare     @id int,
			@electionId int,
			@wahlkreisId int,
            @personId int,
			@amount int

delete from Erststimme

Declare curP cursor For

  select Id, Election_Id, Wahlkreis_Id, Person_Id, Amount
  from ErststimmeAmount

OPEN curP 
Fetch Next From curP Into @id, @electionId, @wahlkreisId, @personId, @amount

While @@Fetch_Status = 0 Begin
	With numTable(num) as (select (num)
					From (Values(0), (1), (2), (3), (4), (5), (6), (7), (8), (9)) as MyTable(num)),
		idTable(id) as (select row_number() over ( order by a.num)
					from numTable a, numTable b, numTable c, numTable d, numTable e, numTable f)

	Insert into Erststimme(Election_Id, Wahlkreis_Id, Person_Id)
	select Election_Id, Wahlkreis_Id, Person_Id
	from ErststimmeAmount s, idTable t
	where s.Id = @id and t.id <= @amount

	Fetch Next From curP Into @id, @electionId, @wahlkreisId, @personId, @amount

End -- End of Fetch

Close curP
Deallocate curP
Go


/*-----------*/

CREATE PROCEDURE InsertZweitstimmen
AS

Declare     @id int,
			@electionId int,
			@partyId int,
			@wahlkreisId int,
			@amount int

delete from Zweitstimme

Declare curP cursor For

  select Id, Election_Id, Party_Id, Wahlkreis_Id, Amount
  from ZweitstimmeAmount

OPEN curP 
Fetch Next From curP Into @id, @electionId, @partyId, @wahlkreisId, @amount

While @@Fetch_Status = 0 Begin
	With numTable(num) as (select (num)
					From (Values(0), (1), (2), (3), (4), (5), (6), (7), (8), (9)) as MyTable(num)),
		idTable(id) as (select row_number() over ( order by a.num)
					from numTable a, numTable b, numTable c, numTable d, numTable e)

	Insert into Zweitstimme(Election_Id, Party_Id, Wahlkreis_Id)
	select Election_Id, Party_Id, Wahlkreis_Id
	from ZweitstimmeAmount s, idTable t
	where s.Id = @id and t.id <= @amount

	Fetch Next From curP Into @id, @electionId, @partyId, @wahlkreisId, @amount

End -- End of Fetch

Close curP
Deallocate curP
Go


/*-----------*/
