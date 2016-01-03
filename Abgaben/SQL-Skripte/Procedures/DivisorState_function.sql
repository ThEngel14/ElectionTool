USE [ElectionDB]
GO

/****** Object:  UserDefinedFunction [dbo].[DivisorState]    Script Date: 14.12.2015 12:18:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO







ALTER  FUNCTION [dbo].[DivisorState](@Election_ID int, @Bundesland_ID int, @initDivisor int, @Seats int ) RETURNS int AS

/*alter  FUNCTION [dbo].[DivisorState_test](@Election_ID int, @Bundesland_ID int, @initDivisor int, @Seats int, @PartyList as PartyListParam READONLY ) RETURNS int AS

*/


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


