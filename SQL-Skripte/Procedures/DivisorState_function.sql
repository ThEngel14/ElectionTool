USE [ElectionDB]
GO

/****** Object:  UserDefinedFunction [dbo].[DivisorState]    Script Date: 26.11.2015 17:35:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO








--drop function Divisor;
--GO
ALTER  FUNCTION [dbo].[DivisorState](@Election_ID int, @Bundesland_ID int,@Seats int) RETURNS int AS

/*THIS FUNCTION IS A CLONE | APPLY CHANGES IN THE TEMPLATE TO ALL OTHER DIVISOR FUNCTIONS*/

BEGIN
	/*ausgegliedert in aufrufende View, damit hier nicht jedesmal die Seats per Stare ausgeführt wird
	declare @Seats int = (Select s.Seats
							from SeatsPerState s
							where s.Election_Id = @Election_ID 
							and s.Bundesland_ID=@Bundesland_ID); */
	declare @SumOfSeats int;
	declare @TempDivisor int;

	--initial value of Divisor by naive Divison: Population/available_seats
	set @TempDivisor = (Select poB.Count/@Seats
						from PopulationBundesland poB
						where poB.Bundesland_Id = @Bundesland_ID 
						and poB.Election_Id=@Election_ID);


	--temporary seats for each party that is above the 5% threshold, added up for later comparison with actual amount of seats for this state
	/*this is a clone, apply changes also to the computation in the while-loop*/

	set @SumOfSeats = (select Sum(temp.seats)
						from (select votes.Party_ID as Party_Id, round(1.0* votes.Amount/@TempDivisor, 0) as seats
								from ZweitstimmenState votes 
								where votes.Election_Id= @Election_ID
								and votes.Bundesland_Id= @Bundesland_ID
								and votes.Party_ID in (select p.Id from Parties5 p
														where p.Election_Id=@Election_ID)
														) temp
								);

	while @SumOfSeats != @Seats
	begin
		if @SumOfSeats > @Seats
		begin
			set @TempDivisor = @TempDivisor + 2500; --if there are to many seats given to the states, the divisor has to be incremented in order to reduce to quotient (which are the single seats) 	
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
								and votes.Party_ID in (select p.Id from Parties5 p
														where p.Election_Id=@Election_ID)
														) temp
						);
						
	end--while

	return @TempDivisor;
END;--function







GO


